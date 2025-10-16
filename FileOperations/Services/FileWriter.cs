// FileOperations/Services/FileWriter.cs
namespace FileScanner.FileOperations.Services;

public sealed class FileWriter(
    IOptions<ScannerConfiguration> options,
    ILogger<FileWriter> logger) : IFileWriter
{
    private readonly int _bufferSize = options.Value.BufferSize;

    public async Task WriteAsync(
        FilePath outputFilePath,
        IEnumerable<string> contents,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(contents);

        try
        {
            await WriteContentsToFileAsync(outputFilePath, contents, cancellationToken);
            logger.LogInformation("Created file: {OutputFile}", Path.GetFileName(outputFilePath.Value));
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Error writing file: {File}", outputFilePath.Value);
            throw;
        }
    }

    private async Task WriteContentsToFileAsync(
        FilePath filePath,
        IEnumerable<string> contents,
        CancellationToken cancellationToken)
    {
        EnsureDirectoryExistsForFile(filePath.Value);

        await using var stream = CreateFileStream(filePath.Value);
        await using var writer = new StreamWriter(stream, Encoding.UTF8, _bufferSize);

        foreach (var line in contents)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await writer.WriteLineAsync(line.AsMemory(), cancellationToken);
        }
    }

    private static void EnsureDirectoryExistsForFile(string filePath)
    {
        var directory = Path.GetDirectoryName(filePath);
        if (directory is not null)
            Directory.CreateDirectory(directory);
    }

    private FileStream CreateFileStream(string filePath) =>
        new(filePath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            _bufferSize,
            useAsync: true);
}