namespace Settlement.External.Requests;

public class SettlementStatusUpdRequest
{
    public int MatchId { get; set; }

    public short Type { get; set; }

    public bool IsEnabled { get; set; }

    public string SecurityToken { get; set; }
    public int AdminId { get; set; }
}