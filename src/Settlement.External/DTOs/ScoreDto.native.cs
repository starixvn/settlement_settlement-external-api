namespace Settlement.External.DTOs;

public class ScoreDto
{
    public int SourceId { get; set; }

    public int TypeId { get; set; }

    public string HomeValue { get; set; }

    public string AwayValue { get; set; }

    public DateTime CreatedDate { get; set; }
}