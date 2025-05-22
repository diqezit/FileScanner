// FileScanner.Core/Interfaces/IOutputFileNameGenerator.cs
namespace FileScanner.PathManagement.Interfaces;

public interface IOutputFileNameGenerator
{
    string GenerateFileName(string directoryPath, string projectRootPath, string fileType);
}