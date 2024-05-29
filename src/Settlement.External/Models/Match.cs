namespace Settlement.External.Models;

public class Match
{
    public Match()
    {
        ResultInfo = new List<Score>();
    }

    public int MatchId { get; set; }

    public List<Score> ResultInfo { get; set; }
}