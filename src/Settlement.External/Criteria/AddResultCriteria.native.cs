using Settlement.External.Constants;

namespace Settlement.External.Criteria;

public class AddResultCriteria : NonQueryCommand
{
    public int MatchId { get; set; }

    public int SourceId { get; set; }

    public GoalType TypeId { get; set; }

    public string HomeValue { get; set; }

    public string AwayValue { get; set; }

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public override string GetSettingKey() => "AutoPayout_InsertResult";

    public override bool IsValid() => MatchId > 0;
}