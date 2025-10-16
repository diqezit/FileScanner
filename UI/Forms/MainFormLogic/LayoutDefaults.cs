// UI/Forms/MainFormLogic/LayoutDefaults.cs
namespace FileScanner.UI.Forms.MainFormLogic;

public static class LayoutDefaults
{
    public static readonly AnchorStyles TopLeftRight =
        AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

    public static readonly AnchorStyles AllSides =
        AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

    public static readonly Padding SmallPadding = new(2);
    public static readonly Padding MediumPadding = new(20);
}