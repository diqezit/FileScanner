// FileOperations/Services/UnifiedFileWriter.cs
namespace FileScanner.FileOperations.Services;

public sealed class UnifiedFileWriter(
    ITreeGenerator treeGenerator,
    IProjectStatisticsCalculator statsCalculator,
    ILogger<UnifiedFileWriter> logger) : IUnifiedFileWriter
{
    private const string UnifiedFileName = "_United_All_Files.txt";

    public async Task WriteUnifiedFileAsync(
        DirectoryPath projectRoot,
        DirectoryPath outputDirectory,
        CancellationToken cancellationToken)
    {
        try
        {
            await TryUnifyFilesAsync(projectRoot, outputDirectory, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Error creating unified file");
            throw;
        }
    }

    private async Task TryUnifyFilesAsync(
        DirectoryPath projectRoot,
        DirectoryPath outputDirectory,
        CancellationToken cancellationToken)
    {
        var filesToUnite = SourceFileProvider.GetFilesToUnite(outputDirectory.Value);
        if (filesToUnite.Length == 0)
        {
            logger.LogWarning("No source files found to unite");
            return;
        }

        await WriteUnifiedFileWithMetadataAsync(projectRoot, outputDirectory, filesToUnite, cancellationToken);
    }

    private async Task WriteUnifiedFileWithMetadataAsync(
        DirectoryPath projectRoot,
        DirectoryPath outputDirectory,
        string[] filesToUnite,
        CancellationToken cancellationToken)
    {
        var metadataHeader = await BuildMetadataHeaderAsync(projectRoot, cancellationToken);
        var outputPath = Path.Combine(outputDirectory.Value, UnifiedFileName);

        await UnifiedFileContentWriter.WriteAsync(
            outputPath,
            filesToUnite,
            metadataHeader,
            cancellationToken);

        logger.LogInformation("Created unified file: {FileName}", UnifiedFileName);
    }

    private async Task<string> BuildMetadataHeaderAsync(
        DirectoryPath projectRoot,
        CancellationToken cancellationToken)
    {
        var tree = treeGenerator.GenerateDirectoryTree(projectRoot);
        var stats = await statsCalculator.CalculateAsync(projectRoot, cancellationToken);

        return MetadataHeaderFormatter.Format(tree, stats);
    }
}