namespace FileScanner.PathManagement.Services;

public sealed class OutputFileNameGenerator(IOptions<ScannerConfiguration> options) : IOutputFileNameGenerator
{
    private readonly string _rootFileName = options.Value.RootFileName;
    private readonly int _maxFileNameLength = options.Value.MaxFileNameLength;
    private static readonly HashSet<char> InvalidChars = [.. Path.GetInvalidFileNameChars()
            .Concat([Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar])];

    public string GenerateFileName(string directoryPath, string projectRootPath, string fileType)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directoryPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(projectRootPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileType);

        var baseName = directoryPath.Equals(projectRootPath, StringComparison.OrdinalIgnoreCase)
            ? _rootFileName
            : SanitizeFileName(Path.GetRelativePath(projectRootPath, directoryPath));

        var fileName = fileType == "other" ? baseName : $"{baseName}({fileType})";

        return fileName.Length > _maxFileNameLength - 4
            ? TruncateFileName(fileName)
            : fileName;
    }

    private static string SanitizeFileName(string path)
    {
        var sanitized = string.Concat(path
            .Where(c => !InvalidChars.Contains(c))
            .Select(c => c == ' ' ? '_' : c));

        return string.IsNullOrWhiteSpace(sanitized) ? "unnamed" : sanitized;
    }

    private string TruncateFileName(string fileName)
    {
        var maxLength = _maxFileNameLength - 12;
        var truncated = fileName[..Math.Min(fileName.Length, maxLength)];
        return $"{truncated}_{Math.Abs(fileName.GetHashCode()):X8}";
    }
}