using Settlement.External.Constants;
using ErrorCodes = Settlement.External.Constants.ErrorCode;

namespace Settlement.External.Models;

[Serializable]
public class Result
{
    private string errorMsg = "Success";

    public int ErrorCode { get; set; }

    public string ErrorMsg
    {
        get
        {
            if (ErrorCode == 0)
            {
                errorMsg = "Success";
            }

            return errorMsg;
        }

        set => errorMsg = value;
    }

    public bool IsFail => ErrorCode > 0;

    public static Result ErrorSecurityToken => new() { ErrorCode = 1, ErrorMsg = ErrorMessage.IncorrectSecurityToken };

    public static Result ErrorHomeValue => new() { ErrorCode = ErrorCodes.InvalidHomeValue, ErrorMsg = ErrorMessage.InvalidHomeValue };

    public static Result ErrorAwayValue => new() { ErrorCode = ErrorCodes.InvalidAwayValue, ErrorMsg = ErrorMessage.InvalidAwayValue };

    public static Result ErrorParameter => new() { ErrorCode = ErrorCodes.InvalidParameter, ErrorMsg = ErrorMessage.InvalidParameter };
};