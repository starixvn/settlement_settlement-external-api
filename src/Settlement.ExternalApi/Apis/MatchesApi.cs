using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Settlement.External.DTOs;
using Settlement.External.Models;

namespace Settlement.ExternalApi.Apis;

public static class MatchesApi
{
    public static IEndpointRouteBuilder MapMatchApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/Match");

        api.MapGet("getMatchResult", GetMatchResult);
        api.MapPost("singleMatch", CreateMatchResult);
        api.MapPost("multipleMatch", CreateMatchesResult);
        api.MapPost("goalRecord", CreateGoalRecord);
        api.MapPost("updateSettlementStatus", UpdateMatchSettlementStatus);
        return app;
    }

    private static async Task<Ok<Result>> CreateGoalRecord(
        [FromBody] QueryRequest model,
        MatchServices services)
    {
        var valid = services.ValidationService.ValidResult(model);

        if (valid.IsFail) return TypedResults.Ok(valid);

        var result = await services.SoccerWayService.Insert(model);

        return TypedResults.Ok(result);
    }

    private static async Task<Ok<Result>> CreateMatchesResult(
        [FromBody] ListQueryRequest model,
        MatchServices services)
    {
        var multiValid = services.ValidationService.MultiValidation(model);

        if (multiValid.IsFail) return TypedResults.Ok(multiValid);

        var result = await services.ResultDetailsService.InsertMultiMatches(model);

        return TypedResults.Ok(result);
    }

    private static async Task<Ok<Result>> CreateMatchResult(
        [FromBody] QueryRequest model,
        MatchServices services)
    {
        var valid = services.ValidationService.ValidResult(model);

        if (valid.IsFail) return TypedResults.Ok(valid);

        var result = await services.ResultDetailsService.Insert(model);

        return TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<MatchDto>, Ok<Result>>> GetMatchResult(
        int matchId,
        string securityToken,
        MatchServices services
        )
    {
        if (!services.ValidationService.ValidToken(securityToken))
        {
            return TypedResults.Ok(Result.ErrorSecurityToken);
        }

        var match = await services.ResultDetailsService.GetMatch(matchId);

        return TypedResults.Ok(match);
    }

    private static async Task<Ok<Result>> UpdateMatchSettlementStatus(
        [FromBody] SettlementStatusUpdRequest model,
        MatchServices services)
    {
        var validResult = services.ValidationService.ValidSettlementStatusRequest(model);

        if (validResult.IsFail) return TypedResults.Ok(validResult);

        var result = await services.MatchSettlementService.UpdateMatchSettlementStatus(model);

        return TypedResults.Ok(result);
    }
}