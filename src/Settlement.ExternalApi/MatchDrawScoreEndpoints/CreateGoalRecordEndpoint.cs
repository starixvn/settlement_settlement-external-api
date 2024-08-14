using MinimalApi.Endpoint;
using Settlement.External.Models;
using Settlement.External.Requests;
using Settlement.External.Services;
using Settlement.ExternalApi.EndpointFilters;

namespace Settlement.ExternalApi.MatchDrawScoreEndpoints;

public class CreateGoalRecordEndpoint(SoccerWayResultService soccerWayService, ValidationRequestService validationService)
    : IEndpoint<IResult, GoalRecordRequest>

{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("api/match/goalRecord",
            async (GoalRecordRequest request) => await HandleAsync(request))
            .Produces<Result>()
            .AddEndpointFilter<LogRequestFilter>()
            .WithTags("DrawScoreAndGoalRecordEndpoints");
    }

    public async Task<IResult> HandleAsync(GoalRecordRequest request)
    {
        var valid = validationService.ValidGoalRecordRequest(request);

        if (valid.IsFail)
        {
            return Results.BadRequest(valid);
        }

        var result = await soccerWayService.Insert(request);

        return Results.Ok(result);
    }
}