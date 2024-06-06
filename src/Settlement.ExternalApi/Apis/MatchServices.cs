using Settlement.External.Services;

namespace Settlement.ExternalApi.Apis;

public class MatchServices(
    ValidationRequestService validationService,
    ProviderResultService resultDetailsService,
    SoccerWayResultService soccerWayService,
    MatchSettlementService matchSettlementService)
{
    public ValidationRequestService ValidationService { get; } = validationService;
    public ProviderResultService ResultDetailsService { get; } = resultDetailsService;
    public SoccerWayResultService SoccerWayService { get; } = soccerWayService;
    public MatchSettlementService MatchSettlementService { get; } = matchSettlementService;
}