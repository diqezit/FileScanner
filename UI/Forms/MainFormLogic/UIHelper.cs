// UI/Forms/MainFormLogic/UIHelper.cs
#nullable enable

namespace FileScanner.UI.Forms.MainFormLogic;

/// <summary>
/// Provides static helper methods for common UI tasks like dialogs and formatting.
/// </summary>
public static class UIHelper
{
    public static void BrowseFolder(
        string description,
        string selectedPath,
        Action<string> onSelected,
        bool showNewFolderButton = false)
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = description,
            SelectedPath = selectedPath,
            UseDescriptionForTitle = true,
            ShowNewFolderButton = showNewFolderButton
        };

        if (dialog.ShowDialog() == DialogResult.OK)
            onSelected(dialog.SelectedPath);
    }

    // Wrap process start in try-catch to handle cases where explorer is unavailable
    public static void OpenFolder(string path)
    {
        try
        {
            System.Diagnostics.Process.Start("explorer.exe", path);
        }
        catch
        {
            // Fails silently if explorer.exe is missing or permissions fail
        }
    }

    public static string FormatFileSize(long bytes)
    {
        string[] sizes = ["B", "KB", "MB", "GB", "TB"];
        double len = bytes;
        int order = 0;
        // Use 1024 for binary units standard for file sizes
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }

    public static void ShowWarning(string message) =>
        MessageBox.Show(
            message,
            "Validation Error",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);

    public static void ShowErrorMessage(string message) =>
        MessageBox.Show(
            message,
            "Error",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);

    public static DialogResult ShowQuestionDialog(string message, string title) =>
        MessageBox.Show(
            message,
            title,
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Information);
}