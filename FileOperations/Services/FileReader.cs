namespace FileScanner.FileOperations.Services;

public sealed class FileReader(
    IOptions<ScannerConfiguration> options,
    ILogger<FileReader> logger) : IFileReader
{
    private readonly int _maxFileSize = options.Value.MaxFileSize;

    public async Task<FileContent> ReadFileAsync(
        string filePath,
        string rootPath,
        CancellationToken cancellationToken)
    {
        ValidatePaths(filePath, rootPath);

        try
        {
            var fileInfo = new FileInfo(filePath);

            if (!fileInfo.Exists)
                return CreateErrorContent(filePath, rootPath, "File not found");

            if (fileInfo.Length > _maxFileSize)
                return CreateErrorContent(filePath, rootPath, $"File too large: {fileInfo.Length:N0} bytes");

            var relativePath = Path.GetRelativePath(rootPath, filePath).Replace('\\', '/');
            var content = await File.ReadAllTextAsync(filePath, Encoding.UTF8, cancellationToken);

            return new FileContent(relativePath, content, true);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning(ex, "Error reading file: {File}", filePath);
            return CreateErrorContent(filePath, rootPath, $"Read error: {ex.Message}");
        }
    }

    private static void ValidatePaths(string filePath, string rootPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(rootPath);
    }

    private static FileContent CreateErrorContent(string filePath, string rootPath, string error) =>
        new(Path.GetRelativePath(rootPath, filePath), $"// {error}", false);
}