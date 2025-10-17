#nullable enable

namespace FileScanner.Analysis.Services;

public sealed class ProjectStatisticsCalculator : IProjectStatisticsCalculator
{
    public Task<ProjectStatistics> CalculateAsync(
        ProjectStructure projectStructure,
        CancellationToken cancellationToken) =>
        Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            long totalSize = 0;
            int fileCount = 0;

            var allFiles = projectStructure.FileGroups.SelectMany(g => g.Value);
            foreach (var file in allFiles)
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    totalSize += new FileInfo(file.Value).Length;
                    fileCount++;
                }
                catch (IOException) { }
            }

            return new ProjectStatistics(
                FileCount: fileCount,
                DirectoryCount: projectStructure.AllDirectories.Count,
                TotalSize: totalSize
            );
        }, cancellationToken);
}