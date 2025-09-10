using FluentValidation;
using Settlement.External.Models;

namespace Settlement.External.Requests;

public record ChangeResultMatchRequest(string FromDate, string ToDate);

public class ChangeResultMatchRequestValidator : AbstractValidator<ChangeResultMatchRequest>
{
    public static Result ValidateRequest(ChangeResultMatchRequest request)
    {
        // Validate datetime format first
        if (!DateTime.TryParse(request.FromDate, out var fromDateTime))
        {
            return new Result { ErrorCode = 1, ErrorMsg = "Invalid fromDate format. Expected format: yyyy-MM-dd HH:mm:ss" };
        }

        if (!DateTime.TryParse(request.ToDate, out var toDateTime))
        {
            return new Result { ErrorCode = 1, ErrorMsg = "Invalid toDate format. Expected format: yyyy-MM-dd HH:mm:ss" };
        }

        // Validate date range
        return toDateTime < fromDateTime ? new Result { ErrorCode = 1, ErrorMsg = "From date must be before To date." } : new Result();
    }
}