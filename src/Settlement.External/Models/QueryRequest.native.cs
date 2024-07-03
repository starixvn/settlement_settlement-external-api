namespace Settlement.External.Models;

public class QueryRequest
{
    public string SecurityToken { get; set; }

    public int MatchId { get; set; }

    public List<Score> ResultInfo { get; set; }
}