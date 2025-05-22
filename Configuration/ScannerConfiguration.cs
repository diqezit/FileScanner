namespace FileScanner.Configuration;

public sealed record ScannerConfiguration
{
    public required string[] IgnoredExtensions { get; init; }
    public required string[] IgnoredDirectories { get; init; }
    public required string DefaultProjectName { get; init; }
    public required string DefaultOutputFolderName { get; init; }
    public required string RootFileName { get; init; }

    public static ScannerConfiguration Default => new()
    {
        IgnoredExtensions =
        [
            ".csproj.user", ".suo", ".user", ".exe", ".dll", ".pdb",
            ".config", ".binlog", ".vspscc", ".designer.cs", ".ico",
            ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".db", ".sqlite",
            ".bak", ".tmp", ".log", ".json", ".xml", ".zip", ".rar",
            ".7z", ".txt", ".md", ".resources"
        ],
        IgnoredDirectories =
        [
            "bin", "obj", ".git", ".vs", "node_modules",
            "packages", "wwwroot", "dist"
        ],
        DefaultProjectName = "SpectrumNet.csproj",
        DefaultOutputFolderName = "GeneratedProjectContent",
        RootFileName = "Project_Root"
    };
}