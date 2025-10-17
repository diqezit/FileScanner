#nullable enable

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
    public ProjectStructure EnumerateProject(
        DirectoryPath projectRoot,
        bool useFilters) =>
        projectEnumerator.EnumerateProject(projectRoot, useFilters);

    public async Task<bool> ProcessProjectFilesAsync(
        ProjectStructure projectStructure,
        DirectoryPath projectRoot,
        DirectoryPath outputDirectory,
        CancellationToken cancellationToken)
    {
        if (!Directory.Exists(projectRoot.Value))
        {
            logger.LogError(
                "Project directory not found: {Project}",
                projectRoot.Value);

            throw new DirectoryNotFoundException(
                $"Project directory not found: {projectRoot.Value}");
        }

        logger.LogInformation(
            "Starting intermediate file processing for: {ProjectRoot}",
            projectRoot.Value);

        try
        {
            await directoryCleaner.CleanAsync(outputDirectory, cancellationToken);

            var processingTasks = projectStructure.FileGroups.Select(group =>
                ProcessDirectoryContentsAsync(
                    group.Key,
                    projectRoot,
                    outputDirectory,
                    group.Value,
                    cancellationToken));

            await Task.WhenAll(processingTasks);
            return true;
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("File processing cancelled");
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred during file processing: {Directory}",
                projectRoot.Value);
            return false;
        }
    }

    private async Task ProcessDirectoryContentsAsync(
        DirectoryPath directoryPath,
        DirectoryPath rootPath,
        DirectoryPath outputDirectory,
        List<FilePath> filesInDirectory,
        CancellationToken cancellationToken)
    {
        var filePathsAsStrings = filesInDirectory.Select(f => f.Value);
        var fileGroupsByType = await fileGrouper.GroupFilesByTypeAsync(
            filePathsAsStrings,
            cancellationToken);

        var writingTasks = fileGroupsByType
            .Where(group => group.Value.Count > 0)
            .Select(group => WriteFileGroupAsync(
                group.Key,
                group.Value,
                directoryPath,
                rootPath,
                outputDirectory,
                cancellationToken));

        await Task.WhenAll(writingTasks);
    }

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
}