// Scanning/Interfaces/IFileGrouper.cs
namespace FileScanner.Scanning.Interfaces;

public interface IFileGrouper
{
    Task<Dictionary<string, List<FilePath>>> GroupFilesByTypeAsync(
        IEnumerable<string> filePaths,
        CancellationToken cancellationToken);
}