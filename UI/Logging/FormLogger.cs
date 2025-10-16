// UI/Logging/FormLogger.cs
namespace FileScanner.UI.Logging;

public sealed class FormLogger(FormLoggerProvider provider, string categoryName) : ILogger
{
    private const int MaxLogLines = 1000;
    private const int LinesToTrim = 500;
    private readonly LogEntryFormatter _formatter = new(categoryName);

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        => null;

    public bool IsEnabled(LogLevel logLevel)
        => logLevel != LogLevel.None;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        var logTextBox = provider.LogTextBox;
        if (logTextBox is null || !IsEnabled(logLevel)) return;

        var message = formatter(state, exception);
        var logEntry = _formatter.Format(logLevel, message, exception);

        // UI updates must run on UI thread
        if (logTextBox.InvokeRequired)
            logTextBox.Invoke(() => AppendLogMessage(logTextBox, logEntry));
        else
            AppendLogMessage(logTextBox, logEntry);
    }

    // Prevents UI from freezing by limiting number of lines in textbox
    private static void AppendLogMessage(TextBox logTextBox, string message)
    {
        logTextBox.AppendText(message + Environment.NewLine);

        if (logTextBox.Lines.Length > MaxLogLines)
            // Keep recent logs by removing older half
            logTextBox.Lines = [.. logTextBox.Lines.Skip(LinesToTrim)];

        logTextBox.SelectionStart = logTextBox.TextLength;
        logTextBox.ScrollToCaret();
    }
}