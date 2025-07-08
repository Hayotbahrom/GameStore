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

            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            var problem = new
            {
                status = 500,
                message = ex.Message,
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
        }
    }
}
