namespace EpidPortfolio.Abstractions.Mail;

// 사용자 우편함에서 사용할 기능.
[Alias("epidportfolio.mail.user-mailbox")]
public interface IUserMailboxGrain : IGrainWithGuidKey
{
    // 받을 수 있는 우편을 조회한다.
    [Alias("get-mails")]
    Task<MailItem[]> GetMailsAsync();

    // 새 우편을 우편함에 넣는다.
    [Alias("deliver-mail")]
    Task<MailItem> DeliverMailAsync(MailDelivery delivery);

    // 우편 한 건의 보상을 받는다.
    [Alias("claim-mail")]
    Task<MailClaimResult> ClaimMailAsync(Guid mailId);

    // 받을 수 있는 모든 보상을 받는다.
    [Alias("claim-all-mails")]
    Task<MailClaimResult[]> ClaimAllMailsAsync();

    // 수령했거나 만료된 우편을 지운다.
    [Alias("delete-mail")]
    Task<MailDeleteResult> DeleteMailAsync(Guid mailId);
}
