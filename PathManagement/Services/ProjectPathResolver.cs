// PathManagement/Services/ProjectPathResolver.cs
namespace FileScanner.PathManagement.Services;

public sealed class ProjectPathResolver(
    ILogger<ProjectPathResolver> logger) : IProjectPathResolver
{
    public DirectoryPath? FindProjectRoot(string startPath, string projectFileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(startPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(projectFileName);

        var foundPath = SearchUpwardsFrom(new DirectoryInfo(startPath), projectFileName);
        return foundPath is not null ? new DirectoryPath(foundPath) : null;
    }

    private string? SearchUpwardsFrom(DirectoryInfo? startDirectory, string projectFileName)
    {
        var current = startDirectory;
        var searchPattern = GetSearchPattern(projectFileName);

        while (current is not null)
        {
            if (DirectoryContainsProject(current, searchPattern))
            {
                logger.LogInformation("Found project root: {ProjectRoot}", current.FullName);
                return current.FullName;
            }
            current = current.Parent;
        }
        return null;
    }

    private static bool DirectoryContainsProject(DirectoryInfo directory, string searchPattern)
    {
        try
        {
            return directory.GetFiles(searchPattern, SearchOption.TopDirectoryOnly).Length > 0;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
    }

    private static string GetSearchPattern(string projectFileName) =>
        projectFileName.Contains('*') || projectFileName.Contains('?')
            ? projectFileName
            : $"*{projectFileName}*";
}