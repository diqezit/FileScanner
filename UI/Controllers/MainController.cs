namespace FileScanner.UI.Controllers;

public sealed class MainController
{
    private readonly IMainFormView _view;
    private readonly ScanOrchestrator _scanOrchestrator;
    private readonly IDefaultPathProvider _defaultPathProvider;
    private readonly IUserSettingsService _userSettingsService;
    private readonly ITreeGenerator _treeGenerator;
    private readonly IProjectStatisticsCalculator _statsCalculator;
    private readonly ILogger<SettingsController> _settingsLogger;

    private readonly SettingsController _settingsController;
    private readonly ProjectPreviewController _previewController;
    private readonly ScanFeedbackController _feedbackController;

    public MainController(
        IMainFormView view,
        ScanOrchestrator scanOrchestrator,
        IDefaultPathProvider defaultPathProvider,
        IUserSettingsService userSettingsService,
        ITreeGenerator treeGenerator,
        IProjectStatisticsCalculator statsCalculator,
        ILogger<SettingsController> settingsLogger)
    {
        _view = view;
        _scanOrchestrator = scanOrchestrator;
        _defaultPathProvider = defaultPathProvider;
        _userSettingsService = userSettingsService;
        _treeGenerator = treeGenerator;
        _statsCalculator = statsCalculator;
        _settingsLogger = settingsLogger;

        _settingsController = new SettingsController(
            _view,
            _userSettingsService,
            _defaultPathProvider,
            _settingsLogger);

        _previewController = new ProjectPreviewController(
            _view,
            _treeGenerator,
            _statsCalculator);

        _feedbackController = new ScanFeedbackController(_view);
    }

    public void Initialize()
    {
        WireUpViewEvents();
        WireUpOrchestratorEvents();
    }

    private void WireUpViewEvents()
    {
        _view.LoadForm += OnViewLoad;
        _view.StartScanClick += OnStartScan;
        _view.CancelScanClick += OnCancelScan;
        _view.BrowseProjectClick += OnBrowseProject;
        _view.BrowseOutputClick += OnBrowseOutput;
    }

    private void WireUpOrchestratorEvents()
    {
        _scanOrchestrator.ScanStarted += (s, e) =>
            _feedbackController.HandleScanStarted();

        _scanOrchestrator.ScanCompleted += (s, e) =>
        {
            _feedbackController.HandleScanCompleted(e);
            _settingsController.SaveCurrentSettings();
        };

        _scanOrchestrator.ScanCancelled += (s, e) =>
            _feedbackController.HandleScanCancelled();

        _scanOrchestrator.ScanFailed += (s, e) =>
            _feedbackController.HandleScanFailed(e);
    }

    private void OnViewLoad(object? sender, EventArgs e)
    {
        _settingsController.LoadInitialSettings();
        _ = HandleAsyncTask(_previewController.UpdateProjectPreview());
    }

    private void OnStartScan(object? sender, EventArgs e)
    {
        if (!ValidateInputs())
            return;

        var options = new PostScanOptions(
            IsSplitEnabled: _view.IsSplitEnabled,
            ChunkSize: _view.ChunkSizeInChars
        );

        _ = HandleAsyncTask(
            _scanOrchestrator.PerformScanAsync(
                _view.ProjectPath,
                _view.OutputPath,
                options));
    }

    private void OnCancelScan(object? sender, EventArgs e) =>
        _scanOrchestrator.RequestCancellation();

    private void OnBrowseProject(object? sender, EventArgs e) =>
        UIHelper.BrowseFolder(
            "Select project root folder",
            _view.ProjectPath,
            path =>
            {
                _view.ProjectPath = path;
                _view.OutputPath = _defaultPathProvider.GetDefaultOutputPath(
                    new DirectoryPath(path)).Value;

                _ = HandleAsyncTask(_previewController.UpdateProjectPreview());
            });

    private void OnBrowseOutput(object? sender, EventArgs e) =>
        UIHelper.BrowseFolder(
            "Select output folder",
            _view.OutputPath,
            path => _view.OutputPath = path,
            true);

    public void HandleFormClosing(FormClosingEventArgs e)
    {
        if (_scanOrchestrator.IsScanRunning)
            HandleCloseWithRunningScan(e);
        else
            _settingsController.SaveCurrentSettings();
    }

    private void HandleCloseWithRunningScan(FormClosingEventArgs e)
    {
        const string message = "Scan is still running. Are you sure you want to close?";
        bool userConfirmed = UIHelper.ShowQuestionDialog(message, "Confirm") == DialogResult.Yes;

        if (userConfirmed)
            _scanOrchestrator.RequestCancellation();
        else
            e.Cancel = true;
    }

    private bool ValidateInputs()
    {
        if (string.IsNullOrWhiteSpace(_view.ProjectPath) || !Directory.Exists(_view.ProjectPath))
        {
            UIHelper.ShowWarning(
                $"Project directory does not exist:\n{_view.ProjectPath}");
            return false;
        }

        if (string.IsNullOrWhiteSpace(_view.OutputPath))
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