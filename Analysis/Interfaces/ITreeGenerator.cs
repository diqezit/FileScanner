// File: Analysis/Interfaces/ITreeGenerator.cs
namespace FileScanner.Analysis.Interfaces;

public interface ITreeGenerator
{
    // Generates a directory tree from an abstract project structure
    // This supports both physical and logical (filter-based) layouts
    string GenerateDirectoryTree(
        ProjectStructure projectStructure,
        DirectoryPath rootPath);
}