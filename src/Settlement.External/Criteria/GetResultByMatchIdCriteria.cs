using Fanex.Data.Repository;

namespace Settlement.External.Criteria;

public class GetResultByMatchIdCriteria : CriteriaBase
{
    public int MatchId { get; set; }

    public override string GetSettingKey() => "AutoPayout_GetResultByMatchId";

    public override bool IsValid() => MatchId > 0;
}