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
