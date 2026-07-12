namespace EpidPortfolio.Abstractions.Mail;

// 새 우편을 전달할 때 사용하는 데이터.
[GenerateSerializer]
[Alias("epidportfolio.mail.mail-delivery")]
public sealed record MailDelivery
{
    [Id(0)]
    public Guid MailId { get; init; }

    [Id(1)]
    public required string Title { get; init; }

    [Id(2)]
    public required string Body { get; init; }

    [Id(3)]
    public DateTimeOffset ExpiresAt { get; init; }

    [Id(4)]
    public MailAttachment[] Attachments { get; init; } = [];
}
