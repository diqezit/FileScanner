// PathManagement/Interfaces/IDefaultPathProvider.cs
namespace FileScanner.PathManagement.Interfaces;

public interface IDefaultPathProvider
{
    DirectoryPath GetDefaultProjectPath();
    DirectoryPath GetDefaultOutputPath(DirectoryPath projectPath);
}