using System.Diagnostics;
using System.Text;

namespace GameStore.API.Middlewares;

public class RequestLoggingMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task Invoke(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        context.Request.EnableBuffering();

        var requestBody = await ReadRequestBody(context.Request);
        var originalBodyStream = context.Response.Body;

        var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;

        context.Response.OnStarting(async () =>
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

            var logText =
                $"Timestamp: {logEntry.Timestamp}\n" +
                $"IP: {logEntry.IP}\n" +
                $"Method: {logEntry.Method}\n" +
                $"URL: {logEntry.URL}\n" +
                $"StatusCode: {logEntry.StatusCode}\n" +
                $"ElapsedMilliseconds: {logEntry.ElapsedMilliseconds}\n" +
                $"RequestBody: {logEntry.RequestBody}\n" +
                $"ResponseBody:\n{responseBody}\n" +
                new string('-', 100) + "\n";

            WriteLogToFile(logText);
        });

        try
        {
            await _next(context);
        }
        finally
        {
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