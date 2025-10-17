// Scanning/Services/ProjectProcessorService.cs
namespace FileScanner.Scanning.Services;

public sealed class ProjectProcessorService(
    IOutputDirectoryCleaner directoryCleaner,
    IProjectEnumerator projectEnumerator,
    IFileGrouper fileGrouper,
    IFileContentAggregator contentAggregator,
    IFileWriter fileWriter,
    IOutputFileNameGenerator nameGenerator,
    ILogger<ProjectProcessorService> logger) : IProjectProcessor
{
    public async Task<bool> ProcessProjectAsync(
        DirectoryPath projectRootDirectory,
        DirectoryPath outputDirectory,
        CancellationToken cancellationToken = default)
    {
        if (!ArePrerequisitesMet(projectRootDirectory))
            return false;

        logger.LogInformation(
            "Starting project processing for: {ProjectRoot}",
            projectRootDirectory.Value);

        try
        {
            await directoryCleaner.CleanAsync(outputDirectory, cancellationToken);

            var enumerationResult = projectEnumerator.EnumerateProject(projectRootDirectory);

            await ProcessDiscoveredFiles(
                enumerationResult,
                projectRootDirectory,
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
            logger.LogError(
                ex,
                "An unhandled error occurred during directory processing: {Directory}",
                projectRootDirectory.Value);
            return false;
        }
    }

    // Groups all discovered files by directory and processes them in parallel
    private async Task ProcessDiscoveredFiles(
        ProjectEnumerationResult enumerationResult,
        DirectoryPath rootPath,
        DirectoryPath outputDirectory,
        CancellationToken cancellationToken)
    {
        var filesByDirectory = enumerationResult.Files
            .GroupBy(f => new DirectoryPath(Path.GetDirectoryName(f.Value)!));

        var processingTasks = filesByDirectory.Select(directoryGroup =>
            ProcessDirectoryContentsAsync(
                directoryGroup.Key,
                rootPath,
                outputDirectory,
                directoryGroup.ToList(), // Convert group to a list for processing
                cancellationToken));

        await Task.WhenAll(processingTasks);
    }

    // Processes all files belonging to a single directory
    private async Task ProcessDirectoryContentsAsync(
        DirectoryPath directoryPath,
        DirectoryPath rootPath,
        DirectoryPath outputDirectory,
        List<FilePath> filesInDirectory,
        CancellationToken cancellationToken)
    {
        var fileGroups = await GroupFilesByTypeAsync(filesInDirectory, cancellationToken);

        var writingTasks = fileGroups
            .Where(group => group.Value.Any())
            .Select(group => WriteFileGroupAsync(
                group.Key,
                group.Value,
                directoryPath,
                rootPath,
                outputDirectory,
                cancellationToken));

        await Task.WhenAll(writingTasks);
    }

    // Groups a list of files by their classified type
    private async Task<Dictionary<string, List<FilePath>>> GroupFilesByTypeAsync(
        List<FilePath> filesInDirectory,
        CancellationToken cancellationToken)
    {
        var filePathsAsStrings = filesInDirectory.Select(f => f.Value);

        return await fileGrouper.GroupFilesByTypeAsync(
            filePathsAsStrings,
            cancellationToken);
    }

    // Aggregates content for a file group and writes a single output file
    private async Task WriteFileGroupAsync(
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

        if (aggregatedContents.Count == 0)
            return;

        var outputFileName = nameGenerator.GenerateFileName(
            directoryPath,
            rootPath,
            fileType);

        var outputPath = new FilePath(
            Path.Combine(outputDirectory.Value, $"{outputFileName}.txt"));

        await fileWriter.WriteAsync(
            outputPath,
            aggregatedContents,
            cancellationToken);
    }

    // Fails fast if root directory is invalid before starting any processing
    private bool ArePrerequisitesMet(DirectoryPath projectRootDirectory)
    {
        if (Directory.Exists(projectRootDirectory.Value))
            return true;

        logger.LogError(
            "Scan prerequisites not met for project: {Project}",
            projectRootDirectory.Value);

        throw new DirectoryNotFoundException(
            $"Project directory not found: {projectRootDirectory.Value}");
    }
}