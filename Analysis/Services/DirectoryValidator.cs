// Analysis/Services/DirectoryValidator.cs
namespace FileScanner.Analysis.Services;

public sealed class DirectoryValidator(IOptions<ScannerConfiguration> options)
    : IDirectoryValidator
{
    private readonly HashSet<string> _ignoredDirectories = new(
        options.Value.IgnoredDirectories,
        StringComparer.OrdinalIgnoreCase);

    private readonly HashSet<string> _ignoredExtensions = new(
        options.Value.IgnoredExtensions,
        StringComparer.OrdinalIgnoreCase);

    public bool ShouldIgnoreDirectory(string directoryPath)
    {
        if (IsInvalidPath(directoryPath)) return true;
        var directoryName = Path.GetFileName(directoryPath);
        if (IsInvalidPath(directoryName)) return true;
        return IsNameInIgnoredList(directoryName) || IsHidden(directoryName);
    }

    public bool ShouldIgnoreFile(FilePath filePath)
    {
        if (IsInvalidPath(filePath.Value)) return true;
        var fileName = Path.GetFileName(filePath.Value);
        if (IsInvalidPath(fileName)) return true;
        return IsHidden(fileName) || HasIgnoredExtension(filePath);
    }

    private static bool IsInvalidPath(string? path) => string.IsNullOrWhiteSpace(path);

    private bool IsNameInIgnoredList(string directoryName) =>
        _ignoredDirectories.Contains(directoryName.Trim());

    private bool HasIgnoredExtension(FilePath filePath)
    {
        var extension = Path.GetExtension(filePath.Value);
        return !string.IsNullOrEmpty(extension) && _ignoredExtensions.Contains(extension);
    }

    private static bool IsHidden(string name) => name.StartsWith('.') && name.Length > 1;
}