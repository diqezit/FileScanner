namespace FileScanner.UI.Services;

public sealed class FormLogger(TextBox logTextBox) : ILogger
{
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        var message = formatter(state, exception);
        LogMessage($"[{logLevel}] {message}");
    }

    private void LogMessage(string message)
    {
        if (logTextBox.InvokeRequired)
            logTextBox.Invoke(() => AppendLogMessage(message));
        else
            AppendLogMessage(message);
    }

    private void AppendLogMessage(string message) =>
        logTextBox.AppendText(
            $"{DateTime.Now:HH:mm:ss} {message}{Environment.NewLine}");
}