// Scanning/Interfaces/IFileScanner.cs
namespace FileScanner.Scanning.Interfaces;

public interface IFileScanner
{
    Task<bool> ScanDirectoryAsync(
        DirectoryPath projectRootDirectory,
        DirectoryPath outputDirectory,
        CancellationToken cancellationToken = default);
}