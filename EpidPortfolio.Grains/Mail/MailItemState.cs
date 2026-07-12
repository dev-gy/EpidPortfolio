namespace EpidPortfolio.Grains.Mail;

// 저장소에 보관할 우편 한 건의 상태.
[GenerateSerializer]
[Alias("epidportfolio.mail.mail-item-state")]
public sealed class MailItemState
{
    [Id(0)]
    public Guid MailId { get; set; }

    [Id(1)]
    public string Title { get; set; } = string.Empty;

    [Id(2)]
    public string Body { get; set; } = string.Empty;

    [Id(3)]
    public DateTimeOffset ExpiresAt { get; set; }

    [Id(5)]
    public bool IsClaimed { get; set; }

    [Id(6)]
    public DateTimeOffset DeliveredAt { get; set; }

    [Id(7)]
    public List<MailAttachmentState> Attachments { get; set; } = [];

    [Id(8)]
    public DateTimeOffset? ClaimedAt { get; set; }
}

[GenerateSerializer]
[Alias("epidportfolio.mail.mail-attachment-state")]
public sealed class MailAttachmentState
{
    [Id(0)]
    public string RewardCode { get; set; } = string.Empty;

    [Id(1)]
    public long Quantity { get; set; }
}
