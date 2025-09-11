namespace Settlement.External.Criteria;

public class GetResultChangeCriteria : CriteriaBase
{
    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public bool IsTest { get; set; }

    public override string GetSettingKey() => "NAP_Set_ResultChangeLog_Sel";

    public override bool IsValid() => FromDate <= ToDate;
}