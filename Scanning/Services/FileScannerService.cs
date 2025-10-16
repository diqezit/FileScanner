// Scanning/Services/FileScannerService.cs
namespace FileScanner.Scanning.Services;

public sealed class FileScannerService(
    IFileProcessor fileProcessor,
    IOutputDirectoryCleaner directoryCleaner,
    ILogger<FileScannerService> logger) : IFileScanner
{
    public async Task<bool> ScanDirectoryAsync(
        DirectoryPath projectRootDirectory,
        DirectoryPath outputDirectory,
        CancellationToken cancellationToken = default)
    {
        if (!ArePrerequisitesMet(projectRootDirectory))
            return false;

        logger.LogInformation(
            "Starting directory scan for: {ProjectRoot}",
            projectRootDirectory.Value);

        await directoryCleaner.CleanAsync(outputDirectory, cancellationToken);

        return await fileProcessor.ProcessDirectoryAsync(
            projectRootDirectory,
            outputDirectory,
            cancellationToken);
    }

    private bool ArePrerequisitesMet(DirectoryPath projectRootDirectory)
    {
        try
        {
            if (!Directory.Exists(projectRootDirectory.Value))
                throw new DirectoryNotFoundException(
                    $"Project directory not found: {projectRootDirectory.Value}");

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Scan prerequisites not met for project: {Project}",
                projectRootDirectory.Value);

            return false;
        }
    }
}