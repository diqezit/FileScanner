// Scanning/Services/FileProcessor.cs
namespace FileScanner.Scanning.Services;

public sealed class FileProcessor(
    IDirectoryProcessor directoryProcessor,
    ILogger<FileProcessor> logger) : IFileProcessor
{
    public async Task<bool> ProcessDirectoryAsync(
        DirectoryPath directoryPath,
        DirectoryPath outputDirectory,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await directoryProcessor.ProcessAsync(
                directoryPath,
                directoryPath,
                outputDirectory,
                cancellationToken);
            return true;
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Directory processing cancelled");
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled error occurred during directory processing: {Directory}", directoryPath.Value);
            return false;
        }
    }
}