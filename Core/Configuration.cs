namespace FileScanner.Core;

#region Path Types

public readonly struct DirPath(string v)
{
    public string V { get; } = Path.GetFullPath(
        v ?? throw new ArgumentNullException(nameof(v)));

    public override string ToString() => V;
}

public readonly struct FilePath(string v)
{
    public string V { get; } = Path.GetFullPath(
        v ?? throw new ArgumentNullException(nameof(v)));

    public override string ToString() => V;
}

#endregion

#region Configuration

public sealed record ScannerConfiguration
{
    public HashSet<string> IgnoredExt { get; init; } = [];
    public HashSet<string> IgnoredDirs { get; init; } = [];
    public Dictionary<string, string> TypeMap { get; init; } = [];
    public int MaxFileSize { get; init; } = 100 * 1024 * 1024;
    public int MaxTreeDepth { get; init; } = 50;
    public int BufferSize { get; init; } = 65536;
    public int DefaultChunk { get; init; } = 100000;

    public static ScannerConfiguration Default => new()
    {
        IgnoredExt = new(StringComparer.OrdinalIgnoreCase)
        {
            ".exe", ".dll", ".pdb", ".obj", ".o", ".a", ".lib", ".so", ".dylib",
            ".cache", ".suo", ".user", ".DS_Store",
            ".ico", ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".svg", ".webp",
            ".mp3", ".mp4", ".wav", ".avi", ".mov",
            ".ttf", ".otf", ".woff", ".woff2",
            ".pdf", ".doc", ".docx", ".xls", ".xlsx",
            ".db", ".sqlite", ".mdb",
            ".zip", ".rar", ".7z", ".tar", ".gz",
            ".bak", ".tmp", ".log",
            ".nupkg", ".snupkg", ".pfx", ".snk", ".cer",
            ".vsidx", ".db-shm", ".db-wal"
        },
        IgnoredDirs = new(StringComparer.OrdinalIgnoreCase)
        {
            "bin", "obj", "out", "build", "dist", "target", "x64",
            ".git", ".svn", ".hg", ".idea", ".vs", ".vscode",
            "node_modules", "packages", "vendor",
            "venv", ".venv", "__pycache__",
            "TestResults", "GeneratedProjectContent", "coverage"
        },
        TypeMap = new(StringComparer.OrdinalIgnoreCase)
        {
            [".cs"] = "csharp",
            [".csproj"] = "project",
            [".sln"] = "solution",
            [".fs"] = "fsharp",
            [".vb"] = "vbnet",
            [".xaml"] = "xaml",
            [".razor"] = "razor",
            [".cshtml"] = "razor",
            [".h"] = "cpp_h",
            [".hpp"] = "cpp_h",
            [".cpp"] = "cpp",
            [".cc"] = "cpp",
            [".c"] = "c",
            [".go"] = "go",
            [".rs"] = "rust",
            [".swift"] = "swift",
            [".java"] = "java",
            [".kt"] = "kotlin",
            [".scala"] = "scala",
            [".js"] = "js",
            [".ts"] = "ts",
            [".tsx"] = "ts",
            [".html"] = "html",
            [".css"] = "css",
            [".scss"] = "sass",
            [".vue"] = "vue",
            [".svelte"] = "svelte",
            [".py"] = "python",
            [".rb"] = "ruby",
            [".php"] = "php",
            [".sh"] = "shell",
            [".ps1"] = "ps",
            [".bat"] = "batch",
            [".json"] = "json",
            [".xml"] = "xml",
            [".yml"] = "yaml",
            [".yaml"] = "yaml",
            [".toml"] = "toml",
            [".ini"] = "ini",
            [".sql"] = "sql",
            [".md"] = "md",
            [".txt"] = "text",
            ["Dockerfile"] = "docker",
            [".tf"] = "tf"
        }
    };
}

#endregion

#region Settings

public sealed class UserSettings
{
    public string? ProjectPath { get; set; }
    public string? OutputPath { get; set; }
    public DateTime LastUsed { get; set; }
}

public sealed class SettingsManager
{
    static readonly JsonSerializerOptions Opts = new() { WriteIndented = true };

    readonly string _path = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "settings.json");

    public UserSettings Load()
    {
        try
        {
            if (!File.Exists(_path))
                return Default();

            var s = JsonSerializer.Deserialize<UserSettings>(
                File.ReadAllText(_path));

            return s?.ProjectPath != null && Directory.Exists(s.ProjectPath)
                ? s
                : Default();
        }
        catch { return Default(); }
    }

    public void Save(string proj, string output)
    {
        try
        {
            var s = new UserSettings
            {
                ProjectPath = proj,
                OutputPath = output,
                LastUsed = DateTime.Now
            };
            File.WriteAllText(_path, JsonSerializer.Serialize(s, Opts));
        }
        catch { }
    }

    static UserSettings Default() => new()
    {
        ProjectPath = AppDomain.CurrentDomain.BaseDirectory,
        OutputPath = AppDomain.CurrentDomain.BaseDirectory
    };
}

#endregion

#region Models

public record ScanOptions(
    bool UseFilters,
    bool Split,
    int ChunkSize);

public record ProjectStats(
    int Files,
    int Dirs,
    long Size);

public record ScanResult(
    bool Ok,
    string? OutDir = null,
    string? Tree = null,
    ProjectStats? Stats = null,
    string? Error = null);

#endregion