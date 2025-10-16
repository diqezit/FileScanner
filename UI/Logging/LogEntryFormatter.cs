// UI/Logging/LogEntryFormatter.cs
namespace FileScanner.UI.Logging;

// Separates log message creation from delivery logic
internal sealed class LogEntryFormatter(string categoryName)
{
    private static readonly Dictionary<LogLevel, string> LogLevelMap = new()
    {
        [LogLevel.Trace] = "TRCE",
        [LogLevel.Debug] = "DBUG",
        [LogLevel.Information] = "INFO",
        [LogLevel.Warning] = "WARN",
        [LogLevel.Error] = "ERRO",
        [LogLevel.Critical] = "CRIT"
    };

    // Keep category names short for better log readability
    private readonly string _shortCategoryName = categoryName.Length > 30
        ? "..." + categoryName[^27..]
        : categoryName;

    public string Format(
        LogLevel logLevel,
        string message,
        Exception? exception)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss");
        var level = LogLevelMap.GetValueOrDefault(logLevel, "NONE");

        var logEntry = $"[{timestamp}] [{level}] {_shortCategoryName}: {message}";

        if (exception is not null)
            // Exception message is sufficient for UI log
            logEntry += $"{Environment.NewLine}  -> {exception.Message}";

        return logEntry;
    }
}