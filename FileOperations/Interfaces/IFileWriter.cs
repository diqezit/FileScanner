// FileScanner.Core/Interfaces/IFileWriter.cs
namespace FileScanner.FileOperations.Interfaces;

public interface IFileWriter
{
    Task WriteAsync(string outputFilePath, IEnumerable<string> contents, CancellationToken cancellationToken);
}