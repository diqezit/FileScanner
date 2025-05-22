// FileScanner.Core/Services/FileProcessor.cs
namespace FileScanner.Scanning.Services;

public sealed class FileProcessor(
    IDirectoryProcessor directoryProcessor,
    ILogger<FileProcessor> logger) : IFileProcessor
{
    public async Task<bool> ProcessDirectoryAsync(
        string directoryPath,
        string outputDirectory,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directoryPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputDirectory);

        try
        {
            directoryPath = Path.GetFullPath(directoryPath);
            outputDirectory = Path.GetFullPath(outputDirectory);

            if (!Directory.Exists(directoryPath))
            {
                logger.LogError("Directory not found: {DirectoryPath}", directoryPath);
                return false;
            }

            await directoryProcessor.ProcessAsync(directoryPath, directoryPath, outputDirectory, cancellationToken);
            return true;
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Directory processing cancelled");
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing directory: {Directory}", directoryPath);
            return false;
        }
    }
}