// FileScanner.Core/Interfaces/IFileProcessor.cs
namespace FileScanner.Scanning.Interfaces;

public interface IFileProcessor
{
    Task<bool> ProcessDirectoryAsync(string directoryPath, string outputDirectory, CancellationToken cancellationToken = default);
}