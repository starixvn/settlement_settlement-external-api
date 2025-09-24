using System.Text.Json.Serialization;
using Settlement.External.Extensions;

namespace Settlement.External.Models;

public class MatchResult
{
    [JsonPropertyName("mid")]
    public int MatchId { get; set; }

    [JsonPropertyName("ct")]
    [JsonConverter(typeof(CustomDateTimeConverter))]
    public DateTimeOffset ChangeTime { get; set; }
}
public class MatchResultChangeResponse
{
    public List<List<MatchResult>> Results { get; set; } = new();

    public string ErrorMsg { get; set; } = string.Empty;
}
