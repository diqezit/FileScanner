// Analysis/Interfaces/IDirectoryValidator.cs
namespace FileScanner.Analysis.Interfaces;

public interface IDirectoryValidator
{
    bool ShouldIgnoreDirectory(string directoryName);
    bool ShouldIgnoreFile(FilePath filePath);
}