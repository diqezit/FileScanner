// Configuration/Services/SettingsManager.cs
namespace FileScanner.Configuration.Services;

public sealed class SettingsManager(
    IUserSettingsService userSettingsService,
    IDefaultPathProvider defaultPathProvider,
    ILogger logger)
{
    // Ensures a valid settings object is always returned
    public UserSettings Load() =>
        TryLoadPersistentSettings() ?? GetDefaultSettings();

    // Persist current state, a failure is non-critical
    public void Save(string projectPath, string outputPath)
    {
        try
        {
            var settings = new UserSettings
            {
                LastProjectPath = projectPath,
                LastOutputPath = outputPath,
                LastUsed = DateTime.Now
            };
            userSettingsService.Save(settings);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to save user settings");
        }
    }

    private UserSettings? TryLoadPersistentSettings()
    {
        try
        {
            var settings = userSettingsService.Load();
            // Stored path must be validated to prevent errors on startup
            if (IsValidPath(settings.LastProjectPath))
                return settings;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to load settings, will use defaults");
        }

        return null;
    }

    // Provides a good first-run experience or a fallback on error
    private UserSettings GetDefaultSettings()
    {
        var projectPath = defaultPathProvider.GetDefaultProjectPath();
        var outputPath = defaultPathProvider.GetDefaultOutputPath(projectPath);
        return new UserSettings
        {
            LastProjectPath = projectPath.Value,
            LastOutputPath = outputPath.Value
        };
    }

    private static bool IsValidPath(string? path) =>
        !string.IsNullOrEmpty(path) && Directory.Exists(path);
}