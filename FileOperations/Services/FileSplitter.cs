// FileOperations/Services/FileSplitter.cs
namespace FileScanner.FileOperations.Services;

public sealed class FileSplitter(ILogger<FileSplitter> logger) : IFileSplitter
{
    public async Task SplitFileAsync(
        FilePath sourceFile,
        long chunkSizeInChars,
        CancellationToken cancellationToken)
    {
        if (!IsSplittingRequired(sourceFile, chunkSizeInChars))
            return;

        LogSplittingStart(sourceFile, chunkSizeInChars);

        await PerformSplitting(
            sourceFile,
            chunkSizeInChars,
            cancellationToken);
    }

    private bool IsSplittingRequired(FilePath sourceFile, long chunkSizeInChars)
    {
        if (!File.Exists(sourceFile.Value))
        {
            logger.LogWarning(
                "Source file for splitting not found: {File}",
                sourceFile.Value);
            return false;
        }

        var sourceFileInfo = new FileInfo(sourceFile.Value);
        if (sourceFileInfo.Length <= chunkSizeInChars)
        {
            logger.LogInformation(
                "File is smaller than chunk size, no splitting needed");
            return false;
        }

        return true;
    }

    private void LogSplittingStart(FilePath sourceFile, long chunkSizeInChars)
    {
        logger.LogInformation(
            "Splitting file {File} into chunks of ~{Size} chars",
            Path.GetFileName(sourceFile.Value),
            chunkSizeInChars);
    }

    private async Task PerformSplitting(
        FilePath sourceFile,
        long chunkSizeInChars,
        CancellationToken cancellationToken)
    {
        int partNumber = 1;
        using var reader = new StreamReader(sourceFile.Value);

        while (!reader.EndOfStream)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var chunkContent = await ReadChunkAsync(reader, chunkSizeInChars);
            if (chunkContent is null)
                continue;

            await WriteChunkToFile(
                sourceFile,
                partNumber++,
                chunkContent,
                cancellationToken);
        }
    }

    private static async Task<string?> ReadChunkAsync(
        StreamReader reader,
        long chunkSizeInChars)
    {
        var chunkBuffer = new char[chunkSizeInChars];
        int charsRead = await reader.ReadBlockAsync(chunkBuffer, 0, chunkBuffer.Length);

        return charsRead > 0 ? new string(chunkBuffer, 0, charsRead) : null;
    }

    private async Task WriteChunkToFile(
        FilePath originalFile,
        int partNumber,
        string chunkContent,
        CancellationToken cancellationToken)
    {
        var chunkFileName = GetChunkFileName(
            originalFile.Value,
            partNumber,
            chunkContent.Length);

        await File.WriteAllTextAsync(chunkFileName, chunkContent, cancellationToken);

        logger.LogInformation(
            "Created chunk: {ChunkFile}",
            Path.GetFileName(chunkFileName));
    }

    private static string GetChunkFileName(
        string originalPath,
        int partNumber,
        int symbolsInChunk)
    {
        var directory = Path.GetDirectoryName(originalPath);
        var fileNameWithoutExt = Path.GetFileNameWithoutExtension(originalPath);
        var extension = Path.GetExtension(originalPath);

        var newFileName =
            $"{fileNameWithoutExt}(PART_{partNumber}-Symbols_{symbolsInChunk}){extension}";

        return Path.Combine(directory!, newFileName);
    }
}