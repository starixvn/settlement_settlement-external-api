namespace Settlement.External.Criteria;

public class GetMatchDetailCriteria : CriteriaBase
{
    public string MatchIds { get; set; }

    public override string GetSettingKey() => "NAP_Set_MatchSelInfo";

    public override bool IsValid() => MatchIds.Length > 0;
}