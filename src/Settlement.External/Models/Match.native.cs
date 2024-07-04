namespace Settlement.External.Models;

public class Match
{
    public int MatchId { get; set; }

    public List<Score> ResultInfo { get; set; } = new();
}