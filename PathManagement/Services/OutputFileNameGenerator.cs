// PathManagement/Services/OutputFileNameGenerator.cs
namespace FileScanner.PathManagement.Services;

public sealed class OutputFileNameGenerator(
    IOptions<ScannerConfiguration> options) : IOutputFileNameGenerator
{
    private readonly string _rootFileName = options.Value.RootFileName;
    private readonly int _maxFileNameLength = options.Value.MaxFileNameLength;

    private const string UnnamedFallback = "unnamed";

    private static readonly HashSet<char> InvalidChars = [..Path.GetInvalidFileNameChars()
        .Concat([Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar])];

    public string GenerateFileName(DirectoryPath directoryPath, DirectoryPath projectRootPath, string fileType)
    {
        var baseName = GetBaseName(directoryPath, projectRootPath);
        var withType = AppendFileType(baseName, fileType);
        return EnsureValidLength(withType);
    }

    private string GetBaseName(DirectoryPath directoryPath, DirectoryPath projectRootPath) =>
        directoryPath.Value.Equals(projectRootPath.Value, StringComparison.OrdinalIgnoreCase)
            ? _rootFileName
            : SanitizePath(Path.GetRelativePath(projectRootPath.Value, directoryPath.Value));

    private static string AppendFileType(string baseName, string fileType) =>
        fileType.Equals(FileTypes.Other, StringComparison.OrdinalIgnoreCase)
            ? baseName
            : $"{baseName}({fileType})";

    private string EnsureValidLength(string fileName) =>
        fileName.Length > _maxFileNameLength
            ? TruncateWithHash(fileName)
            : fileName;

    private static string SanitizePath(string path)
    {
        var sanitized = string.Concat(path
            .Where(c => !InvalidChars.Contains(c))
            .Select(c => c == ' ' ? '_' : c));
        return string.IsNullOrWhiteSpace(sanitized) ? UnnamedFallback : sanitized;
    }

    private string TruncateWithHash(string fileName)
    {
        var effectiveMaxLength = _maxFileNameLength - 9;
        if (effectiveMaxLength <= 0)
            return $"{Math.Abs(fileName.GetHashCode()):X8}";

        var truncated = fileName[..Math.Min(fileName.Length, effectiveMaxLength)];
        return $"{truncated}_{Math.Abs(fileName.GetHashCode()):X8}";
    }
}