// FileOperations/Interfaces/IUnifiedFileWriter.cs
namespace FileScanner.FileOperations.Interfaces;

public interface IUnifiedFileWriter
{
    Task WriteUnifiedFileAsync(
        DirectoryPath projectRoot,
        DirectoryPath outputDirectory,
        CancellationToken cancellationToken);
}