namespace FileScanner.Scanning.Services;

public sealed class DirectoryProcessor(
    IDirectoryValidator validator,
    IFileGrouper fileGrouper,
    IFileReader fileReader,
    IFileWriter fileWriter,
    IOutputFileNameGenerator nameGenerator,
    IOptions<ScannerConfiguration> options,
    ILogger<DirectoryProcessor> logger) : IDirectoryProcessor, IDisposable
{
    private readonly SemaphoreSlim _semaphore = new(
        options.Value.MaxParallelism > 0
            ? options.Value.MaxParallelism
            : Environment.ProcessorCount * 2);

    public async Task ProcessAsync(
        string directoryPath,
        string rootPath,
        string outputDirectory,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directoryPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(rootPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputDirectory);

        var directoryName = Path.GetFileName(directoryPath);
        if (validator.ShouldIgnoreDirectory(directoryName))
            return;

        await Task.WhenAll(
            ProcessFilesAsync(directoryPath, rootPath, outputDirectory, cancellationToken),
            ProcessSubdirectoriesAsync(directoryPath, rootPath, outputDirectory, cancellationToken)
        );
    }

    private async Task ProcessFilesAsync(
        string directoryPath,
        string rootPath,
        string outputDirectory,
        CancellationToken cancellationToken)
    {
        var fileGroups = await fileGrouper.GroupFilesByTypeAsync(directoryPath, cancellationToken);

        var tasks = fileGroups
            .Where(g => g.Value.Count > 0)
            .Select(g => ProcessFileGroupAsync(g.Key, g.Value, directoryPath, rootPath, outputDirectory, cancellationToken));

        await Task.WhenAll(tasks);
    }

    private async Task ProcessFileGroupAsync(
        string fileType,
        List<string> files,
        string directoryPath,
        string rootPath,
        string outputDirectory,
        CancellationToken cancellationToken)
    {
        var contents = await ReadFilesAsync(files, rootPath, cancellationToken);
        if (contents.Count == 0) return;

        var outputFileName = nameGenerator.GenerateFileName(directoryPath, rootPath, fileType);
        var outputPath = Path.Combine(outputDirectory, $"{outputFileName}.txt");

        await fileWriter.WriteAsync(outputPath, contents, cancellationToken);
    }

    private async Task<List<string>> ReadFilesAsync(
        List<string> files,
        string rootPath,
        CancellationToken cancellationToken)
    {
        var fileContents = new (int Index, FileContent Content)[files.Count];

        var tasks = files.Select(async (file, index) =>
        {
            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                var content = await fileReader.ReadFileAsync(file, rootPath, cancellationToken);
                fileContents[index] = (index, content);
            }
            finally
            {
                _semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);

        var result = new List<string>(fileContents.Length * 3);
        foreach (var (_, content) in fileContents)
        {
            result.Add($"// {content.RelativePath}");
            result.Add(content.Content);
            result.Add(string.Empty);
        }

        return result;
    }

    private async Task ProcessSubdirectoriesAsync(
        string directoryPath,
        string rootPath,
        string outputDirectory,
        CancellationToken cancellationToken)
    {
        try
        {
            var subdirectories = Directory.EnumerateDirectories(directoryPath).ToArray();
            var tasks = subdirectories.Select(subdirectory =>
                ProcessAsync(subdirectory, rootPath, outputDirectory, cancellationToken));

            await Task.WhenAll(tasks);
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogWarning(ex, "Access denied to subdirectories: {Directory}", directoryPath);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Error processing subdirectories: {Directory}", directoryPath);
        }
    }

    public void Dispose() => _semaphore?.Dispose();
}