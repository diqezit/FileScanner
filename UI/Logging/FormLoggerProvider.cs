// UI/Logging/FormLoggerProvider.cs
namespace FileScanner.UI.Logging;

public sealed class FormLoggerProvider : ILoggerProvider
{
    private readonly ConcurrentDictionary<string, FormLogger> _loggers = new();

    // MainForm sets this property after its controls are created
    // This decouples logger creation from UI initialization timing
    public TextBox? LogTextBox { get; set; }

    public ILogger CreateLogger(string categoryName) =>
        _loggers.GetOrAdd(
            categoryName,
            name => new FormLogger(this, name)
        );

    public void Dispose()
    {
        _loggers.Clear();
        LogTextBox = null;
    }
}