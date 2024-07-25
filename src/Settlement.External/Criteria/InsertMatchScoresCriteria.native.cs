namespace Settlement.External.Criteria;

public class InsertMatchScoresCriteria : NonQueryCommand
{
    public int SourceId { get; set; }

    public string ResourceXml { get; set; }

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public override string GetSettingKey() => "NAP_Set_AutoInstant_ProviderResultDetailIns";

    public override bool IsValid() => SourceId >= 0;
}