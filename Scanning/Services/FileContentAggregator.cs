// Scanning/Services/FileContentAggregator.cs
namespace FileScanner.Scanning.Services;

public sealed class FileContentAggregator(
    IFileReader fileReader,
    IOptions<ScannerConfiguration> options) : IFileContentAggregator, IDisposable
{
    private readonly SemaphoreSlim _semaphore = new(
        options.Value.MaxParallelism > 0
            ? options.Value.MaxParallelism
            : Environment.ProcessorCount * 2);

    public async Task<List<string>> AggregateFileContentsAsync(
        IEnumerable<FilePath> filePaths,
        DirectoryPath rootPath,
        CancellationToken cancellationToken)
    {
        var fileTasks = filePaths
            .Select(path => ReadAndFormatFileAsync(path, rootPath, cancellationToken))
            .ToList();

        var formattedContents = await Task.WhenAll(fileTasks);

        return [.. formattedContents.SelectMany(content => content)];
    }

    private async Task<List<string>> ReadAndFormatFileAsync(
        FilePath filePath,
        DirectoryPath rootPath,
        CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            var fileContent = await fileReader.ReadFileAsync(filePath, rootPath, cancellationToken);
            return FormatContentBlock(fileContent);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private static List<string> FormatContentBlock(FileContent fileContent) =>
    [
        $"// {fileContent.RelativePath.Value}",
        fileContent.Content,
        string.Empty
    ];

    public void Dispose() => _semaphore.Dispose();
}