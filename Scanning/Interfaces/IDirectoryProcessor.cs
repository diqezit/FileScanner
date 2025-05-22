// FileScanner.Core/Interfaces/IDirectoryProcessor.cs
namespace FileScanner.Scanning.Interfaces;

public interface IDirectoryProcessor
{
    Task ProcessAsync(string directoryPath, string rootPath, string outputDirectory, CancellationToken cancellationToken);
}