// FileOperations/Services/OutputDirectoryCleaner.cs
namespace FileScanner.FileOperations.Services;

public sealed class OutputDirectoryCleaner(
    IOptions<ScannerConfiguration> options,
    ILogger<OutputDirectoryCleaner> logger) : IOutputDirectoryCleaner
{
    private readonly int _maxRetries = options.Value.MaxRetries;
    private readonly int _retryDelayMs = options.Value.RetryDelayMs;

    public async Task CleanAsync(DirectoryPath outputDirectory, CancellationToken cancellationToken)
    {
        EnsureOutputDirectoryExists(outputDirectory.Value);

        var filesToClean = Directory.GetFiles(outputDirectory.Value, "*.txt");
        if (filesToClean.Length == 0) return;

        LogCleaningOperation(filesToClean.Length);
        await DeleteFilesAsync(filesToClean, cancellationToken);
    }

    private void EnsureOutputDirectoryExists(string outputDirectory)
    {
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
            logger.LogInformation("Created output directory: {Directory}", outputDirectory);
        }
    }

    private async Task DeleteFilesAsync(string[] files, CancellationToken cancellationToken)
    {
        var deleteTasks = files.Select(file => DeleteFileWithRetriesAsync(file, cancellationToken));
        await Task.WhenAll(deleteTasks);
    }

    private async Task DeleteFileWithRetriesAsync(string filePath, CancellationToken cancellationToken)
    {
        for (int i = 0; i < _maxRetries; i++)
        {
            try
            {
                File.Delete(filePath);
                logger.LogDebug("Deleted file: {File}", Path.GetFileName(filePath));
                return;
            }
            catch (IOException ex)
            {
                if (i == _maxRetries - 1)
                {
                    LogDeletionFailure(ex, filePath);
                    throw;
                }
                await Task.Delay(_retryDelayMs * (i + 1), cancellationToken);
            }
        }
    }

    private void LogCleaningOperation(int fileCount) =>
        logger.LogInformation("Cleaning {Count} files from output directory", fileCount);

    private void LogDeletionFailure(Exception ex, string filePath) =>
        logger.LogWarning(ex, "Failed to delete file: {File}", filePath);
}