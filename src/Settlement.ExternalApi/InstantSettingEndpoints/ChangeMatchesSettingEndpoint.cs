using FluentValidation.AspNetCore.Http;
using MinimalApi.Endpoint;
using Settlement.External.Models;
using Settlement.External.Requests;
using Settlement.External.Services;
using Settlement.ExternalApi.EndpointFilters;

namespace Settlement.ExternalApi.InstantSettingEndpoints
{
    public class ChangeMatchesSettingEndpoint(IInstantSettingService instantSettingService) : IEndpoint<IResult, ChangeInstantSettingRequest>
    {
        public void AddRoute(IEndpointRouteBuilder app)
        {
            app.MapPost("api/match/change-instant-setting",
                async (ChangeInstantSettingRequest request) => await HandleAsync(request))
                .Produces<Result>()
                .AddEndpointFilter<LogRequestFilter>()
                .AddEndpointFilter<FluentValidationEndpointFilter>()
                .WithTags("ChangeMatchesSettingEndpoint");
        }

        public async Task<IResult> HandleAsync(ChangeInstantSettingRequest request)
        {
            var result = await instantSettingService.ChangeInstantSetting(request);
            return Results.Ok(result);
        }
    }
}