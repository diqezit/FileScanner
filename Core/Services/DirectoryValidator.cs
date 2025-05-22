namespace FileScanner.Core.Services;

public sealed class DirectoryValidator(IOptions<ScannerConfiguration> configuration)
    : IDirectoryValidator
{
    private readonly HashSet<string> _ignoredDirectories =
        new(configuration.Value.IgnoredDirectories, StringComparer.OrdinalIgnoreCase);

    private readonly HashSet<string> _ignoredExtensions =
        new(configuration.Value.IgnoredExtensions, StringComparer.OrdinalIgnoreCase);

    public bool ShouldIgnoreDirectory(string directoryName) =>
        _ignoredDirectories.Contains(directoryName);

    public bool ShouldIgnoreFile(string filePath) =>
        _ignoredExtensions.Contains(Path.GetExtension(filePath));
}