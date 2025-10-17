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
        var relativePath = new RelativePath(
            Path.GetRelativePath(rootPath.Value, filePath.Value));

        // Use discard '_' for the unused fileInfo out parameter to fix IDE0059
        if (!TryGetValidFileInfo(filePath, out _, out var validationError))
            return CreateErrorContent(relativePath, validationError);

        var (success, content, readError) = await TryReadContentAsync(
            filePath, cancellationToken);

        if (!success)
            return CreateErrorContent(relativePath, readError!);

        return new FileContent(relativePath, content!, true);
    }

    // Pre-checks file metadata
    // avoids reading invalid or large files
    private bool TryGetValidFileInfo(
        FilePath filePath,
        out FileInfo? fileInfo,
        out string errorMessage)
    {
        fileInfo = null;
        try
        {
            var info = new FileInfo(filePath.Value);

            if (!FileExists(info, out errorMessage)) return false;
            if (!FileSizeIsAllowed(info, out errorMessage)) return false;

            fileInfo = info;
            return true;
        }
        catch (Exception ex)
        {
            // file system access can fail for many reasons
            logger.LogWarning(ex, "Validation failed for file: {File}", filePath.Value);
            errorMessage = $"File system error: {ex.Message}";
            return false;
        }
    }

    // Isolates file system IO
    // handles read errors gracefully
    private async Task<(bool Success, string? Content, string? ErrorMessage)> TryReadContentAsync(
        FilePath filePath,
        CancellationToken cancellationToken)
    {
        try
        {
            var content = await File.ReadAllTextAsync(
                filePath.Value, Encoding.UTF8, cancellationToken);
            return (true, content, null);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning(ex, "Error reading file: {File}", filePath.Value);
            return (false, null, $"Read error: {ex.Message}");
        }
    }

    private static bool FileExists(FileInfo fileInfo, out string errorMessage)
    {
        errorMessage = string.Empty;
        if (!fileInfo.Exists)
        {
            errorMessage = "File not found";
            return false;
        }
        return true;
    }

    // File size limit protects against memory issues
    // and excessively large outputs
    private bool FileSizeIsAllowed(FileInfo fileInfo, out string errorMessage)
    {
        errorMessage = string.Empty;
        if (fileInfo.Length > _maxFileSize)
        {
            errorMessage = $"File is too large ({UIHelper.FormatFileSize(fileInfo.Length)})";
            return false;
        }
        return true;
    }

    private static FileContent CreateErrorContent(RelativePath relativePath, string reason)
    {
        var errorText = $"// ERROR: File skipped. Reason: {reason}";
        return new FileContent(relativePath, errorText, false, reason);
    }
}