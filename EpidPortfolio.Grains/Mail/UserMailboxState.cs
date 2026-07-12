namespace EpidPortfolio.Grains.Mail;

// 사용자 한 명의 우편함 상태.
[GenerateSerializer]
[Alias("epidportfolio.mail.user-mailbox-state")]
public sealed class UserMailboxState
{
    [Id(0)]
    public List<MailItemState> Mails { get; set; } = [];
}
