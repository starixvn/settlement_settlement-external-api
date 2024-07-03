using Fanex.Data.Repository;

namespace Settlement.External.Criteria;

public class MatchSettlementStatusUpdCriteria : NonQueryCommand
{
    public int MatchId { get; set; }
    public short Type { get; set; }
    public string Ip { get; set; }
    public string AdminId { get; set; }
    public bool IsEnabled { get; set; }
    public int ErrorCode { get; set; }
    public string ErrMsg { get; set; }

    public override string GetSettingKey() => "NAP_Set_SettlementStatusUpd";

    public override bool IsValid() => MatchId > 0;
}