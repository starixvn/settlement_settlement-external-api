using MinimalApi.Endpoint;
using Settlement.External.Models;
using Settlement.External.Requests;
using Settlement.External.Services;
using Settlement.ExternalApi.EndpointFilters;

namespace Settlement.ExternalApi.MatchStatusEndpoints;

public class ChangeResultMatchEndpoint(MatchChangeResultService matchChangeResultService)
    : IEndpoint<IResult, ChangeResultMatchRequest>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("api/match/change-result",
            async (ChangeResultMatchRequest request) => await HandleAsync(request))
            .Produces<MatchResultChangeResponse>()
            .AddEndpointFilter<LogRequestFilter>()
            .WithTags("MatchStatusEndpoints");
    }

    public async Task<IResult> HandleAsync(ChangeResultMatchRequest request)
    {
        var validationResult = ChangeResultMatchRequestValidator.ValidateRequest(request);

        if (validationResult.IsFail)
        {
            return Results.BadRequest(new Result { ErrorCode = validationResult.ErrorCode, ErrorMsg = validationResult.ErrorMsg });
        }

        var matchResults = await matchChangeResultService.GetDistinctMatchesWithChanges(request);
        
        if(!string.IsNullOrEmpty(matchResults.ErrorMsg))
        {
            return Results.BadRequest(new Result { ErrorCode = 99, ErrorMsg = matchResults.ErrorMsg });
        }

        return Results.Ok(matchResults.Results);
    }
}