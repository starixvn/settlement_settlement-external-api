using System.Text;
using Settlement.External.Common;
using Settlement.External.Constants;
using Settlement.External.Criteria;
using Settlement.External.Models;

namespace Settlement.External.Services;

public interface SoccerWayResultService
{
    Task<Result> Insert(GoalRecordRequest model);
}

public class SoccerWayResultServiceImp(IDynamicRepository repository) : SoccerWayResultService
{
    private const int Timer = 45;
    private const string HomeGoal = "H";
    private const string AwayGoal = "A";
    private const string Semicolon = ";";

    private int MatchId { get; set; }

    public async Task<Result> Insert(GoalRecordRequest model)
    {
        MatchId = model.MatchId;

        if (model.ResultInfo.Count == 0)
        {
            return await EmptyScoreInfo();
        }

        var results = new List<Result>
        {
            await ProcessForAutoPayout(model.ResultInfo),
            await ProcessForAutoInstant(model.ResultInfo)
        };

        var errorResults = results.Where(result => result.IsFail);

        return errorResults.Any()
            ? new Result
            {
                ErrorCode = ErrorCode.SoccerWayInsertFail,
                ErrorMsg = string.Join("\n", errorResults.Select(result => result.ErrorMsg))
            }
            : new Result();
    }

    private async Task<Result> ProcessForAutoPayout(List<SoccerWayScore> listScore)
    {
        var fullTimeScore = listScore
            .GroupBy(s => new { s.GoalTime, s.TotalScore })
            .Select(g => g.Last())
            .Where(s => s.TotalScore > 0).ToList();

        var halfTimeScore = fullTimeScore.Where(s => s.GoalTime <= Timer).ToList();

        return fullTimeScore.Any()
            ? await AddScoreAndGoal(halfTimeScore, fullTimeScore)
            : await EmptyScoreInfo();
    }

    private async Task<Result> ProcessForAutoInstant(List<SoccerWayScore> listScore)
    {
        var autoInstantScores = listScore.Select(score => new AutoInstantScore
        {
            Type = score.GoalTime <= 45 ? (short)GoalType.FirstHalf : (short)GoalType.SecondHalf,
            HomeValue = string.IsNullOrEmpty(score.HomeValue) ? "0" : score.HomeValue,
            AwayValue = string.IsNullOrEmpty(score.AwayValue) ? "0" : score.AwayValue
        });

        var autoInstantMatch = new AutoInstantMatch
        {
            Id = MatchId,
            Scores = autoInstantScores.ToArray()
        };

        var resourceXml = Helper.GeneralXmlString(autoInstantMatch);
        var criteria = new InsertMatchScoresCriteria
        {
            SourceId = Sources.SoccerWay,
            ResourceXML = resourceXml
        };

        var result = await InsertMatchScores(criteria);

        return result;
    }

    private async Task<Result> InsertSoccerWay(AddSoccerWayResultCriteria criteria)
    {
        await repository.ExecuteAsync(criteria);

        return new Result
        {
            ErrorCode = criteria.ErrCode,
            ErrorMsg = criteria.ErrMsg
        };
    }

    private async Task<Result> InsertMatchScores(InsertMatchScoresCriteria criteria)
    {
        await repository.ExecuteAsync(criteria);

        return new Result
        {
            ErrorCode = criteria.ErrCode,
            ErrorMsg = criteria.ErrMsg
        };
    }

    private async Task<Result> AddScoreAndGoal(List<SoccerWayScore> halfTimeScore, List<SoccerWayScore> fullTimeScore)
    {
        var error = string.Empty;
        var halfTimeScoreResult = await AddScore(halfTimeScore.LastOrDefault(), GoalType.HalfTime);

        if (halfTimeScoreResult.IsFail)
        {
            error = $"HalfTime Score: {halfTimeScoreResult.ErrorMsg}";
        }
        else
        {
            var halfTimeGoalResult = await AddGoal(halfTimeScore, GoalType.HalfTimeFirstGoalLastGoal);
            if (halfTimeGoalResult.IsFail)
            {
                error = $"HalfTime Goal: {halfTimeGoalResult.ErrorMsg}";
            }
        }

        error = halfTimeScore.Count == fullTimeScore.Count ? error : string.Empty;
        var fullTimeScoreResult = await AddScore(fullTimeScore.LastOrDefault(), GoalType.FullTime);
        if (fullTimeScoreResult.IsFail)
        {
            error = string.Join("\n", error, $"Full Time Score: {fullTimeScoreResult.ErrorMsg}");
        }
        else
        {
            var fullTimeGoalResult = await AddGoal(fullTimeScore, GoalType.FullTimeFirstGoalLastGoal);
            if (fullTimeGoalResult.IsFail)
            {
                error = string.Join("\n", error, $"Full Time Goal: {fullTimeGoalResult.ErrorMsg}");
            }
        }

        return string.IsNullOrWhiteSpace(error)
            ? new Result()
            : new Result { ErrorCode = ErrorCode.SoccerWayInsertFail, ErrorMsg = error };
    }

    private async Task<Result> EmptyScoreInfo()
    {
        var actions = new List<GoalType> { GoalType.HalfTime, GoalType.FullTime, GoalType.HalfTimeFirstGoalLastGoal, GoalType.FullTimeFirstGoalLastGoal };
        var result = new Result();

        foreach (var action in actions)
        {
            var value = action == GoalType.HalfTime || action == GoalType.FullTime ? "0" : string.Empty;
            var criteria = new AddSoccerWayResultCriteria
            {
                MatchId = MatchId,
                SourceId = Sources.SoccerWay,
                TypeId = action,
                HomeValue = value,
                AwayValue = value
            };

            if (action == GoalType.FullTimeFirstGoalLastGoal)
            {
                criteria.HomeValue = Semicolon;
            }

            result = await InsertSoccerWay(criteria);

            if (result.ErrorCode > 0)
            {
                return result;
            }
        }

        return result;
    }

    private async Task<Result> AddScore(SoccerWayScore score, GoalType typeId)
    {
        var criteria = new AddSoccerWayResultCriteria
        {
            MatchId = MatchId,
            SourceId = Sources.SoccerWay,
            TypeId = typeId,
            HomeValue = score == null ? "0" : score.HomeValue,
            AwayValue = score == null ? "0" : score.AwayValue
        };
        var result = await InsertSoccerWay(criteria);

        return result;
    }

    private async Task<Result> AddGoal(List<SoccerWayScore> listScore, GoalType typeId)
    {
        var goal = typeId == GoalType.FullTimeFirstGoalLastGoal ? FullTimeGoalRecord(listScore) : GoalRecord(listScore);
        var criteria = new AddSoccerWayResultCriteria
        {
            MatchId = MatchId,
            SourceId = Sources.SoccerWay,
            TypeId = typeId,
            HomeValue = goal,
            AwayValue = string.Empty
        };

        var result = await InsertSoccerWay(criteria);

        return result;
    }

    private static string GoalRecord(List<SoccerWayScore> listScore)
    {
        var goal = new StringBuilder();

        foreach (var score in listScore)
        {
            goal = score.IsHome ? goal.Append(HomeGoal) : goal.Append(AwayGoal);
        }

        return goal.ToString();
    }

    private static string FullTimeGoalRecord(List<SoccerWayScore> listScore)
    {
        var halfTimeScore = listScore.Where(s => s.GoalTime <= Timer).ToList();
        var fullTimeScore = listScore.Where(s => s.GoalTime > Timer).ToList();

        return string.Join(";", GoalRecord(halfTimeScore), GoalRecord(fullTimeScore));
    }
}