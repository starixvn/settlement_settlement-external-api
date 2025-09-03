using FluentValidation;
using Settlement.External.Models;

namespace Settlement.External.Requests;

public record MatchItem(int MatchId, string Remark);

public record ChangeInstantSettingRequest(List<MatchItem> MatchItems);

public abstract class MatchItemValidator : AbstractValidator<MatchItem>
{
    public static Result ValidateMatchItem(MatchItem item)
    {
        return item.MatchId <= 0 ? new Result { ErrorCode = 1, ErrorMsg = "MatchId must be greater than 0." } : new Result();
    }
}

public class ChangeInstantSettingRequestValidator : AbstractValidator<ChangeInstantSettingRequest>
{
    public static Result ValidateRequest(ChangeInstantSettingRequest request)
    {
        if (request.MatchItems == null || request.MatchItems.Count == 0)
        {
            return new Result { ErrorCode = 2, ErrorMsg = "MatchItems cannot be empty." };
        }

        foreach (var itemResult in request.MatchItems.Select(MatchItemValidator.ValidateMatchItem).Where(itemResult => itemResult.IsFail))
        {
            return itemResult;
        }

        return new Result();
    }
}