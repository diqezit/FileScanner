// PathManagement/Interfaces/IProjectPathResolver.cs
namespace FileScanner.PathManagement.Interfaces;

public interface IProjectPathResolver
{
    string? FindProjectRoot(string startPath, string projectFileName);
}