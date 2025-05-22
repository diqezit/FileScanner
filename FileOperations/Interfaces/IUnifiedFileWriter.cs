// FileOperations/Interfaces/IUnifiedFileWriter.cs
namespace FileScanner.FileOperations.Interfaces;

public interface IUnifiedFileWriter
{
    Task WriteUnifiedFileAsync(
        string outputDirectory,
        CancellationToken cancellationToken);
}