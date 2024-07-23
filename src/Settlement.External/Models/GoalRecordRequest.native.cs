namespace Settlement.External.Models;

public class GoalRecordRequest
{
    public string SecurityToken { get; set; }

    public int MatchId { get; set; }

    public List<SoccerWayScore> ResultInfo { get; set; }
}