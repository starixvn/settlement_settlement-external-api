namespace Settlement.External.DTOs;

public class MatchDto
{
    public int MatchId { get; set; }

    public IList<ScoreDto> ResultInfo { get; set; }
}