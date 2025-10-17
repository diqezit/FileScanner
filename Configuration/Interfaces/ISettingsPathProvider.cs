// Configuration/Interfaces/ISettingsPathProvider.cs
namespace FileScanner.Configuration.Interfaces;

public interface ISettingsPathProvider
{
    string GetSettingsFilePath();
}