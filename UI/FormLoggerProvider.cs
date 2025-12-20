namespace FileScanner.UI;

public sealed class FormLoggerProvider : ILoggerProvider
{
    readonly ConcurrentDictionary<string, FormLogger> _loggers = new();

    public TextBox? Target { get; set; }

    public ILogger CreateLogger(string cat) =>
        _loggers.GetOrAdd(cat, n => new(this, n));

    public void Dispose()
    {
        _loggers.Clear();
        Target = null;
    }
}

public sealed class FormLogger(FormLoggerProvider p, string cat) : ILogger
{
    static readonly Dictionary<LogLevel, string> Lvl = new()
    {
        [LogLevel.Trace] = "TRC",
        [LogLevel.Debug] = "DBG",
        [LogLevel.Information] = "INF",
        [LogLevel.Warning] = "WRN",
        [LogLevel.Error] = "ERR",
        [LogLevel.Critical] = "CRT"
    };

    readonly string _cat = cat.Length > 20
        ? $"...{cat[^17..]}"
        : cat;

    public IDisposable? BeginScope<T>(T s) where T : notnull => null;

    public bool IsEnabled(LogLevel l) => l != LogLevel.None;

    public void Log<T>(
        LogLevel l,
        EventId e,
        T s,
        Exception? ex,
        Func<T, Exception?, string> f)
    {
        var tb = p.Target;
        if (tb == null || !IsEnabled(l)) return;

        var time = DateTime.Now.ToString("HH:mm:ss");
        var lvl = Lvl.GetValueOrDefault(l, "???");
        var msg = $"[{time}] [{lvl}] {_cat}: {f(s, ex)}";

        if (ex != null)
            msg += $"\n  → {ex.Message}";

        if (tb.InvokeRequired)
            tb.Invoke(() => Append(tb, msg));
        else
            Append(tb, msg);
    }

    static void Append(TextBox tb, string msg)
    {
        tb.AppendText(msg + Environment.NewLine);

        if (tb.Lines.Length > 1000)
            tb.Lines = [.. tb.Lines.Skip(500)];

        tb.SelectionStart = tb.TextLength;
        tb.ScrollToCaret();
    }
}