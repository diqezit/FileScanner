// Scanning/Services/ProjectFileScanner.cs
namespace FileScanner.Scanning.Services;

public sealed class ProjectFileScanner(
    IFileProcessor fileProcessor,
    IOutputDirectoryCleaner directoryCleaner,
    IUnifiedFileWriter unifiedFileWriter,
    IOptions<ScannerConfiguration> options,
    ILogger<ProjectFileScanner> logger) : IFileScanner
{
    private readonly string _defaultOutputFolderName =
        options.Value.DefaultOutputFolderName;

    public async Task ScanAndGenerateAsync(
        string projectRootDirectory,
        string outputDirectory,
        CancellationToken cancellationToken = default)
    {
        ValidateParameters(projectRootDirectory, outputDirectory);
        ValidateProjectDirectory(projectRootDirectory);

        LogScanStart(projectRootDirectory);

        await CleanOutputDirectoryAsync(
            outputDirectory,
            cancellationToken);

        var success = await ProcessDirectoryAsync(
            projectRootDirectory,
            outputDirectory,
            cancellationToken);

        if (success)
        {
            await CreateUnifiedFileAsync(
                outputDirectory,
                cancellationToken);

            LogScanComplete(outputDirectory);
        }
    }

    private static void ValidateParameters(
        string projectRootDirectory,
        string outputDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectRootDirectory);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputDirectory);
    }

    private static void ValidateProjectDirectory(string directory)
    {
        if (!Directory.Exists(directory))
            throw new DirectoryNotFoundException(
                $"Project directory not found: {directory}");
    }

    private void LogScanStart(string projectRoot) =>
        logger.LogInformation(
            "Starting project scan: {ProjectRoot}",
            projectRoot);

    private async Task CleanOutputDirectoryAsync(
        string outputDirectory,
        CancellationToken cancellationToken)
    {
        await directoryCleaner.CleanAsync(
            outputDirectory,
            cancellationToken);
    }

    private async Task<bool> ProcessDirectoryAsync(
        string projectRootDirectory,
        string outputDirectory,
        CancellationToken cancellationToken)
    {
        return await fileProcessor.ProcessDirectoryAsync(
            projectRootDirectory,
            outputDirectory,
            cancellationToken);
    }

    private async Task CreateUnifiedFileAsync(
        string outputDirectory,
        CancellationToken cancellationToken)
    {
        await unifiedFileWriter.WriteUnifiedFileAsync(
            outputDirectory,
            cancellationToken);
    }

    private void LogScanComplete(string outputDirectory) =>
        logger.LogInformation(
            "Scan completed. Files saved to: {OutputDirectory}",
            outputDirectory);
}