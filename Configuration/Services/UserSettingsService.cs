namespace FileScanner.Configuration.Services;

public sealed class UserSettingsService(ILogger<UserSettingsService> logger) : IUserSettingsService
{
    private readonly string _settingsPath = GetSettingsPath();
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
            return new UserSettings();
        }
    }

    public void Save(UserSettings settings)
    {
        try
        {
            EnsureDirectoryExists();
            var json = SerializeSettings(settings);
            File.WriteAllText(_settingsPath, json);
        }
        catch (Exception ex)
        {
            LogSaveError(ex);
        }
    }

    private static string GetSettingsPath()
    {
        var appDataPath = Environment.GetFolderPath(
            Environment.SpecialFolder.ApplicationData);

        return Path.Combine(
            appDataPath,
            "FileScanner",
            "settings.json");
    }

    private void EnsureDirectoryExists()
    {
        var directory = Path.GetDirectoryName(_settingsPath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);
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