using Settlement.External.Constants;

namespace Settlement.External.Models;

public class Score
{
    public int SourceId { get; set; }

    public GoalType TypeId { get; set; }

    public string HomeValue { get; set; }

    public string AwayValue { get; set; }

    public bool IsHome { get; set; }

    public int GoalTime { get; set; }

    public int TotalScore
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(HomeValue) && !string.IsNullOrWhiteSpace(AwayValue))
            {
                return Convert.ToInt32(HomeValue) + Convert.ToInt32(AwayValue);
            }

            return 0;
        }
    }
}