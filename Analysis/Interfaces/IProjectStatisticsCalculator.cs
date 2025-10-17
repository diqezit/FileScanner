// File: Analysis/Interfaces/IProjectStatisticsCalculator.cs
namespace FileScanner.Analysis.Interfaces;

public interface IProjectStatisticsCalculator
{
    // Calculates stats based on a pre-discovered project structure
    // This decouples calculation from file system traversal
    Task<ProjectStatistics> CalculateAsync(
        ProjectStructure projectStructure,
        CancellationToken cancellationToken);
}