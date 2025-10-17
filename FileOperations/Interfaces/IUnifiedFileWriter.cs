// File: FileOperations/Interfaces/IUnifiedFileWriter.cs
namespace FileScanner.FileOperations.Interfaces;

public interface IUnifiedFileWriter
{
    // Writes the final unified file using a pre-generated metadata header
    // The writer's job is to assemble, not to generate content
    Task WriteUnifiedFileAsync(
        string metadataHeader,
        DirectoryPath outputDirectory,
        CancellationToken cancellationToken);
}