// FileOperations/Interfaces/IFileWriter.cs
namespace FileScanner.FileOperations.Interfaces;

public interface IFileWriter
{
    Task WriteAsync(
        FilePath outputFilePath,
        IEnumerable<string> contents,
        CancellationToken cancellationToken);
}