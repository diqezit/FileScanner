namespace FileScanner.FileOperations.Services;

public sealed class FileWriter(
    IOptions<ScannerConfiguration> options,
    ILogger<FileWriter> logger) : IFileWriter
{
    private readonly int _bufferSize = options.Value.BufferSize;

    public async Task WriteAsync(
        string outputFilePath,
        IEnumerable<string> contents,
        CancellationToken cancellationToken)
    {
        ValidateInputParameters(outputFilePath, contents);

        try
        {
            EnsureDirectoryExists(outputFilePath);
            await WriteContentsToFileAsync(outputFilePath, contents, cancellationToken);
            LogSuccessfulWrite(outputFilePath);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            LogWriteError(ex, outputFilePath);
            throw;
        }
    }

    private static void ValidateInputParameters(string outputFilePath, IEnumerable<string> contents)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(outputFilePath);
        ArgumentNullException.ThrowIfNull(contents);
    }

    private static void EnsureDirectoryExists(string filePath)
    {
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);
    }

    private async Task WriteContentsToFileAsync(
        string outputFilePath,
        IEnumerable<string> contents,
        CancellationToken cancellationToken)
    {
        await using var stream = CreateFileStream(outputFilePath);
        await using var writer = CreateStreamWriter(stream);

        await WriteLinesToStreamAsync(writer, contents, cancellationToken);
    }

    private FileStream CreateFileStream(string outputFilePath) =>
        new(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.None, _bufferSize, useAsync: true);

    private StreamWriter CreateStreamWriter(FileStream stream) =>
        new(stream, Encoding.UTF8, _bufferSize);

    private static async Task WriteLinesToStreamAsync(
        StreamWriter writer,
        IEnumerable<string> contents,
        CancellationToken cancellationToken)
    {
        foreach (var line in contents)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await writer.WriteLineAsync(line.AsMemory(), cancellationToken);
        }
    }

    private void LogSuccessfulWrite(string outputFilePath) =>
        logger.LogInformation("Created file: {OutputFile}", Path.GetFileName(outputFilePath));

    private void LogWriteError(Exception ex, string outputFilePath) =>
        logger.LogError(ex, "Error writing file: {File}", outputFilePath);
}