// Configuration/UserSettings.cs
namespace FileScanner.Configuration.Models;

public sealed class UserSettings
{
    public string? LastProjectPath { get; set; }
    public string? LastOutputPath { get; set; }
    public DateTime LastUsed { get; set; }
}