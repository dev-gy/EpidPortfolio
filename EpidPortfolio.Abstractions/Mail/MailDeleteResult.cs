namespace EpidPortfolio.Abstractions.Mail;

// 우편 삭제 결과.
[GenerateSerializer]
[Alias("epidportfolio.mail.mail-delete-result")]
public sealed record MailDeleteResult
{
    [Id(0)]
    public Guid MailId { get; init; }

    [Id(1)]
    public MailDeleteStatus Status { get; init; }
}

[Alias("epidportfolio.mail.mail-delete-status")]
public enum MailDeleteStatus
{
    Deleted = 0,
    NotFound = 1,
    HasUnclaimedRewards = 2
}
