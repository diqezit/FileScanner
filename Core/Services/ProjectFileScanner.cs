namespace FileScanner.Core.Services;

public sealed class ProjectFileScanner(
    IFileProcessor fileProcessor,
    ILogger<ProjectFileScanner> logger) : IFileScanner
{
    public async Task ScanAndGenerateAsync(
        string projectRootDirectory,
        string outputDirectory,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectRootDirectory);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputDirectory);

        if (!Directory.Exists(projectRootDirectory))
        {
            throw new DirectoryNotFoundException(
                $"Проектная директория не найдена: {projectRootDirectory}");
        }

        logger.LogInformation(
            "Начинаем сканирование проекта: {ProjectRoot}",
            projectRootDirectory);

        await PrepareOutputDirectoryAsync(outputDirectory, cancellationToken);
        await fileProcessor.ProcessDirectoryAsync(
            projectRootDirectory,
            outputDirectory,
            cancellationToken);

        logger.LogInformation(
            "Сканирование завершено. Файлы сохранены в: {OutputDirectory}",
            outputDirectory);
    }

    private static async Task PrepareOutputDirectoryAsync(
        string outputDirectory,
        CancellationToken cancellationToken)
    {
        if (!Directory.Exists(outputDirectory))
            Directory.CreateDirectory(outputDirectory);
        else
        {
            var txtFiles = Directory.GetFiles(outputDirectory, "*.txt");
            var deleteTasks = txtFiles.Select(async file =>
            {
                try
                {
                    await Task.Run(() => File.Delete(file), cancellationToken);
                }
                catch (Exception) { }
            });

            await Task.WhenAll(deleteTasks);
        }
    }
}