// Configuration/Services/UserSettingsService.cs
namespace FileScanner.Configuration.Services;

public sealed class UserSettingsService(
    ISettingsPathProvider pathProvider,
    ILogger<UserSettingsService> logger) : IUserSettingsService
{
    // Path is determined by an external provider, not hardcoded here
    // This allows changing storage location without altering this service
    private readonly string _settingsPath = pathProvider.GetSettingsFilePath();
    private readonly ILogger<UserSettingsService> _logger = logger;
    private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    public UserSettings Load()
    {
        try
        {
            if (!File.Exists(_settingsPath))
                return new UserSettings();

            var json = File.ReadAllText(_settingsPath);
            return DeserializeSettings(json);
        }
        catch (Exception ex)
        {
            LogLoadError(ex);
            // Return empty settings on failure so the app can start
            return new UserSettings();
        }
    }

    public void Save(UserSettings settings)
    {
        try
        {
            var json = SerializeSettings(settings);
            File.WriteAllText(_settingsPath, json);
        }
        catch (Exception ex)
        {
            // Saving settings is non-critical, log a warning but don't crash
            LogSaveError(ex);
        }
    }

    private static UserSettings DeserializeSettings(string json) =>
        JsonSerializer.Deserialize<UserSettings>(json) ?? new UserSettings();

    private static string SerializeSettings(UserSettings settings) =>
        JsonSerializer.Serialize(settings, _jsonOptions);

    private void LogLoadError(Exception ex) =>
        _logger.LogWarning(ex, "Failed to load user settings");

    private void LogSaveError(Exception ex) =>
        _logger.LogWarning(ex, "Failed to save user settings");
}