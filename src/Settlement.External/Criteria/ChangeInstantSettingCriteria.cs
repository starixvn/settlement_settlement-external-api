namespace Settlement.External.Criteria;

public class ChangeInstantSettingCriteria : NonQueryCommand
{
    public string MatchList { get; set; }
    public override string GetSettingKey() => "";
    public override bool IsValid() => true;
}
