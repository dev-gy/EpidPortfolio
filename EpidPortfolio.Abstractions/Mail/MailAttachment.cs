namespace EpidPortfolio.Abstractions.Mail;

// 우편에 들어가는 보상 한 종류.
[GenerateSerializer]
[Alias("epidportfolio.mail.mail-attachment")]
public sealed record MailAttachment
{
    [Id(0)]
    public required string RewardCode { get; init; }

    [Id(1)]
    public long Quantity { get; init; }
}
