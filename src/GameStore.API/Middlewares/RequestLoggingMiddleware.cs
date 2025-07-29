using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace GameStore.API.Middlewares;

public class RequestLoggingMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
    };

    public async Task Invoke(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        context.Request.EnableBuffering();

        var requestBody = await ReadRequestBody(context.Request);
        var originalBodyStream = context.Response.Body;

        var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            responseBodyStream.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();
            responseBodyStream.Seek(0, SeekOrigin.Begin);

            var logEntry = new
            {
                Timestamp = DateTime.UtcNow.ToString("u"),
                IP = context.Connection.RemoteIpAddress?.ToString(),
                Method = context.Request.Method,
                URL = context.Request.Path,
                StatusCode = context.Response.StatusCode,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                RequestBody = requestBody,
                ResponseBody = responseBody,
            };

            var logText = JsonSerializer.Serialize(logEntry, options: _jsonOptions) +
                          Environment.NewLine + new string('-', 100) + Environment.NewLine;

            WriteLogToFile(logText);

            responseBodyStream.Seek(0, SeekOrigin.Begin);
            await responseBodyStream.CopyToAsync(originalBodyStream);
            context.Response.Body = originalBodyStream;
        }
    }

    private static async Task<string> ReadRequestBody(HttpRequest request)
    {
        request.Body.Position = 0;
        using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0;
        return body;
    }

    private static void WriteLogToFile(string log)
    {
        var logDir = Path.Combine(Directory.GetCurrentDirectory(), "logs");
        Directory.CreateDirectory(logDir);
        var logPath = Path.Combine(logDir, $"requests-{DateTime.UtcNow:yyyy-MM-dd}.txt");
        File.AppendAllText(logPath, log);
    }
}