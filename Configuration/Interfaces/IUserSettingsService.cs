// Configuration/Interfaces/IUserSettingsService.cs
namespace FileScanner.Configuration.Interfaces;

public interface IUserSettingsService
{
    UserSettings Load();
    void Save(UserSettings settings);
}