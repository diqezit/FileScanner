// File: Scanning/Interfaces/IProjectProcessor.cs
namespace FileScanner.Scanning.Interfaces;

public interface IProjectProcessor
{
    // Selects a strategy and enumerates a project's files
    ProjectStructure EnumerateProject(
        DirectoryPath projectRoot,
        bool useFilters);

    // Processes the enumerated files to create intermediate .txt files
    Task<bool> ProcessProjectFilesAsync(
        ProjectStructure projectStructure,
        DirectoryPath projectRoot,
        DirectoryPath outputDirectory,
        CancellationToken cancellationToken);
}