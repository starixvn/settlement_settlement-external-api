using Settlement.External.Constants;

namespace Settlement.External.Models;

public class Score
{
    public int SourceId { get; set; }

    public GoalType TypeId { get; set; }

    public string HomeValue { get; set; }

    public string AwayValue { get; set; }
}