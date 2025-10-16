// UI/Forms/MainFormLogic/ProjectViewManager.cs
namespace FileScanner.UI.Forms.MainFormLogic;

public sealed class ProjectViewManager(
    MainForm form,
    ITreeGenerator treeGenerator,
    IProjectStatisticsCalculator statsCalculator,
    ILogger logger)
{
    public async Task UpdateProjectViewAsync()
    {
        var projectPathStr = form.GetTrimmedProjectPath();
        if (!IsPathValid(projectPathStr))
        {
            form.DisplayInvalidPathMessageInTree();
            return;
        }

        var projectPath = new DirectoryPath(projectPathStr);
        await PerformViewUpdateAsync(projectPath);
    }

    private async Task PerformViewUpdateAsync(DirectoryPath projectPath)
    {
        form.SetWaitCursor(true);
        try
        {
            await Task.WhenAll(
                GenerateTreeAsync(projectPath),
                DisplayStatsAsync(projectPath)
            );
        }
        catch (Exception ex)
        {
            HandleUpdateError(ex);
        }
        finally
        {
            form.SetWaitCursor(false);
        }
    }

    private async Task GenerateTreeAsync(DirectoryPath projectPath)
    {
        var treeText = await Task.Run(() => treeGenerator.GenerateDirectoryTree(projectPath));
        form.UpdateProjectTreeText(treeText);
    }

    private async Task DisplayStatsAsync(DirectoryPath projectPath)
    {
        var stats = await statsCalculator.CalculateAsync(projectPath, CancellationToken.None);
        var statsText = FormatProjectStats(stats);
        form.UpdateFileCountLabel(statsText);
    }

    private static bool IsPathValid(string? path) =>
        !string.IsNullOrWhiteSpace(path) && Directory.Exists(path);

    private static string FormatProjectStats(ProjectStatistics stats) =>
        $"Files: {stats.FileCount:N0} | " +
        $"Folders: {stats.DirectoryCount:N0} | " +
        $"Size: {UIHelper.FormatFileSize(stats.TotalSize)}";

    private void HandleUpdateError(Exception ex)
    {
        form.UpdateProjectTreeText($"Error generating view: {ex.Message}");
        form.UpdateFileCountLabel("Error");
        logger.LogError(ex, "Error updating project view");
    }
}