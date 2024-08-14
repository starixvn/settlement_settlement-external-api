using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.Database;
using ILogger = Fanex.Logging.ILogger;

namespace Settlement.ExternalApi.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            await HandleException(context, exception);
        }
    }

    private async Task HandleException(HttpContext context, Exception exception)
    {
        var logMessage = new LogMessage($"Error from external Api: {exception.Message}")
            .AddSection("Database Info", DatabaseInfoGetters.GetDbExceptionInfo(exception))
            .AddSection("Server Info", CommonInfoGetters.GetServerInfo())
            .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(exception))
            .ToString();
        await logger.ErrorAsync(logMessage);

        var method = context.Request.Method.ToLowerInvariant();
        var requestedWith = context.Request.Headers.XRequestedWith;

        if (method is "post" or "put" or "delete" && requestedWith == "XMLHttpRequest")
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }

        await context.Response.WriteAsync(exception.Message);
        await context.Response.WriteAsync(Environment.NewLine);
        await context.Response.WriteAsync(exception.StackTrace ?? string.Empty);
    }
}