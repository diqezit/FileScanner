namespace FileScanner.Configuration;

public sealed record ScannerConfiguration
{
    public string[]? AllowedExtensions { get; init; }
    public required string[] IgnoredExtensions { get; init; }
    public required string[] IgnoredDirectories { get; init; }
    public required string DefaultProjectName { get; init; }
    public required string DefaultOutputFolderName { get; init; }
    public required string RootFileName { get; init; }
    public int MaxFileSize { get; init; } = 100 * 1024 * 1024;
    public int MaxPathLength { get; init; } = 260;
    public int MaxFileNameLength { get; init; } = 255;
    public int MaxTreeDepth { get; init; } = 50;
    public int BufferSize { get; init; } = 65536;
    public int MaxRetries { get; init; } = 3;
    public int RetryDelayMs { get; init; } = 100;
    public int MaxParallelism { get; init; } = 0;

    public static ScannerConfiguration Default => new()
    {
        AllowedExtensions = null,

        IgnoredExtensions =
        [
            ".exe", ".dll", ".pdb", ".obj", ".o", ".a", ".lib", ".so", ".dylib",
            ".cache", ".binlog", ".buildlog", ".tlog", ".suo", ".user",
            ".designer.cs", ".generated.cs", ".g.cs", ".g.i.cs", ".xaml.g.cs",
            ".ico", ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".svg", ".webp",
            ".mp3", ".mp4", ".wav", ".avi", ".mov", ".mkv",
            ".db", ".sqlite", ".zip", ".rar", ".7z", ".tar", ".gz", ".bz2", ".xz",
            ".bak", ".tmp", ".temp", ".log", ".swp", "~",
            ".nupkg", ".snupkg", ".nuspec",
            ".pfx", ".snk", ".cer", ".crt", ".key", ".pem"
        ],

        IgnoredDirectories =
        [
            "bin", "obj", "out", "build", "dist", "target", "x64",
            ".git", ".svn", ".hg",
            ".vs", ".vscode", ".idea", ".rider",
            "node_modules", "packages",
            "TestResults", "$RECYCLE.BIN", "System Volume Information", "tmp", "temp"
        ],

        DefaultProjectName = "*.csproj",
        DefaultOutputFolderName = "GeneratedProjectContent",
        RootFileName = "Project_Root"
    };

    public Dictionary<string, string> FileTypeMapping { get; init; } = new
        (StringComparer.OrdinalIgnoreCase)
    {
        [".h"] = "cplusplus_header",
        [".hpp"] = "cplusplus_header",
        [".cpp"] = "cplusplus_source",
        [".c"] = "c_source",
        [".rc"] = "resource",
        [".cs"] = "csharp",
        [".xaml"] = "xaml",
        [".csproj"] = "project",
        [".vcxproj"] = "project",
        [".sln"] = "solution",
        [".razor"] = "razor",
        [".cshtml"] = "razor",
        [".js"] = "javascript",
        [".ts"] = "typescript",
        [".html"] = "markup",
        [".css"] = "styles",
        [".json"] = "json",
        [".xml"] = "xml",
        [".config"] = "config",
        [".sql"] = "sql",
        [".txt"] = "text",
        [".md"] = "markdown",
        ["CMakeLists.txt"] = "cmake",
    };
}