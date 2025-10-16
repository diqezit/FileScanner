// FileOperations/Services/FileReader.cs
namespace FileScanner.FileOperations.Services;

public sealed class FileReader(
    IOptions<ScannerConfiguration> options,
    ILogger<FileReader> logger) : IFileReader
{
    private readonly int _maxFileSize = options.Value.MaxFileSize;

    public async Task<FileContent> ReadFileAsync(
        FilePath filePath,
        DirectoryPath rootPath,
        CancellationToken cancellationToken)
    {
        var relativePath = new RelativePath(Path.GetRelativePath(rootPath.Value, filePath.Value));

        if (!IsFileValidForReading(filePath, out var validationError))
            return CreateErrorContent(relativePath, validationError);

        var (success, content, readError) = await TryReadContentAsync(filePath, cancellationToken);
        if (!success)
            return CreateErrorContent(relativePath, readError!);

        return new FileContent(relativePath, content!, true);
    }

    private bool IsFileValidForReading(FilePath filePath, out string errorMessage)
    {
        var fileInfo = new FileInfo(filePath.Value);
        if (!CheckFileExists(fileInfo, out errorMessage)) return false;
        if (!CheckFileSize(fileInfo, out errorMessage)) return false;
        return true;
    }

    private async Task<(bool Success, string? Content, string? ErrorMessage)> TryReadContentAsync(
        FilePath filePath,
        CancellationToken cancellationToken)
    {
        try
        {
            var content = await File.ReadAllTextAsync(filePath.Value, Encoding.UTF8, cancellationToken);
            return (true, content, null);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning(ex, "Error reading file: {File}", filePath.Value);
            return (false, null, $"Read error: {ex.Message}");
        }
    }

    private static bool CheckFileExists(FileInfo fileInfo, out string errorMessage)
    {
        errorMessage = string.Empty;
        if (!fileInfo.Exists)
        {
            errorMessage = "File not found";
            return false;
        }
        return true;
    }

    private bool CheckFileSize(FileInfo fileInfo, out string errorMessage)
    {
        errorMessage = string.Empty;
        if (fileInfo.Length > _maxFileSize)
        {
            errorMessage = $"File too large: {fileInfo.Length:N0} bytes";
            return false;
        }
        return true;
    }

    private static FileContent CreateErrorContent(RelativePath relativePath, string error) =>
        new(relativePath, $"// {error}", false);
}