namespace GameStore.API.Logging;

public static class LoggingBuilderExtention
{
    public static ILoggingBuilder AddFileLogging(this ILoggingBuilder builder, Action<FileLoggerOption> configure)
    {
        builder.Services.Configure(configure);
        builder.Services.AddSingleton<ILoggerProvider, FileLoggerProvider>();
        return builder;
    }
}
