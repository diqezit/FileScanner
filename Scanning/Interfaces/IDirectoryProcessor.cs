// Scanning/Interfaces/IDirectoryProcessor.cs
namespace FileScanner.Scanning.Interfaces;

public interface IDirectoryProcessor
{
    Task ProcessAsync(
        DirectoryPath directoryPath,
        DirectoryPath rootPath,
        DirectoryPath outputDirectory,
        CancellationToken cancellationToken);
}