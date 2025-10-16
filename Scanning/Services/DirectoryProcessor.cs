// Scanning/Services/DirectoryProcessor.cs
namespace FileScanner.Scanning.Services;

public sealed class DirectoryProcessor(
    IDirectoryValidator validator,
    IFileGrouper fileGrouper,
    IFileContentAggregator contentAggregator,
    IFileWriter fileWriter,
    IOutputFileNameGenerator nameGenerator,
    ILogger<DirectoryProcessor> logger) : IDirectoryProcessor
{
    public async Task ProcessAsync(
        DirectoryPath directoryPath,
        DirectoryPath rootPath,
        DirectoryPath outputDirectory,
        CancellationToken cancellationToken)
    {
        if (validator.ShouldIgnoreDirectory(Path.GetFileName(directoryPath.Value)))
            return;

        await Task.WhenAll(
            ProcessFilesInDirectoryAsync(directoryPath, rootPath, outputDirectory, cancellationToken),
            ProcessSubdirectoriesAsync(directoryPath, rootPath, outputDirectory, cancellationToken)
        );
    }

    private async Task ProcessFilesInDirectoryAsync(
        DirectoryPath directoryPath,
        DirectoryPath rootPath,
        DirectoryPath outputDirectory,
        CancellationToken cancellationToken)
    {
        var filesInDirectory = Directory.EnumerateFiles(directoryPath.Value);
        var fileGroups = await fileGrouper.GroupFilesByTypeAsync(filesInDirectory, cancellationToken);

        var processingTasks = fileGroups
            .Where(group => group.Value.Any())
            .Select(group => ProcessAndWriteFileGroupAsync(
                group.Key,
                group.Value,
                directoryPath,
                rootPath,
                outputDirectory,
                cancellationToken));

        await Task.WhenAll(processingTasks);
    }

    private async Task ProcessAndWriteFileGroupAsync(
        string fileType,
        IEnumerable<FilePath> filePaths,
        DirectoryPath directoryPath,
        DirectoryPath rootPath,
        DirectoryPath outputDirectory,
        CancellationToken cancellationToken)
    {
        var aggregatedContents = await contentAggregator.AggregateFileContentsAsync(
            filePaths,
            rootPath,
            cancellationToken);

        if (aggregatedContents.Count == 0) return;

        var outputFileName = nameGenerator.GenerateFileName(directoryPath, rootPath, fileType);
        var outputPath = new FilePath(Path.Combine(outputDirectory.Value, $"{outputFileName}.txt"));

        await fileWriter.WriteAsync(outputPath, aggregatedContents, cancellationToken);
    }

    private async Task ProcessSubdirectoriesAsync(
        DirectoryPath directoryPath,
        DirectoryPath rootPath,
        DirectoryPath outputDirectory,
        CancellationToken cancellationToken)
    {
        try
        {
            var subdirectories = Directory.EnumerateDirectories(directoryPath.Value);
            var tasks = subdirectories.Select(subDir =>
                ProcessAsync(new DirectoryPath(subDir), rootPath, outputDirectory, cancellationToken));
            await Task.WhenAll(tasks);
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogWarning(ex, "Access denied to subdirectories of {Directory}", directoryPath.Value);
        }
    }
}