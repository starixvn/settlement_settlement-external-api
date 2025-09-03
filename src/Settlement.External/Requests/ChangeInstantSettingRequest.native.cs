using FluentValidation;

namespace Settlement.External.Requests;

public record MatchItem(int MatchId, string Remark);

public record ChangeInstantSettingRequest(List<MatchItem> MatchItems);

public record MatchesChangedInstant(List<MatchItem> MatchItems);


public class MatchItemValidator : AbstractValidator<MatchItem>
{
    public MatchItemValidator()
    {
        RuleFor(x => x.MatchId)
            .GreaterThan(0).WithMessage("MatchId must be greater than 0.");
    }
}

public class ChangeInstantSettingRequestValidator : AbstractValidator<ChangeInstantSettingRequest>
{
    public ChangeInstantSettingRequestValidator()
    {
        RuleFor(x => x.MatchItems)
            .NotEmpty().WithMessage("MatchItems cannot be empty.");

        RuleForEach(x => x.MatchItems).SetValidator(new MatchItemValidator());
    }
}
