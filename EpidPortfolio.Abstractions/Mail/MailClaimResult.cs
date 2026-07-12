namespace EpidPortfolio.Abstractions.Mail;

// 우편 보상 수령 결과.
[GenerateSerializer]
[Alias("epidportfolio.mail.mail-claim-result")]
public sealed record MailClaimResult
{
    [Id(0)]
    public Guid MailId { get; init; }

    [Id(1)]
    public MailClaimStatus Status { get; init; }

    [Id(2)]
    public MailAttachment[] Attachments { get; init; } = [];
}

[Alias("epidportfolio.mail.mail-claim-status")]
public enum MailClaimStatus
{
    Claimed = 0,
    AlreadyClaimed = 1,
    Expired = 2,
    NotFound = 3
}
