// FileOperations/Interfaces/IOutputDirectoryCleaner.cs
namespace FileScanner.FileOperations.Interfaces;

public interface IOutputDirectoryCleaner
{
    Task CleanAsync(
        DirectoryPath outputDirectory,
        CancellationToken cancellationToken);
}