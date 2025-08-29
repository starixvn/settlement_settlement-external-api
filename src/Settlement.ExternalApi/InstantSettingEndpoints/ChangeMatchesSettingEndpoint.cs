using FluentValidation.AspNetCore.Http;
using MassTransit;
using MinimalApi.Endpoint;
using Settlement.External.Models;
using Settlement.External.Requests;
using Settlement.External.Services;
using Settlement.ExternalApi.EndpointFilters;

namespace Settlement.ExternalApi.InstantSettingEndpoints
{
    public class ChangeMatchesSettingEndpoint(IPublishEndpoint publishEndpoint, IInstantSettingService instantSettingService) : IEndpoint<IResult, ChangeInstantSettingRequest>
    {
        public void AddRoute(IEndpointRouteBuilder app)
        {
            app.MapPost("api/matches/change-instant-setting",
                async (ChangeInstantSettingRequest request) => await HandleAsync(request))
                .Produces<Result>()
                .AddEndpointFilter<LogRequestFilter>()
                .AddEndpointFilter<FluentValidationEndpointFilter>()
                .WithTags("ChangeMatchesSettingEndpoint");
        }

        public async Task<IResult> HandleAsync(ChangeInstantSettingRequest request)
        {
            await instantSettingService.ChangeInstantSetting(request);
            //await publishEndpoint.Publish(new MatchesChangedInstant(request.MatchItems));

            return Results.Ok(new Result());
        }
    }
}
