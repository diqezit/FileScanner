#nullable enable

namespace FileScanner.FileOperations.Services;

public sealed class UnifiedFileWriter(
    ILogger<UnifiedFileWriter> logger) : IUnifiedFileWriter
{
    private const string UnifiedFileName = "_United_All_Files.txt";

    public async Task WriteUnifiedFileAsync(
        string metadataHeader,
        DirectoryPath outputDirectory,
        CancellationToken cancellationToken)
    {
        try
        {
            var filesToUnite = SourceFileProvider.GetFilesToUnite(
                outputDirectory.Value);

            if (filesToUnite.Length == 0)
            {
                logger.LogWarning("No source files found to unite");
                return;
            }

            var outputPath = Path.Combine(
                outputDirectory.Value,
                UnifiedFileName);

            await UnifiedFileContentWriter.WriteAsync(
                outputPath,
                filesToUnite,
                metadataHeader,
                cancellationToken);

            logger.LogInformation(
                "Created unified file: {FileName}",
                UnifiedFileName);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Error creating unified file");
            throw;
        }
    }
}