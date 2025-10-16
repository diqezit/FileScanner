// UI/Forms/MainFormLogic/UITheme.cs
namespace FileScanner.UI.Forms.MainFormLogic;

public static class UITheme
{
    public static readonly Color FormBackground = Color.FromArgb(248, 249, 250);
    public static readonly Color TextboxBackground = Color.FromArgb(245, 245, 245);
    public static readonly Color DarkControlBackground = Color.FromArgb(30, 30, 30);
    public static readonly Color StatusStripBackground = Color.FromArgb(37, 37, 38);

    public static readonly Color PrimaryText = Color.FromArgb(32, 32, 32);
    public static readonly Color SecondaryText = Color.FromArgb(128, 128, 128);
    public static readonly Color LightText = Color.White;
    public static readonly Color MutedLightText = Color.FromArgb(180, 180, 180);

    public static readonly Color AccentColor = Color.FromArgb(0, 120, 215);
    public static readonly Color DestructiveColor = Color.FromArgb(210, 43, 43);
    public static readonly Color NeutralButton = Color.FromArgb(225, 225, 225);

    public static readonly Font TitleFont = new("Segoe UI", 10F, FontStyle.Bold);
    public static readonly Font SubtitleFont = new("Segoe UI", 9F, FontStyle.Bold);
    public static readonly Font BodyFont = new("Segoe UI", 8F);
    public static readonly Font CodeFont = new("Consolas", 8.5F);
    public static readonly Font IconFont = new("Segoe Fluent Icons", 9F);
    public static readonly Font TreeFont = new("Cascadia Code", 9F);
    public static readonly Font LogFont = new("Cascadia Code", 8F);
}