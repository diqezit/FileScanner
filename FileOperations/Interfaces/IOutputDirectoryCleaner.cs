// FileScanner.Core/Interfaces/IOutputDirectoryCleaner.cs
namespace FileScanner.FileOperations.Interfaces;

public interface IOutputDirectoryCleaner
{
    Task CleanAsync(string outputDirectory, CancellationToken cancellationToken);
}