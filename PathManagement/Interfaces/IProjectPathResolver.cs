// PathManagement/Interfaces/IProjectPathResolver.cs
namespace FileScanner.PathManagement.Interfaces;

public interface IProjectPathResolver
{
    DirectoryPath? FindProjectRoot(string startPath, string projectFileName);
}