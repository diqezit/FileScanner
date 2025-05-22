namespace FileScanner.Core.Services;

public sealed class ProjectPathResolver(ILogger<ProjectPathResolver> logger)
    : IProjectPathResolver
{
    public string? FindProjectRoot(string startPath, string projectFileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(startPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(projectFileName);

        var currentDir = startPath;

        while (!string.IsNullOrEmpty(currentDir) &&
               !currentDir.Equals(
                   Path.GetPathRoot(currentDir),
                   StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                if (Directory.GetFiles(currentDir, projectFileName).Length > 0)
                {
                    logger.LogInformation(
                        "Найден корень проекта: {ProjectRoot}",
                        currentDir);
                    return currentDir;
                }

                currentDir = Directory.GetParent(currentDir)?.FullName;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex,
                    "Ошибка при поиске корня проекта в {Directory}",
                    currentDir);
                break;
            }
        }

        logger.LogWarning(
            "Корень проекта не найден, начиная поиск с {StartPath}",
            startPath);
        return null;
    }
}