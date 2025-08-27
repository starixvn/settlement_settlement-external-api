using MinimalApi.Endpoint;
using Settlement.External.Models;
using Settlement.External.Requests;
using Settlement.ExternalApi.EndpointFilters;

namespace Settlement.ExternalApi.InstantSettingEndpoints
{
    public class ChangeMatchesSettingEndpoint : IEndpoint<IResult, ChangeInstantSettingRequest>
    {
        public void AddRoute(IEndpointRouteBuilder app)
        {
            app.MapPost("api/matches/change-instant-setting",
                async (ChangeInstantSettingRequest request) => await HandleAsync(request))
                .Produces<Result>()
                .AddEndpointFilter<LogRequestFilter>()
                .WithTags("ChangeMatchesSettingEndpoint");
        }

        public async Task<IResult> HandleAsync(ChangeInstantSettingRequest request)
        {
            //throw new NotImplementedException();

            return Results.Ok(new Result());
        }
    }
}
