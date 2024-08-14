using MinimalApi.Endpoint;
using Settlement.External.Models;
using Settlement.External.Requests;
using Settlement.External.Services;
using Settlement.ExternalApi.EndpointFilters;

namespace Settlement.ExternalApi.MatchStatusEndpoints;

public class UpdateMatchStatusEndpoint(MatchSettlementService matchSettlementService, ValidationRequestService validationService)
    : IEndpoint<IResult, SettlementStatusUpdRequest>

{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("api/match/update-settlement-status",
            async (SettlementStatusUpdRequest request) => await HandleAsync(request))
            .Produces<Result>()
            .AddEndpointFilter<LogRequestFilter>()
            .WithTags("UpdateSettlementStatusEndpoints");
    }

    public async Task<IResult> HandleAsync(SettlementStatusUpdRequest request)
    {
        var validResult = validationService.ValidSettlementStatusRequest(request);

        if (validResult.IsFail)
        {
            return Results.BadRequest(validResult);
        }

        var result = await matchSettlementService.UpdateMatchSettlementStatus(request);

        return Results.Ok(result);
    }
}