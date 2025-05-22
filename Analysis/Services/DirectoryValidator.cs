namespace FileScanner.Analysis.Services;

public sealed class DirectoryValidator(IOptions<ScannerConfiguration> options) : IDirectoryValidator
{
    private readonly HashSet<string> _ignoredDirectories = new(
        options.Value.IgnoredDirectories,
        StringComparer.OrdinalIgnoreCase);

    private readonly HashSet<string> _ignoredExtensions = new(
        options.Value.IgnoredExtensions,
        StringComparer.OrdinalIgnoreCase);

    public bool ShouldIgnoreDirectory(string directoryName)
    {
        if (string.IsNullOrWhiteSpace(directoryName)) return true;
        directoryName = directoryName.Trim();
        return _ignoredDirectories.Contains(directoryName) || IsHiddenItem(directoryName);
    }

    public bool ShouldIgnoreFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath)) return true;

        var fileName = Path.GetFileName(filePath);
        if (string.IsNullOrWhiteSpace(fileName)) return true;

        if (IsHiddenItem(fileName)) return true;

        var extension = Path.GetExtension(filePath);
        return !string.IsNullOrWhiteSpace(extension) && _ignoredExtensions.Contains(extension);
    }

    private static bool IsHiddenItem(string name) => name.StartsWith('.') && name.Length > 1;
}