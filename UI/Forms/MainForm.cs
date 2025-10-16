// UI/Forms/MainForm.cs
#nullable enable

namespace FileScanner.UI.Forms;

public partial class MainForm : Form
{
    private readonly SettingsManager _settingsManager;
    private readonly ProjectViewManager _projectViewManager;
    private readonly ScanOrchestrator _scanOrchestrator;
    private readonly ScanUIHandler _scanUIHandler;
    private readonly IDefaultPathProvider _defaultPathProvider;

    public MainForm(
        ITreeGenerator treeGenerator,
        IProjectStatisticsCalculator statsCalculator,
        IUserSettingsService userSettingsService,
        IDefaultPathProvider defaultPathProvider,
        ScanOrchestrator scanOrchestrator,
        FormLoggerProvider formLoggerProvider,
        ILogger<MainForm> logger)
    {
        InitializeComponent();

        formLoggerProvider.LogTextBox = this.txtLog;

        _defaultPathProvider = defaultPathProvider;
        _settingsManager = new SettingsManager(userSettingsService, defaultPathProvider, logger);
        _projectViewManager = new ProjectViewManager(this, treeGenerator, statsCalculator, logger);
        _scanOrchestrator = scanOrchestrator;
        _scanUIHandler = new ScanUIHandler(this, logger);

        WireUpEvents();
        LoadAndApplySettings();
    }

    private void WireUpEvents()
    {
        _scanOrchestrator.ScanStarted += _scanUIHandler.OnScanStarted;
        _scanOrchestrator.ScanCompleted += OnScanCompleted;
        _scanOrchestrator.ScanCancelled += _scanUIHandler.OnScanCancelled;
        _scanOrchestrator.ScanFailed += _scanUIHandler.OnScanFailed;
    }

    private void LoadAndApplySettings()
    {
        var settings = _settingsManager.Load();
        ApplySettings(settings);
    }

    private void ApplySettings(UserSettings settings)
    {
        DirectoryPath projectPath;
        if (settings.LastProjectPath is not null && Directory.Exists(settings.LastProjectPath))
            projectPath = new DirectoryPath(settings.LastProjectPath);
        else
            projectPath = _defaultPathProvider.GetDefaultProjectPath();

        var outputPath = settings.LastOutputPath is not null && Directory.Exists(settings.LastOutputPath)
            ? new DirectoryPath(settings.LastOutputPath)
            : _defaultPathProvider.GetDefaultOutputPath(projectPath);

        SetProjectPath(projectPath.Value);
        SetOutputPath(outputPath.Value);
        UpdateStatus("Ready");
        GenerateProjectTree();
    }

    private void OnProjectPathSelected(string path)
    {
        var projectPath = new DirectoryPath(path);
        SetProjectPath(projectPath.Value);
        SetOutputPath(_defaultPathProvider.GetDefaultOutputPath(projectPath).Value);
        UpdateStatus("Project folder updated");
        GenerateProjectTree();
        SaveCurrentSettings();
    }

    private void SaveCurrentSettings() =>
       _settingsManager.Save(GetTrimmedProjectPath(), GetTrimmedOutputPath());

    private void OnScanCompleted(object? sender, ScanCompletedEventArgs e)
    {
        _scanUIHandler.OnScanCompleted(sender, e);
        SaveCurrentSettings();
    }

    private async void BtnStartScan_Click(object? _, EventArgs __)
    {
        if (ValidateScanInputs())
            await _scanOrchestrator.PerformScanAsync(
                GetTrimmedProjectPath(),
                GetTrimmedOutputPath());
    }

    private void BtnCancel_Click(object? _, EventArgs __) =>
        _scanOrchestrator.RequestCancellation();

    private void BtnBrowseProject_Click(object? _, EventArgs __) =>
        UIHelper.BrowseFolder("Select project root folder", txtProjectPath.Text, OnProjectPathSelected);

    private void BtnBrowseOutput_Click(object? _, EventArgs __) =>
        UIHelper.BrowseFolder("Select output folder", txtOutputDirectory.Text, OnOutputPathSelected, true);

    private void OnOutputPathSelected(string path)
    {
        SetOutputPath(path);
        UpdateStatus("Output folder updated");
        SaveCurrentSettings();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (_scanOrchestrator.IsScanRunning)
        {
            var message = "Scan is still running. Are you sure you want to close?";
            if (UIHelper.ShowQuestionDialog(message, "Confirm") == DialogResult.Yes)
                _scanOrchestrator.RequestCancellation();
            else
                e.Cancel = true;
        }
        else
        {
            SaveCurrentSettings();
        }
        base.OnFormClosing(e);
    }

    public string GetTrimmedProjectPath() => txtProjectPath.Text.Trim();
    public string GetTrimmedOutputPath() => txtOutputDirectory.Text.Trim();
    public void SetProjectPath(string path) => txtProjectPath.Text = path;
    public void SetOutputPath(string path) => txtOutputDirectory.Text = path;
    public void UpdateStatus(string message) => statusLabel.Text = message;
    public void UpdateFileCountLabel(string text) => fileCountLabel.Text = text;
    public void ClearLog() => txtLog.Clear();
    public void SetWaitCursor(bool isWaiting) => Cursor = isWaiting ? Cursors.WaitCursor : Cursors.Default;
    public async void GenerateProjectTree() => await _projectViewManager.UpdateProjectViewAsync();
    public void UpdateProjectTreeText(string text) => txtTree.Text = text;

    public void DisplayInvalidPathMessageInTree()
    {
        txtTree.Text = "Please specify a valid project path";
        fileCountLabel.Text = string.Empty;
    }

    public void SetScanningState(bool isScanning)
    {
        bool controlsEnabled = !isScanning;
        txtProjectPath.Enabled = controlsEnabled;
        txtOutputDirectory.Enabled = controlsEnabled;
        btnBrowseProject.Enabled = controlsEnabled;
        btnBrowseOutput.Enabled = controlsEnabled;
        btnStartScan.Visible = !isScanning;
        btnCancel.Visible = isScanning;
        progressBar.Visible = isScanning;
        progressBar.Style = isScanning ? ProgressBarStyle.Marquee : ProgressBarStyle.Continuous;
        progressBar.Value = 0;
    }

    private bool ValidateScanInputs()
    {
        var projectPath = GetTrimmedProjectPath();
        if (string.IsNullOrWhiteSpace(projectPath) || !Directory.Exists(projectPath))
        {
            UIHelper.ShowWarning($"Project directory does not exist:\n{projectPath}");
            return false;
        }

        var outputPath = GetTrimmedOutputPath();
        if (string.IsNullOrWhiteSpace(outputPath))
        {
            UIHelper.ShowWarning("Please specify output directory");
            return false;
        }
        return true;
    }
}