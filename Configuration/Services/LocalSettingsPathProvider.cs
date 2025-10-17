// Configuration/Services/LocalSettingsPathProvider.cs
namespace FileScanner.Configuration.Services;

public sealed class LocalSettingsPathProvider : ISettingsPathProvider
{
    private const string SettingsFileName = "usersettings.json";

    // Places settings file next to the application .exe
    public string GetSettingsFilePath()
    {
        var applicationDirectory = AppDomain.CurrentDomain.BaseDirectory;
        return Path.Combine(applicationDirectory, SettingsFileName);
    }
}