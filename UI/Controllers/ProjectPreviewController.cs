// UI/Controllers/ProjectPreviewController.cs
namespace FileScanner.UI.Controllers;

public sealed class ProjectPreviewController(
    IMainFormView view,
    ITreeGenerator treeGenerator,
    IProjectStatisticsCalculator statsCalculator)
{
    public async Task UpdateProjectPreview()
    {
        if (!IsPathValid())
        {
            view.ShowInvalidPathInPreview();
            return;
        }

        view.SetWaitCursor(true);

        try
        {
            await GenerateAndDisplayPreview();
        }
        catch (Exception ex)
        {
            DisplayError(ex);
        }
        finally
        {
            view.SetWaitCursor(false);
        }
    }

    private bool IsPathValid() =>
        !string.IsNullOrWhiteSpace(view.ProjectPath) && Directory.Exists(view.ProjectPath);

    private async Task GenerateAndDisplayPreview()
    {
        var projectPath = new DirectoryPath(view.ProjectPath);

        var treeTask = GenerateTreeAsync(projectPath);
        var statsTask = CalculateStatsAsync(projectPath);

        await Task.WhenAll(treeTask, statsTask);

        view.ProjectTreeText = await treeTask;
        view.StatisticsText = FormatProjectStats(await statsTask);
    }

    private Task<string> GenerateTreeAsync(DirectoryPath projectPath) =>
        Task.Run(() => treeGenerator.GenerateDirectoryTree(projectPath));

    private Task<ProjectStatistics> CalculateStatsAsync(DirectoryPath projectPath) =>
        statsCalculator.CalculateAsync(projectPath, CancellationToken.None);

    private void DisplayError(Exception ex)
    {
        view.ProjectTreeText = $"Error generating view: {ex.Message}";
        view.StatisticsText = "Error";
    }

    private static string FormatProjectStats(ProjectStatistics stats) =>
        $"Files: {stats.FileCount:N0} | " +
        $"Folders: {stats.DirectoryCount:N0} | " +
        $"Size: {UIHelper.FormatFileSize(stats.TotalSize)}";
}