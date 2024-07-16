using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Settlement.External.Constants;

namespace Settlement.External.Models;

public class SoccerWayScore
{
    public int SourceId { get; set; }

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