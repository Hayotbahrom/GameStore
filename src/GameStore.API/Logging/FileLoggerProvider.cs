using System.Collections.Concurrent;
using System.Globalization;
using Microsoft.Extensions.Options;

namespace GameStore.API.Logging;

public sealed class FileLoggerProvider : ILoggerProvider
{
    private readonly ConcurrentDictionary<string, object> _filelock = new();
    private readonly IOptionsMonitor<FileLoggerOption> _optionMonitor;

    public FileLoggerProvider(IOptionsMonitor<FileLoggerOption> optionsMonitor)
    {
        _optionMonitor = optionsMonitor;
        Directory.CreateDirectory(Option.DirectoryPath);
    }

    public FileLoggerOption Option => _optionMonitor.CurrentValue;

    public ILogger CreateLogger(string categoryName)
    {
        return new FileLogger(categoryName, this);
    }

    public void Dispose()
    {
    }

    public string ResolvePath(DateTime now)
    {
        var fileName = Option.FilePattern.Replace(
            "{yyyy-MM-dd}",
            now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));

        return Path.Combine(Option.DirectoryPath, fileName);
    }

    public void WriteLine(string path, string line)
    {
        var @lock = _filelock.GetOrAdd(path, _ => new object());
        lock (@lock)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.AppendAllText(path, line);
        }
    }
}
