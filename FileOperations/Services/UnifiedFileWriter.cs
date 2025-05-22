namespace FileScanner.FileOperations.Services;

public sealed class UnifiedFileWriter(
    ILogger<UnifiedFileWriter> logger) : IUnifiedFileWriter
{
    private const string UnifiedFileName = "_United_All_Files.txt";
    private const string FilePattern = "*.txt";

    public async Task WriteUnifiedFileAsync(
        string outputDirectory,
        CancellationToken cancellationToken)
    {
        try
        {
            var files = GetFilesToUnite(outputDirectory);

            if (!HasFilesToProcess(files))
            {
                LogNoFilesFound();
                return;
            }

            var outputPath = GetUnifiedFilePath(outputDirectory);
            await WriteUnifiedContentAsync(
                outputPath,
                files,
                cancellationToken);

            LogSuccess();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw;
        }
    }

    private string[] GetFilesToUnite(string directory)
    {
        return [.. Directory
            .GetFiles(directory, FilePattern)
            .Where(IsNotUnifiedFile)
            .OrderBy(f => f)];
    }

    private bool IsNotUnifiedFile(string filePath)
    {
        var fileName = Path.GetFileName(filePath);
        return !fileName.Equals(
            UnifiedFileName,
            StringComparison.OrdinalIgnoreCase);
    }

    private static bool HasFilesToProcess(string[] files) =>
        files.Length > 0;

    private static string GetUnifiedFilePath(string directory) =>
        Path.Combine(directory, UnifiedFileName);

    private static async Task WriteUnifiedContentAsync(
        string outputPath,
        string[] files,
        CancellationToken cancellationToken)
    {
        await using var writer = CreateWriter(outputPath);

        await WriteHeaderAsync(writer, files.Length);
        await WriteFilesContentAsync(
            writer,
            files,
            cancellationToken);
    }

    private static StreamWriter CreateWriter(string path) =>
        new(path, false, Encoding.UTF8);

    private static async Task WriteHeaderAsync(
        StreamWriter writer,
        int fileCount)
    {
        await writer.WriteLineAsync("// " + new string('=', 40));
        await writer.WriteLineAsync("// UNIFIED PROJECT CONTENT");
        await writer.WriteLineAsync($"// Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        await writer.WriteLineAsync($"// Total files: {fileCount}");
        await writer.WriteLineAsync("// " + new string('=', 40));
        await writer.WriteLineAsync();
    }

    private static async Task WriteFilesContentAsync(
        StreamWriter writer,
        string[] files,
        CancellationToken cancellationToken)
    {
        foreach (var file in files)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await WriteFileContentAsync(writer, file, cancellationToken);
        }
    }

    private static async Task WriteFileContentAsync(
        StreamWriter writer,
        string filePath,
        CancellationToken cancellationToken)
    {
        await WriteFileSeparatorAsync(writer, filePath);
        await WriteFileBodyAsync(writer, filePath, cancellationToken);
        await WriteEndingNewLinesAsync(writer);
    }

    private static async Task WriteFileSeparatorAsync(
        StreamWriter writer,
        string filePath)
    {
        var fileName = Path.GetFileNameWithoutExtension(filePath);

        await writer.WriteLineAsync("// " + new string('=', 40));
        await writer.WriteLineAsync($"// FILE: {fileName}");
        await writer.WriteLineAsync("// " + new string('=', 40));
        await writer.WriteLineAsync();
    }

    private static async Task WriteFileBodyAsync(
        StreamWriter writer,
        string filePath,
        CancellationToken cancellationToken)
    {
        var content = await File.ReadAllTextAsync(
            filePath,
            cancellationToken);

        await writer.WriteLineAsync(content);
    }

    private static async Task WriteEndingNewLinesAsync(StreamWriter writer)
    {
        await writer.WriteLineAsync();
        await writer.WriteLineAsync();
    }

    private void LogNoFilesFound() =>
        logger.LogWarning("No files found to unite");

    private void LogSuccess() =>
        logger.LogInformation(
            "Created unified file: {FileName}",
            UnifiedFileName);

    private void LogError(Exception ex) =>
        logger.LogError(ex, "Error creating unified file");
}