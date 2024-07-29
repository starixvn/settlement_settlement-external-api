using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Settlement.External.Models;
using Settlement.ExternalApi.EndpointFilters;

namespace Settlement.ExternalApi.Apis;

public static class MatchesApi
{
    public static IEndpointRouteBuilder MapMatchApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/Match");

        api.MapPost("singleMatch", CreateMatchResult).AddEndpointFilter<LogRequestFilter>();
        api.MapPost("goalRecord", CreateGoalRecord).AddEndpointFilter<LogRequestFilter>();
        api.MapPost("update-settlement-status", UpdateMatchSettlementStatus).AddEndpointFilter<LogRequestFilter>();

        return app;
    }

    private static async Task<Results<Ok<Result>, BadRequest<Result>>> CreateGoalRecord(
        [FromBody] GoalRecordRequest model,
        MatchServices services)
    {
        var valid = services.ValidationService.ValidGoalRecordRequest(model);

        if (valid.IsFail)
        {
            return TypedResults.BadRequest(valid);
        }

        var result = await services.SoccerWayService.Insert(model);

        return TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<Result>, BadRequest<Result>>> CreateMatchResult(
        [FromBody] QueryRequest model,
        MatchServices services)
    {
        var valid = services.ValidationService.ValidResult(model);

        if (valid.IsFail)
        {
            return TypedResults.BadRequest(valid);
        }

        var result = await services.ResultDetailsService.Insert(model);

        return TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<Result>, BadRequest<Result>>> UpdateMatchSettlementStatus(
        [FromBody] SettlementStatusUpdRequest model,
        MatchServices services)
    {
        var validResult = services.ValidationService.ValidSettlementStatusRequest(model);

        if (validResult.IsFail)
        {
            return TypedResults.BadRequest(validResult);
        }

        var result = await services.MatchSettlementService.UpdateMatchSettlementStatus(model);

        return TypedResults.Ok(result);
    }
}