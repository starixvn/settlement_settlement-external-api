namespace Settlement.External.Models;

public class SettlementStatusUpdRequest
{
    public int MatchId { get; set; }
    public short Type { get; set; }
    public bool IsEnabled { get; set; }
    public string SecurityToken { get; set; }
}