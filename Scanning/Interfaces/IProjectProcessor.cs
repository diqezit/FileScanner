// Scanning/Interfaces/IProjectProcessor.cs
namespace FileScanner.Scanning.Interfaces;

public interface IProjectProcessor
{
    Task<bool> ProcessProjectAsync(
        DirectoryPath projectRootDirectory,
        DirectoryPath outputDirectory,
        CancellationToken cancellationToken = default);
}