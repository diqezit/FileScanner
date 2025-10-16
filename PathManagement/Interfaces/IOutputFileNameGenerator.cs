// PathManagement/Interfaces/IOutputFileNameGenerator.cs
namespace FileScanner.PathManagement.Interfaces;

public interface IOutputFileNameGenerator
{
    string GenerateFileName(
        DirectoryPath directoryPath,
        DirectoryPath projectRootPath,
        string fileType);
}