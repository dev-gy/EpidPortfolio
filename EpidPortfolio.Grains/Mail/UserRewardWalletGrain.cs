using EpidPortfolio.Abstractions.Mail;

namespace EpidPortfolio.Grains.Mail;

// 우편 보상을 사용자 잔액에 더한다.
[GrainType("epidportfolio.mail.user-reward-wallet")]
public sealed class UserRewardWalletGrain(
    // Microsoft Learn: IPersistentState를 사용한 Orleans Grain 영속화
    // https://learn.microsoft.com/ko-kr/dotnet/orleans/grains/grain-persistence/
    [PersistentState("reward-wallet", "mail")]
    IPersistentState<UserRewardWalletState> state)
    : Grain, IUserRewardWalletGrain
{
    public async Task GrantMailRewardsAsync(MailAttachment[] attachments)
    {
        foreach (var attachment in attachments)
        {
            state.State.Balances.TryGetValue(
                attachment.RewardCode,
                out var quantity);

            state.State.Balances[attachment.RewardCode] =
                checked(quantity + attachment.Quantity);
        }

        await state.WriteStateAsync();
    }

}
