// FileScanner.Core/Interfaces/IFileScanner.cs
namespace FileScanner.Scanning.Interfaces;

public interface IFileScanner
{
    Task ScanAndGenerateAsync(string projectRootDirectory, string outputDirectory, CancellationToken cancellationToken = default);
}