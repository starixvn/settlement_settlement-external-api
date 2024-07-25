using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Settlement.External.DTOs;
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
        api.MapPost("multipleMatch", CreateMatchesResult).AddEndpointFilter<LogRequestFilter>();
        api.MapGet("getMatchResult", GetMatchResult).AddEndpointFilter<LogRequestFilter>();

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

    private static async Task<Results<Ok<Result>, BadRequest<Result>>> CreateMatchesResult(
        [FromBody] ListQueryRequest model,
        MatchServices services)
    {
        var multiValid = services.ValidationService.MultiValidation(model);

        if (multiValid.IsFail)
        {
            return TypedResults.BadRequest(multiValid);
        }

        var result = await services.ResultDetailsService.InsertMultiMatches(model);

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

    private static async Task<Results<Ok<MatchDto>, BadRequest<Result>>> GetMatchResult(
        int matchId,
        string securityToken,
        MatchServices services
        )
    {
        if (!services.ValidationService.ValidToken(securityToken))
        {
            return TypedResults.BadRequest(Result.ErrorSecurityToken);
        }

        var match = await services.ResultDetailsService.GetMatch(matchId);

        return TypedResults.Ok(match);
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