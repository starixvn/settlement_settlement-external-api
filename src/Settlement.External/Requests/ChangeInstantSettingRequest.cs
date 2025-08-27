namespace Settlement.External.Requests;

public record MatchItem(int MatchId, string Remark);

public record ChangeInstantSettingRequest(List<MatchItem> MatchItems);
