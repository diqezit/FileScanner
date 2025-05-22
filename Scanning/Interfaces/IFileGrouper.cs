// FileScanner.Core/Interfaces/IFileGrouper.cs
namespace FileScanner.Scanning.Interfaces;

public interface IFileGrouper
{
    Task<Dictionary<string, List<string>>> GroupFilesByTypeAsync(string directoryPath, CancellationToken cancellationToken);
}