using System.Text.Json.Serialization;

namespace Settlement.External.Models;

public class MatchResult
{
    [JsonPropertyName("mid")]
    public int MatchId { get; set; }

    [JsonPropertyName("ct")]
    public DateTime ChangeTime { get; set; }
}
