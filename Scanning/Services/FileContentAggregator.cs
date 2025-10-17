// File: Scanning/Services/FileContentAggregator.cs
namespace FileScanner.Scanning.Services;

// Aggregates file content with controlled parallelism
// Prevents overwhelming file system with too many open handles
public sealed class FileContentAggregator(
    IFileReader fileReader,
    IOptions<ScannerConfiguration> options) : IFileContentAggregator, IDisposable
{
    // Limit concurrent reads to avoid system overload
    // Fallback to processor count if not configured
    private readonly SemaphoreSlim _semaphore = new(
        options.Value.MaxParallelism > 0
            ? options.Value.MaxParallelism
            : Environment.ProcessorCount * 2);

    public async Task<List<string>> AggregateFileContentsAsync(
        IEnumerable<FilePath> filePaths,
        DirectoryPath rootPath,
        CancellationToken cancellationToken)
    {
        var readTasks = filePaths.Select(
            path => ReadAndFormatFileAsync(path, rootPath, cancellationToken));

        var formattedBlocks = await Task.WhenAll(readTasks);

        return [.. formattedBlocks.SelectMany(block => block)];
    }

    // Reads a single file within a semaphore-controlled slot
    private async Task<List<string>> ReadAndFormatFileAsync(
        FilePath filePath,
        DirectoryPath rootPath,
        CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            var fileContent = await fileReader.ReadFileAsync(
                filePath, rootPath, cancellationToken);

            return SourceBlockFormatter.Format(fileContent);
        }
        finally
        {
            // always release semaphore slot even if read fails
            _semaphore.Release();
        }
    }

    public void Dispose() => _semaphore.Dispose();
}