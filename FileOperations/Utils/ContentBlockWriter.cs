// File: FileOperations/Utils/ContentBlockWriter.cs
namespace FileScanner.FileOperations.Utils;

// Manages visual structure of the final unified file
internal static class ContentBlockWriter
{
    private const string ModuleBorder = "// ████████████████████████████████████████";

    public static async Task WriteMainHeaderAsync(StreamWriter writer, int fileCount)
    {
        await writer.WriteLineAsync(FormattingConstants.Separator);
        await writer.WriteLineAsync("// UNIFIED PROJECT CONTENT");
        await writer.WriteLineAsync($"// Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        await writer.WriteLineAsync($"// Total files: {fileCount}");
        await writer.WriteLineAsync(FormattingConstants.Separator);
        await writer.WriteLineAsync();
    }

    public static async Task WriteModuleHeaderAsync(StreamWriter writer, string moduleName)
    {
        await writer.WriteLineAsync();
        await writer.WriteLineAsync();
        await writer.WriteLineAsync(ModuleBorder);
        await writer.WriteLineAsync($"// ███ MODULE: {moduleName.ToUpper()}");
        await writer.WriteLineAsync(ModuleBorder);
        await writer.WriteLineAsync();
    }

    public static async Task WriteFileBlockAsync(
        StreamWriter writer,
        string filePath,
        CancellationToken token)
    {
        var cleanFileName = FileNameParser.GetCleanFileName(filePath);

        WriteFileSeparator(writer, cleanFileName);
        await AppendFileContentAsync(writer, filePath, token);
    }

    private static void WriteFileSeparator(StreamWriter writer, string fileName)
    {
        writer.WriteLine(FormattingConstants.Separator);
        writer.WriteLine($"// FILE: {fileName}");
        writer.WriteLine(FormattingConstants.Separator);
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