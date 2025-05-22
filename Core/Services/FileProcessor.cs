namespace FileScanner.Core.Services;

public sealed class FileProcessor(
    IDirectoryValidator validator,
    IOutputFileNameGenerator fileNameGenerator,
    IFileTypeClassifier fileTypeClassifier,
    ILogger<FileProcessor> logger) : IFileProcessor
{
    public async Task<bool> ProcessDirectoryAsync(
        string directoryPath,
        string outputDirectory,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directoryPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputDirectory);

        if (!Directory.Exists(directoryPath))
        {
            logger.LogWarning(
                "Директория не существует: {DirectoryPath}",
                directoryPath);
            return false;
        }

        return await ProcessDirectoryRecursiveAsync(
            directoryPath,
            outputDirectory,
            directoryPath,
            cancellationToken);
    }

    private async Task<bool> ProcessDirectoryRecursiveAsync(
        string currentPath,
        string outputDirectory,
        string rootPath,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var directoryInfo = new DirectoryInfo(currentPath);

        if (validator.ShouldIgnoreDirectory(directoryInfo.Name))
        {
            logger.LogDebug("Игнорируем директорию: {Directory}",
                Path.GetRelativePath(rootPath, currentPath));
            return true;
        }

        var filesByType = new Dictionary<string, List<string>>();

        foreach (var file in Directory.GetFiles(currentPath))
        {
            if (validator.ShouldIgnoreFile(file))
            {
                logger.LogDebug("Игнорируем файл: {File}",
                    Path.GetRelativePath(rootPath, file));
                continue;
            }

            var fileType = fileTypeClassifier.ClassifyFile(file);

            if (!filesByType.ContainsKey(fileType))
                filesByType[fileType] = new List<string>();

            try
            {
                var relativePath = Path.GetRelativePath(rootPath, file)
                    .Replace('\\', '/');
                var content = await File.ReadAllTextAsync(file, cancellationToken);

                filesByType[fileType].Add($"// {relativePath}");
                filesByType[fileType].Add(content);
                filesByType[fileType].Add("");
            }
            catch (Exception ex)
            {
                var relativePath = Path.GetRelativePath(rootPath, file);
                var errorMessage =
                    $"// Ошибка чтения файла {relativePath}: {ex.Message}";
                filesByType[fileType].Add(errorMessage);
                logger.LogWarning(ex,
                    "Ошибка при чтении файла: {File}", file);
            }
        }

        foreach (var (fileType, contents) in filesByType
            .Where(kvp => kvp.Value.Count > 0))
        {
            await SaveOutputFileAsync(
                currentPath,
                rootPath,
                outputDirectory,
                fileType,
                contents,
                cancellationToken);
        }

        foreach (var subdirectory in Directory.GetDirectories(currentPath))
        {
            await ProcessDirectoryRecursiveAsync(
                subdirectory,
                outputDirectory,
                rootPath,
                cancellationToken);
        }

        return true;
    }

    private async Task SaveOutputFileAsync(
        string currentPath,
        string rootPath,
        string outputDirectory,
        string fileType,
        List<string> fileContents,
        CancellationToken cancellationToken)
    {
        var outputFileName = fileNameGenerator.GenerateFileName(
            currentPath,
            rootPath,
            fileType);
        var outputFilePath = Path.Combine(outputDirectory, $"{outputFileName}.txt");

        var outputDir = Path.GetDirectoryName(outputFilePath);
        if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
            Directory.CreateDirectory(outputDir);

        await File.WriteAllLinesAsync(
            outputFilePath,
            fileContents,
            Encoding.UTF8,
            cancellationToken);

        logger.LogInformation(
            "Создан файл: {OutputFile}",
            Path.GetFileName(outputFilePath));
    }
}