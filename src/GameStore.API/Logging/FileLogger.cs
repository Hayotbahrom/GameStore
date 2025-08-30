using System.Globalization;

namespace GameStore.API.Logging;

public sealed class FileLogger(string category, FileLoggerProvider provider) : ILogger
{
    private readonly string _category = category;
    private readonly FileLoggerProvider _provider = provider;

    public IDisposable BeginScope<TState>(TState state)
        where TState : notnull
        => default!;

    public bool IsEnabled(LogLevel logLevel) => logLevel >= _provider.Option.MinLevel;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        var now = _provider.Option.UseUtcTimeStamp ? DateTime.UtcNow : DateTime.Now;
        var timestamp = now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        var message = formatter(state, exception);

        var line = $"[{timestamp}] [{logLevel}] {_category} {message}" +
                   (exception != null ? Environment.NewLine + exception : string.Empty) +
                   Environment.NewLine;

        var path = _provider.ResolvePath(now);
        _provider.WriteLine(path, line);
    }
}