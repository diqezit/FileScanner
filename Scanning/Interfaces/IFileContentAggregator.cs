// Scanning/Interfaces/IFileContentAggregator.cs
namespace FileScanner.Scanning.Interfaces;

public interface IFileContentAggregator
{
    Task<List<string>> AggregateFileContentsAsync(
        IEnumerable<FilePath> filePaths,
        DirectoryPath rootPath,
        CancellationToken cancellationToken);
}