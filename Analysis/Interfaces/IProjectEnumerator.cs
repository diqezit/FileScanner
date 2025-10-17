// File: Analysis/Interfaces/IProjectEnumerator.cs
namespace FileScanner.Analysis.Interfaces;

// Represents the discovered project structure, either physical or logical
public record ProjectStructure(
    // Groups files by their directory path (physical or logical)
    IReadOnlyDictionary<DirectoryPath, List<FilePath>> FileGroups,
    // Provides all unique directory paths for tree generation
    IReadOnlyList<DirectoryPath> AllDirectories
);

public interface IProjectEnumerator
{
    // Enumerates a project and returns its complete structure
    // useFilters flag determines the enumeration strategy
    ProjectStructure EnumerateProject(
        DirectoryPath projectRoot,
        bool useFilters);
}