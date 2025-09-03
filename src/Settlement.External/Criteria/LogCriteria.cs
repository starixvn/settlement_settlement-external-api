namespace Settlement.External.Criteria;

public class LogCriteria(int matchId, string errorMessage, int actionId, int userId)
    : NonQueryCommand
{
    public int UserId { get; set; } = userId;

    public int? MatchId { get; set; } = matchId;

    public string Remark { get; set; } = errorMessage;

    public string RemarkJson { get; set; }

    public string Ip => string.Empty;

    public int ActionId { get; set; } = actionId;

    public override bool IsValid() => true;

    public override string GetSettingKey() => "WriteLog_UserLog_Settle";
}