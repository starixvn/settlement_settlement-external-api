using Fanex.Data.Repository;

namespace Settlement.External.Criteria;

public class UpdateSoccerWayResultCriteria : NonQueryCommand
{
    public int MatchId { get; set; }

    public int SourceId { get; set; }

    public int TypeId { get; set; }

    public string HomeValue { get; set; }

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public override string GetSettingKey() => "AutoPayout_InsertResult";

    public override bool IsValid() => MatchId > 0;
}