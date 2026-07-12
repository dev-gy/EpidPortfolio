using EpidPortfolio.Abstractions.Mail;

namespace EpidPortfolio.Endpoints.Mail;

// 우편함 조회 API.
public static class MailEndpoints
{
    public static IEndpointRouteBuilder MapMailEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(
            "/v1/users/{userId:guid}/mails",
            async (Guid userId, IGrainFactory grainFactory) =>
            {
                // 사용자 ID로 우편함 Grain을 찾는다.
                var mailbox = grainFactory.GetGrain<IUserMailboxGrain>(userId);
                var mails = await mailbox.GetMailsAsync();

                return Results.Ok(mails);
            });

        endpoints.MapPost(
            "/v1/users/{userId:guid}/mails/{mailId:guid}/claim",
            async (
                Guid userId,
                Guid mailId,
                IGrainFactory grainFactory) =>
            {
                var mailbox = grainFactory.GetGrain<IUserMailboxGrain>(userId);

                var result = await mailbox.ClaimMailAsync(mailId);

                return result.Status switch
                {
                    MailClaimStatus.NotFound => Results.NotFound(result),
                    MailClaimStatus.Expired => Results.Json(
                        result,
                        statusCode: StatusCodes.Status410Gone),
                    _ => Results.Ok(result)
                };
            });

        endpoints.MapPost(
            "/v1/users/{userId:guid}/mails/claim-all",
            async (Guid userId, IGrainFactory grainFactory) =>
            {
                var mailbox = grainFactory
                    .GetGrain<IUserMailboxGrain>(userId);

                return Results.Ok(await mailbox.ClaimAllMailsAsync());
            });

        endpoints.MapDelete(
            "/v1/users/{userId:guid}/mails/{mailId:guid}",
            async (
                Guid userId,
                Guid mailId,
                IGrainFactory grainFactory) =>
            {
                var mailbox = grainFactory
                    .GetGrain<IUserMailboxGrain>(userId);

                var result = await mailbox.DeleteMailAsync(mailId);

                return result.Status switch
                {
                    MailDeleteStatus.Deleted => Results.NoContent(),
                    MailDeleteStatus.NotFound => Results.NotFound(result),
                    _ => Results.Conflict(result)
                };
            });

        return endpoints;
    }
}