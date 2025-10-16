// Analysis/Interfaces/IProjectStatisticsCalculator.cs
using FileScanner.Analysis.Models;

namespace FileScanner.UI.Interfaces;

public interface IProjectStatisticsCalculator
{
    Task<ProjectStatistics> CalculateAsync(
        DirectoryPath projectPath,
        CancellationToken cancellationToken);
}