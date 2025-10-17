// File: FileOperations/Utils/UnifiedFileContentWriter.cs
namespace FileScanner.FileOperations.Utils;

// Orchestrates assembly of the final unified text file
internal static class UnifiedFileContentWriter
{
    // Assembles the final file in a specific order:
    // main header, metadata, then content grouped by module
    public static async Task WriteAsync(
        string outputPath,
        string[] files,
        string metadataHeader,
        CancellationToken cancellationToken)
    {
        await using var writer = new StreamWriter(outputPath, false, Encoding.UTF8);

        await ContentBlockWriter.WriteMainHeaderAsync(writer, files.Length);

        await writer.WriteLineAsync(metadataHeader);

        string? currentModule = null;

        foreach (var file in files)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var moduleName = FileNameParser.ExtractModuleName(file);

            if (moduleName != currentModule)
            {
                // detect module change to inject a new module header
                await ContentBlockWriter.WriteModuleHeaderAsync(writer, moduleName);
                currentModule = moduleName;
            }

            await ContentBlockWriter.WriteFileBlockAsync(writer, file, cancellationToken);
        }
    }
}