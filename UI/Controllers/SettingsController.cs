// UI/Controllers/SettingsController.cs
namespace FileScanner.UI.Controllers;

public sealed class SettingsController(
    IMainFormView view,
    IUserSettingsService userSettingsService,
    IDefaultPathProvider defaultPathProvider,
    ILogger<SettingsController> logger)
{
    private readonly SettingsManager _settingsManager =
        new(userSettingsService, defaultPathProvider, logger);

    public void LoadInitialSettings()
    {
        var settings = _settingsManager.Load();

        view.ProjectPath = settings.LastProjectPath ??
            defaultPathProvider.GetDefaultProjectPath().Value;

        view.OutputPath = settings.LastOutputPath ??
            defaultPathProvider.GetDefaultOutputPath(new DirectoryPath(view.ProjectPath)).Value;
    }

    public void SaveCurrentSettings() =>
        _settingsManager.Save(view.ProjectPath, view.OutputPath);
}