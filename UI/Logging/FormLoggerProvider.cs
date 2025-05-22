namespace FileScanner.UI.Logging;

public sealed class FormLoggerProvider : ILoggerProvider
{
    private static TextBox? _logTextBox;
    private readonly ConcurrentDictionary<string, FormLogger> _loggers = new();

    public static void SetLogTextBox(TextBox textBox) => _logTextBox = textBox;

    public ILogger CreateLogger(string categoryName)
    {
        if (_logTextBox == null)
            return NullLogger.Instance;

        return _loggers.GetOrAdd(categoryName, name => new FormLogger(_logTextBox, name));
    }

    public void Dispose()
    {
        _loggers.Clear();
    }
}

public sealed class NullLogger : ILogger
{
    public static readonly NullLogger Instance = new();

    private NullLogger() { }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => false;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    { }
}