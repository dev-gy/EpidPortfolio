namespace EpidPortfolio.Grains.Mail;

// 우편으로 받은 보상의 사용자별 잔액을 보관한다.
[GenerateSerializer]
[Alias("epidportfolio.mail.user-reward-wallet-state")]
public sealed class UserRewardWalletState
{
    [Id(0)]
    public Dictionary<string, long> Balances { get; set; } = [];
}
