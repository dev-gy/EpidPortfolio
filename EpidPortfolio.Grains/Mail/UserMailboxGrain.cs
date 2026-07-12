using EpidPortfolio.Abstractions.Mail;

namespace EpidPortfolio.Grains.Mail;

// 사용자 한 명의 우편함을 처리한다.
[GrainType("epidportfolio.mail.user-mailbox")]
public sealed class UserMailboxGrain(
    // 우편함 상태를 "mail" 저장소에 보관한다.
    // Microsoft Learn: IPersistentState를 사용한 Orleans Grain 영속화
    // https://learn.microsoft.com/ko-kr/dotnet/orleans/grains/grain-persistence/
    [PersistentState("mailbox", "mail")]
    IPersistentState<UserMailboxState> state)
    : Grain, IUserMailboxGrain
{
    public Task<MailItem[]> GetMailsAsync()
    {
        var now = DateTimeOffset.UtcNow;

        // 만료되지 않은 우편만 고른다.
        var mails = state.State.Mails
            .Where(mail => mail.ExpiresAt > now)
            .OrderByDescending(mail => mail.DeliveredAt)
            .Select(ToMailItem)
            .ToArray();

        return Task.FromResult(mails);
    }

    public async Task<MailItem> DeliverMailAsync(MailDelivery delivery)
    {
        // 전달받은 우편 내용을 확인한다.
        if (delivery.MailId == Guid.Empty)
        {
            throw new ArgumentException("MailId must not be empty.", nameof(delivery));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(delivery.Title);
        ArgumentException.ThrowIfNullOrWhiteSpace(delivery.Body);

        if (delivery.ExpiresAt <= DateTimeOffset.UtcNow)
        {
            throw new ArgumentOutOfRangeException(
                nameof(delivery),
                "ExpiresAt must be in the future.");
        }

        if (delivery.Attachments.Length > 20)
        {
            throw new ArgumentException(
                "A mail can have up to 20 attachments.",
                nameof(delivery));
        }

        foreach (var attachment in delivery.Attachments)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(
                attachment.RewardCode);

            if (attachment.Quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(delivery),
                    "Attachment quantity must be positive.");
            }
        }

        // 같은 우편이 이미 있는지 확인한다.
        var existing = state.State.Mails
            .FirstOrDefault(mail => mail.MailId == delivery.MailId);

        if (existing is not null)
        {
            return ToMailItem(existing);
        }

        var mail = new MailItemState
        {
            MailId = delivery.MailId,
            Title = delivery.Title,
            Body = delivery.Body,
            ExpiresAt = delivery.ExpiresAt,
            DeliveredAt = DateTimeOffset.UtcNow,
            Attachments = delivery.Attachments
                .Select(attachment => new MailAttachmentState
                {
                    RewardCode = attachment.RewardCode,
                    Quantity = attachment.Quantity
                })
                .ToList()
        };

        state.State.Mails.Add(mail);

        // 바뀐 우편함 상태를 저장한다.
        await state.WriteStateAsync();

        return ToMailItem(mail);
    }

    public async Task<MailClaimResult> ClaimMailAsync(Guid mailId)
    {
        var mail = state.State.Mails
            .FirstOrDefault(item => item.MailId == mailId);

        return await ClaimAsync(mailId, mail);
    }

    public async Task<MailClaimResult[]> ClaimAllMailsAsync()
    {
        var now = DateTimeOffset.UtcNow;
        var claimableMails = state.State.Mails
            .Where(mail =>
                mail.ExpiresAt > now &&
                !mail.IsClaimed &&
                mail.Attachments.Count > 0)
            .OrderBy(mail => mail.DeliveredAt)
            .ToArray();

        var results = new List<MailClaimResult>(claimableMails.Length);

        foreach (var mail in claimableMails)
        {
            results.Add(await ClaimAsync(mail.MailId, mail));
        }

        return results.ToArray();
    }

    public async Task<MailDeleteResult> DeleteMailAsync(Guid mailId)
    {
        var mail = state.State.Mails
            .FirstOrDefault(item => item.MailId == mailId);

        if (mail is null)
        {
            return new MailDeleteResult
            {
                MailId = mailId,
                Status = MailDeleteStatus.NotFound
            };
        }

        var isExpired = mail.ExpiresAt <= DateTimeOffset.UtcNow;
        var hasUnclaimedRewards =
            mail.Attachments.Count > 0 &&
            !mail.IsClaimed &&
            !isExpired;

        if (hasUnclaimedRewards)
        {
            return new MailDeleteResult
            {
                MailId = mailId,
                Status = MailDeleteStatus.HasUnclaimedRewards
            };
        }

        state.State.Mails.Remove(mail);
        await state.WriteStateAsync();

        return new MailDeleteResult
        {
            MailId = mailId,
            Status = MailDeleteStatus.Deleted
        };
    }

    private async Task<MailClaimResult> ClaimAsync(
        Guid mailId,
        MailItemState? mail)
    {
        if (mail is null)
        {
            return ClaimResult(mailId, MailClaimStatus.NotFound);
        }

        var attachments = mail.Attachments
            .Select(ToAttachment)
            .ToArray();

        if (mail.ExpiresAt <= DateTimeOffset.UtcNow)
        {
            return ClaimResult(
                mailId,
                MailClaimStatus.Expired,
                attachments);
        }

        if (mail.IsClaimed)
        {
            return ClaimResult(
                mailId,
                MailClaimStatus.AlreadyClaimed,
                attachments);
        }

        var userId = this.GetPrimaryKey();
        var rewardWallet = GrainFactory
            .GetGrain<IUserRewardWalletGrain>(userId);

        // 첨부된 보상을 사용자 잔액에 반영한다.
        await rewardWallet.GrantMailRewardsAsync(attachments);

        mail.IsClaimed = true;
        mail.ClaimedAt = DateTimeOffset.UtcNow;
        await state.WriteStateAsync();

        return ClaimResult(
            mailId,
            MailClaimStatus.Claimed,
            attachments);
    }

    private static MailClaimResult ClaimResult(
        Guid mailId,
        MailClaimStatus status,
        MailAttachment[]? attachments = null)
    {
        return new MailClaimResult
        {
            MailId = mailId,
            Status = status,
            Attachments = attachments ?? []
        };
    }

    private static MailItem ToMailItem(MailItemState mail)
    {
        // 저장 상태를 응답 데이터로 바꾼다.
        return new MailItem
        {
            MailId = mail.MailId,
            Title = mail.Title,
            Body = mail.Body,
            ExpiresAt = mail.ExpiresAt,
            DeliveredAt = mail.DeliveredAt,
            IsClaimed = mail.IsClaimed,
            ClaimedAt = mail.ClaimedAt,
            Attachments = mail.Attachments
                .Select(ToAttachment)
                .ToArray()
        };
    }

    private static MailAttachment ToAttachment(
        MailAttachmentState attachment)
    {
        return new MailAttachment
        {
            RewardCode = attachment.RewardCode,
            Quantity = attachment.Quantity
        };
    }

}
