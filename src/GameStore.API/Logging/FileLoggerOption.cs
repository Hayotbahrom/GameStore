namespace GameStore.API.Logging;

public class FileLoggerOption
{
    public string DirectoryPath { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "Logs");

    public string FilePattern { get; set; } = "log_{yyyy-MM-dd}.txt";

    public LogLevel MinLevel { get; set; } = LogLevel.Information;

    public bool UseUtcTimeStamp { get; set; } = true;
}
