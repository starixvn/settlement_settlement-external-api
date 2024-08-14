using Settlement.External.Models;

namespace Settlement.External.Requests;

public class QueryRequest
{
    public string SecurityToken { get; set; }

    public int MatchId { get; set; }

    public List<Score> ResultInfo { get; set; }
}