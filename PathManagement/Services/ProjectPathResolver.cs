namespace FileScanner.PathManagement.Services;

public sealed class ProjectPathResolver(
    ILogger<ProjectPathResolver> logger) : IProjectPathResolver
{

    public string? FindProjectRoot(string startPath, string projectFileName)
    {
        ValidateInputParameters(startPath, projectFileName);

        var current = GetStartDirectory(startPath);
        var searchPattern = CreateSearchPattern(projectFileName);

        return SearchForProject(current, searchPattern);
    }

    private static void ValidateInputParameters(string startPath, string projectFileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(startPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(projectFileName);
    }

    private static DirectoryInfo GetStartDirectory(string startPath) =>
        new(startPath);

    private static string CreateSearchPattern(string projectFileName) =>
        projectFileName.Contains('*') || projectFileName.Contains('?')
            ? projectFileName
            : $"*{projectFileName}*";

    private string? SearchForProject(DirectoryInfo? current, string searchPattern)
    {
        while (current != null)
        {
            try
            {
                if (ProjectFoundInDirectory(current, searchPattern))
                    return LogAndReturnProjectRoot(current);

                current = current.Parent;
            }
            catch (Exception ex)
            {
                LogSearchError(ex, current);
                break;
            }
        }

        return null;
    }

    private static bool ProjectFoundInDirectory(DirectoryInfo directory, string searchPattern) =>
        directory.GetFiles(searchPattern, SearchOption.TopDirectoryOnly).Length > 0;

    private string LogAndReturnProjectRoot(DirectoryInfo directory)
    {
        var fullPath = directory.FullName;
        logger.LogInformation("Found project root: {ProjectRoot}", fullPath);
        return fullPath;
    }

    private void LogSearchError(Exception ex, DirectoryInfo? directory) =>
        logger.LogWarning(ex, "Error searching directory {Directory}", directory?.FullName);
}