using MinimalApi.Endpoint;
using Settlement.External.Models;
using Settlement.External.Requests;
using Settlement.External.Services;
using Settlement.ExternalApi.EndpointFilters;

namespace Settlement.ExternalApi.MatchDrawScoreEndpoints;

public class CreateScoreEndpoint(ProviderResultService providerResultService, ValidationRequestService validationService)
        : IEndpoint<IResult, QueryRequest>

{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("api/match/singleMatch",
            async (QueryRequest request) => await HandleAsync(request))
            .Produces<Result>()
            .AddEndpointFilter<LogRequestFilter>();
    }

    public async Task<IResult> HandleAsync(QueryRequest request)
    {
        var valid = validationService.ValidResult(request);

        if (valid.IsFail)
        {
            return Results.BadRequest(valid);
        }

        var result = await providerResultService.Insert(request);

        return Results.Ok(result);
    }
}