// Analysis/Services/ProjectStatisticsCalculator.cs
namespace FileScanner.Analysis.Services;

public sealed class ProjectStatisticsCalculator(
    IDirectoryValidator validator) : IProjectStatisticsCalculator
{
    public Task<ProjectStatistics> CalculateAsync(
        DirectoryPath projectPath,
        CancellationToken cancellationToken) =>
        Task.Run(() =>
        {
            var validFiles = GetValidFileInfos(projectPath.Value, validator);
            var validDirs = GetValidDirectoryPaths(projectPath.Value, validator);

            cancellationToken.ThrowIfCancellationRequested();

            return new ProjectStatistics(
                FileCount: validFiles.Count,
                DirectoryCount: validDirs.Count,
                TotalSize: validFiles.Sum(file => file.Length)
            );
        }, cancellationToken);

    private static List<FileInfo> GetValidFileInfos(string path, IDirectoryValidator validator) =>
        [..Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories)
            .Where(file => !validator.ShouldIgnoreFile(new FilePath(file)))
            .Select(file => new FileInfo(file))];

    private static List<string> GetValidDirectoryPaths(string path, IDirectoryValidator validator) =>
        [..Directory.EnumerateDirectories(path, "*", SearchOption.AllDirectories)
            .Where(dir => !validator.ShouldIgnoreDirectory(Path.GetFileName(dir)))];
}