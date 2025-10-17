#nullable enable

namespace FileScanner.UI.Controllers;

public sealed class ProjectPreviewController(
    IMainFormView view,
    IProjectEnumerator projectEnumerator,
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
        !string.IsNullOrWhiteSpace(view.ProjectPath) &&
        Directory.Exists(view.ProjectPath);

    private async Task GenerateAndDisplayPreview()
    {
        var projectPath = new DirectoryPath(view.ProjectPath);

        var projectStructure = await Task.Run(() =>
            projectEnumerator.EnumerateProject(
                projectPath,
                view.UseProjectFilters));

        var treeTask = GenerateTreeAsync(projectStructure, projectPath);
        var statsTask = CalculateStatsAsync(projectStructure);

        await Task.WhenAll(treeTask, statsTask);

        view.ProjectTreeText = await treeTask;
        view.StatisticsText = FormatProjectStats(await statsTask);
    }

    private Task<string> GenerateTreeAsync(
        ProjectStructure structure,
        DirectoryPath root) =>
        Task.Run(() =>
            treeGenerator.GenerateDirectoryTree(structure, root));

    private Task<ProjectStatistics> CalculateStatsAsync(
        ProjectStructure structure) =>
        statsCalculator.CalculateAsync(
            structure,
            CancellationToken.None);

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