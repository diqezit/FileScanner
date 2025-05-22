namespace FileScanner.Core.Services;

public sealed class OutputFileNameGenerator(IOptions<ScannerConfiguration> configuration)
    : IOutputFileNameGenerator
{
    private readonly ScannerConfiguration _config = configuration.Value;

    public string GenerateFileName(
        string directoryPath,
        string projectRootPath,
        string fileType)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directoryPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(projectRootPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileType);

        var baseName = directoryPath.Equals(
            projectRootPath,
            StringComparison.OrdinalIgnoreCase)
            ? _config.RootFileName
            : Path.GetRelativePath(projectRootPath, directoryPath)
                .Replace(Path.DirectorySeparatorChar, '_')
                .Replace(Path.AltDirectorySeparatorChar, '_')
                .Replace('.', '_');

        return fileType == "other"
            ? baseName
            : $"{baseName}({fileType})";
    }
}