namespace Settlement.External.Criteria;

public class GetResultChangeCriteria : CriteriaBase
{
    public DateTime? FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public bool IsTest { get; set; }

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public override string GetSettingKey() => "NAP_Set_ResultChangeLog_Sel";

    public override bool IsValid() => ToDate != default;
}