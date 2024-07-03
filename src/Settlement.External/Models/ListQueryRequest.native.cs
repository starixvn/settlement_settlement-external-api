namespace Settlement.External.Models;

public class ListQueryRequest
{
    public string SecurityToken { get; set; }

    public IList<Match> Matches { get; set; }
}