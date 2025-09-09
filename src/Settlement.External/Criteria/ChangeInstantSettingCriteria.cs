namespace Settlement.External.Criteria;



public class MatchListItem
{
    public int MatchId { get; set; }
    public bool Enable { get; set; }
}

public class ChangeInstantSettingCriteria : NonQueryCommand
{
    public string MatchList { get; set; }
    public override string GetSettingKey() => "NAP_Set_ToggleAutoInstant_OWP";
    public override bool IsValid() => true;
}
