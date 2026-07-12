namespace EpidPortfolio.Abstractions.Mail;

// 우편 보상을 실제 사용자 보상에 더하는 기능.
[Alias("epidportfolio.mail.user-reward-wallet")]
public interface IUserRewardWalletGrain : IGrainWithGuidKey
{
    // 전달받은 보상을 사용자 잔액에 더한다.
    [Alias("grant-mail-rewards")]
    Task GrantMailRewardsAsync(MailAttachment[] attachments);
}
