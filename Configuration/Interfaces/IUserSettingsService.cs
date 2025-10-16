// Configuration/Interfaces/IUserSettingsService.cs
using FileScanner.Configuration.Models;

namespace FileScanner.Configuration.Interfaces;

public interface IUserSettingsService
{
    UserSettings Load();
    void Save(UserSettings settings);
}