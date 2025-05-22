namespace FileScanner.FileOperations.Services;

public sealed class OutputDirectoryCleaner(
    IOptions<ScannerConfiguration> options,
    ILogger<OutputDirectoryCleaner> logger) : IOutputDirectoryCleaner
{
    private readonly int _maxRetries = options.Value.MaxRetries;
    private readonly int _retryDelayMs = options.Value.RetryDelayMs;

    public async Task CleanAsync(string outputDirectory, CancellationToken cancellationToken)
    {
        ValidateDirectory(outputDirectory);
        outputDirectory = NormalizePath(outputDirectory);

        if (DirectoryDoesNotExist(outputDirectory))
        {
            CreateAndLogDirectory(outputDirectory);
            return;
        }

        var txtFiles = FindTextFilesInDirectory(outputDirectory);
        if (NoFilesToClean(txtFiles)) return;

        LogCleaningOperation(txtFiles.Length);
        await DeleteAllFilesAsync(txtFiles, cancellationToken);
    }

    private static void ValidateDirectory(string outputDirectory) =>
        ArgumentException.ThrowIfNullOrWhiteSpace(outputDirectory);

    private static string NormalizePath(string outputDirectory) =>
        Path.GetFullPath(outputDirectory);

    private static bool DirectoryDoesNotExist(string outputDirectory) =>
        !Directory.Exists(outputDirectory);

    private void CreateAndLogDirectory(string outputDirectory)
    {
        Directory.CreateDirectory(outputDirectory);
        logger.LogInformation("Created output directory: {Directory}", outputDirectory);
    }

    private static string[] FindTextFilesInDirectory(string outputDirectory) =>
        Directory.GetFiles(outputDirectory, "*.txt");

    private static bool NoFilesToClean(string[] files) =>
        files.Length == 0;

    private void LogCleaningOperation(int fileCount) =>
        logger.LogInformation("Cleaning {Count} files from output directory", fileCount);

    private async Task DeleteAllFilesAsync(string[] files, CancellationToken cancellationToken)
    {
        var tasks = files.Select(file => DeleteFileWithRetryAsync(file, cancellationToken));
        await Task.WhenAll(tasks);
    }

    private async Task DeleteFileWithRetryAsync(string file, CancellationToken cancellationToken)
    {
        for (int retry = 0; retry < _maxRetries; retry++)
        {
            try
            {
                DeleteFile(file);
                LogSuccessfulDeletion(file);
                return;
            }
            catch (IOException) when (retry < _maxRetries - 1)
            {
                await DelayBeforeRetryAsync(retry, cancellationToken);
            }
            catch (Exception ex) when (retry == _maxRetries - 1)
            {
                LogDeletionFailure(ex, file);
            }
        }
    }

    private static void DeleteFile(string file) =>
        File.Delete(file);

    private void LogSuccessfulDeletion(string file) =>
        logger.LogDebug("Deleted file: {File}", Path.GetFileName(file));

    private async Task DelayBeforeRetryAsync(int retry, CancellationToken cancellationToken) =>
        await Task.Delay(CalculateRetryDelay(retry), cancellationToken);

    private int CalculateRetryDelay(int retry) =>
        _retryDelayMs * (retry + 1);

    private void LogDeletionFailure(Exception ex, string file) =>
        logger.LogWarning(ex, "Failed to delete file: {File}", file);
}