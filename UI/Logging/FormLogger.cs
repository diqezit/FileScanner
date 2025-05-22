namespace FileScanner.UI.Logging;

public sealed class FormLogger(TextBox logTextBox, string categoryName) : ILogger
{
    private const int MaxLogLines = 1000;
    private const int LinesToKeep = 500;
    private static readonly Dictionary<LogLevel, string> LogLevelMap = new()
    {
        [LogLevel.Trace] = "TRCE",
        [LogLevel.Debug] = "DBUG",
        [LogLevel.Information] = "INFO",
        [LogLevel.Warning] = "WARN",
        [LogLevel.Error] = "ERRO",
        [LogLevel.Critical] = "CRIT"
    };

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;

        var message = formatter(state, exception);
        var logEntry = FormatLogEntry(logLevel, message, exception);

        if (logTextBox.InvokeRequired)
            logTextBox.Invoke(() => AppendLogMessage(logEntry));
        else
            AppendLogMessage(logEntry);
    }

    private string FormatLogEntry(LogLevel logLevel, string message, Exception? exception)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss");
        var level = LogLevelMap.GetValueOrDefault(logLevel, "NONE");
        var category = categoryName.Length > 30
            ? "..." + categoryName[^27..]
            : categoryName;

        var logEntry = $"[{timestamp}] [{level}] {category}: {message}";

        if (exception != null)
            logEntry += $"{Environment.NewLine}Exception: {exception.Message}";

        return logEntry;
    }

    private void AppendLogMessage(string message)
    {
        logTextBox.AppendText(message + Environment.NewLine);
        logTextBox.SelectionStart = logTextBox.Text.Length;
        logTextBox.ScrollToCaret();

        if (logTextBox.Lines.Length > MaxLogLines)
            logTextBox.Lines = [.. logTextBox.Lines.Skip(MaxLogLines - LinesToKeep)];
    }
}