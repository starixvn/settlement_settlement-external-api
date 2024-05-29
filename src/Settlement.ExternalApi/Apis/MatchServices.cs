using Settlement.External.Services;

namespace Settlement.ExternalApi.Apis;

public class MatchServices(ValidationRequestService validationService, ProviderResultService resultDetailsService, SoccerWayResultService soccerWayService)
{
    public ValidationRequestService ValidationService { get; } = validationService;
    public ProviderResultService ResultDetailsService { get; } = resultDetailsService;
    public SoccerWayResultService SoccerWayService { get; } = soccerWayService;
}