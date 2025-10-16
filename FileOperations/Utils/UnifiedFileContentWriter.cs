// FileOperations/Utils/UnifiedFileContentWriter.cs
namespace FileScanner.FileOperations.Utils;

// Handles the presentation logic of writing content to a unified file stream
internal static class UnifiedFileContentWriter
{
    private const string Separator = "// ========================================";

    public static async Task WriteAsync(
        string outputPath,
        string[] files,
        string metadataHeader,
        CancellationToken cancellationToken)
    {
        await using var writer = new StreamWriter(outputPath, false, Encoding.UTF8);

        await WriteMainHeaderAsync(writer, files.Length);
        await writer.WriteLineAsync(metadataHeader);

        foreach (var file in files)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await WriteFileBlockAsync(writer, file, cancellationToken);
        }
    }

    private static async Task WriteMainHeaderAsync(StreamWriter writer, int fileCount)
    {
        await writer.WriteLineAsync(Separator);
        await writer.WriteLineAsync("// UNIFIED PROJECT CONTENT");
        await writer.WriteLineAsync($"// Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        await writer.WriteLineAsync($"// Total files: {fileCount}");
        await writer.WriteLineAsync(Separator);
        await writer.WriteLineAsync();
    }

    private static async Task WriteFileBlockAsync(
        StreamWriter writer,
        string filePath,
        CancellationToken token)
    {
        WriteFileSeparator(writer, Path.GetFileName(filePath));
        await AppendFileContentAsync(writer, filePath, token);
    }

    private static void WriteFileSeparator(StreamWriter writer, string fileName)
    {
        writer.WriteLine(Separator);
        writer.WriteLine($"// FILE: {fileName}");
        writer.WriteLine(Separator);
        writer.WriteLine();
    }

    private static async Task AppendFileContentAsync(
        StreamWriter writer,
        string filePath,
        CancellationToken cancellationToken)
    {
        var content = await File.ReadAllTextAsync(filePath, cancellationToken);
        await writer.WriteLineAsync(content);
        await writer.WriteLineAsync();
        await writer.WriteLineAsync();
    }
}