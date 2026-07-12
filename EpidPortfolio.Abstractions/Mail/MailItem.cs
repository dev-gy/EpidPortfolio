namespace EpidPortfolio.Abstractions.Mail;

// 우편 정보를 돌려줄 때 사용하는 데이터.
[GenerateSerializer]
[Alias("epidportfolio.mail.mail-item")]
public sealed record MailItem
{
    [Id(0)]
    public required Guid MailId { get; init; }

    [Id(1)]
    public required string Title { get; init; }

    [Id(2)]
    public required string Body { get; init; }

    [Id(3)]
    public DateTimeOffset ExpiresAt { get; init; }

    [Id(5)]
    public bool IsClaimed { get; init; }

    [Id(6)]
    public DateTimeOffset DeliveredAt { get; init; }

    [Id(7)]
    public MailAttachment[] Attachments { get; init; } = [];

    [Id(8)]
    public DateTimeOffset? ClaimedAt { get; init; }
}
