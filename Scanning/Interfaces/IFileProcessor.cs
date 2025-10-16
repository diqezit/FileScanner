// Scanning/Interfaces/IFileProcessor.cs
namespace FileScanner.Scanning.Interfaces;

public interface IFileProcessor
{
    Task<bool> ProcessDirectoryAsync(
        DirectoryPath directoryPath,
        DirectoryPath outputDirectory,
        CancellationToken cancellationToken = default);
}