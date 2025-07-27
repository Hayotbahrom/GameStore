using System.Text.Json;
using GameStore.Application.Exceptions;

namespace GameStore.API.Middlewares;

public class ExceptionHandlerMiddleWare(RequestDelegate requestDelegate, ILogger<ExceptionHandlerMiddleWare> logger)
{
    private readonly RequestDelegate _next = requestDelegate;
    private readonly ILogger _logger = logger;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (CustomException ex)
        {
            _logger.LogError(ex, ex.Message);
            LogExceptionToFile(ex);

            context.Response.StatusCode = ex.StatusCode;
            context.Response.ContentType = "application/json";

            var problem = new
            {
                status = ex.StatusCode,
                message = ex.Message,
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            LogExceptionToFile(ex);

            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            var problem = new
            {
                status = 500,
                message = "An unexpected error occured. Please try again later.",
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
        }
    }

    private static void LogExceptionToFile(Exception ex)
    {
        var logDir = Path.Combine(Directory.GetCurrentDirectory(), "logs");
        Directory.CreateDirectory(logDir);

        var logPath = Path.Combine(logDir, $"{DateTime.UtcNow:yyyy-MM-dd}-exceptions.txt");

        var log = $"""
        [{DateTime.UtcNow:u}]
        Type: {ex.GetType().Name}
        Message: {ex.Message}
        StackTrace: {ex.StackTrace}
        {(ex.InnerException is not null ? $"InnerException: {ex.InnerException.Message}" : string.Empty)}
        -------------------------------------------------------------------------

        """;

        File.AppendAllText(logPath, log);
    }
}
