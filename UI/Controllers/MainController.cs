#nullable enable

namespace FileScanner.UI.Controllers;

public sealed class MainController(
    IMainFormView view,
    ScanOrchestrator scanOrchestrator,
    IDefaultPathProvider defaultPathProvider,
    IUserSettingsService userSettingsService,
    IProjectEnumerator projectEnumerator,
    ITreeGenerator treeGenerator,
    IProjectStatisticsCalculator statsCalculator,
    ILogger<SettingsController> settingsLogger)
{
    private readonly SettingsController _settingsController = new(
        view,
        userSettingsService,
        defaultPathProvider,
        settingsLogger);

    private readonly ProjectPreviewController _previewController = new(
        view,
        projectEnumerator,
        treeGenerator,
        statsCalculator);

    private readonly ScanFeedbackController _feedbackController = new(view);

    public void Initialize()
    {
        WireUpViewEvents();
        WireUpOrchestratorEvents();
    }

    private void WireUpViewEvents()
    {
        view.LoadForm += OnViewLoad;
        view.StartScanClick += OnStartScan;
        view.CancelScanClick += OnCancelScan;
        view.BrowseProjectClick += OnBrowseProject;
        view.BrowseOutputClick += OnBrowseOutput;
    }

    private void WireUpOrchestratorEvents()
    {
        scanOrchestrator.ScanStarted += (s, e) =>
            _feedbackController.HandleScanStarted();

        scanOrchestrator.ScanCompleted += (s, e) =>
        {
            _feedbackController.HandleScanCompleted(e);
            _settingsController.SaveCurrentSettings();
        };

        scanOrchestrator.ScanCancelled += (s, e) =>
            _feedbackController.HandleScanCancelled();

        scanOrchestrator.ScanFailed += (s, e) =>
            _feedbackController.HandleScanFailed(e);
    }

    private void OnViewLoad(object? sender, EventArgs e)
    {
        _settingsController.LoadInitialSettings();
        RequestPreviewUpdate();
    }

    public void RequestPreviewUpdate() =>
        _ = HandleAsyncTask(_previewController.UpdateProjectPreview());

    private void OnStartScan(object? sender, EventArgs e)
    {
        if (!ValidateInputs())
            return;

        var options = new ScanOptions(
            UseProjectFilters: view.UseProjectFilters,
            IsSplitEnabled: view.IsSplitEnabled,
            ChunkSize: view.ChunkSizeInChars
        );

        _ = HandleAsyncTask(
            scanOrchestrator.PerformScanAsync(
                view.ProjectPath,
                view.OutputPath,
                options));
    }

    private void OnCancelScan(object? sender, EventArgs e) =>
        scanOrchestrator.RequestCancellation();

    private void OnBrowseProject(object? sender, EventArgs e) =>
        UIHelper.BrowseFolder(
            "Select project root folder",
            view.ProjectPath,
            path =>
            {
                view.ProjectPath = path;
                view.OutputPath = defaultPathProvider
                    .GetDefaultOutputPath(new DirectoryPath(path)).Value;
                RequestPreviewUpdate();
            });

    private void OnBrowseOutput(object? sender, EventArgs e) =>
        UIHelper.BrowseFolder(
            "Select output folder",
            view.OutputPath,
            path => view.OutputPath = path,
            true);

    public void HandleFormClosing(FormClosingEventArgs e)
    {
        if (scanOrchestrator.IsScanRunning)
            HandleCloseWithRunningScan(e);
        else
            _settingsController.SaveCurrentSettings();
    }

    private void HandleCloseWithRunningScan(FormClosingEventArgs e)
    {
        const string message =
            "Scan is still running. Are you sure you want to close?";

        var userConfirmed = UIHelper.ShowQuestionDialog(
            message,
            "Confirm") == DialogResult.Yes;

        if (userConfirmed)
            scanOrchestrator.RequestCancellation();
        else
            e.Cancel = true;
    }

    private bool ValidateInputs()
    {
        if (string.IsNullOrWhiteSpace(view.ProjectPath) ||
            !Directory.Exists(view.ProjectPath))
        {
            UIHelper.ShowWarning(
                $"Project directory does not exist:\n{view.ProjectPath}");
            return false;
        }

        if (string.IsNullOrWhiteSpace(view.OutputPath))
        {
            UIHelper.ShowWarning("Please specify output directory");
            return false;
        }

        return true;
    }

    private async Task HandleAsyncTask(Task task)
    {
        try
        {
            await task;
        }
        catch (Exception ex)
        {
            _feedbackController.HandleScanFailed(ex);
        }
    }
}