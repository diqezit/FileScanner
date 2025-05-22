#nullable enable

namespace FileScanner.UI.Forms;

public partial class MainForm : Form
{
    private readonly IFileScanner _fileScanner;
    private readonly IProjectPathResolver _pathResolver;
    private readonly ITreeGenerator _treeGenerator;
    private readonly IDirectoryValidator _validator;
    private readonly IUserSettingsService _userSettingsService;
    private readonly ILogger<MainForm> _logger;
    private readonly ScannerConfiguration _config;
    private CancellationTokenSource? _cancellationTokenSource;

    public MainForm(
        IFileScanner fileScanner,
        IProjectPathResolver pathResolver,
        ITreeGenerator treeGenerator,
        IDirectoryValidator validator,
        IUserSettingsService userSettingsService,
        ILogger<MainForm> logger,
        IOptions<ScannerConfiguration> configuration)
    {
        InitializeComponent();

        _fileScanner = fileScanner;
        _pathResolver = pathResolver;
        _treeGenerator = treeGenerator;
        _validator = validator;
        _userSettingsService = userSettingsService;
        _logger = logger;
        _config = configuration.Value;

        SetupUI();
        LoadSettings();
    }

    private void LoadSettings()
    {
        try
        {
            var settings = _userSettingsService.Load();
            ApplySettings(settings);
        }
        catch
        {
            ApplyDefaultSettings();
        }

        UpdateStatus("Ready");
        GenerateProjectTree();
    }

    private void ApplySettings(UserSettings settings)
    {
        if (IsValidSettingsPath(settings.LastProjectPath))
        {
            SetProjectPath(settings.LastProjectPath!);
            SetOutputPath(settings.LastOutputPath ?? GetDefaultOutputPath(settings.LastProjectPath!));
        }
        else
        {
            ApplyDefaultSettings();
        }
    }

    private static bool IsValidSettingsPath(string? path) =>
        !string.IsNullOrEmpty(path) && Directory.Exists(path);

    private void SetProjectPath(string path) =>
        txtProjectPath.Text = path;

    private void SetOutputPath(string path) =>
        txtOutputDirectory.Text = path;

    private void ApplyDefaultSettings()
    {
        var projectPath = FindDefaultProjectPath();
        SetProjectPath(projectPath);
        SetOutputPath(GetDefaultOutputPath(projectPath));
    }

    private string FindDefaultProjectPath()
    {
        var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        return _pathResolver.FindProjectRoot(currentDirectory, _config.DefaultProjectName)
            ?? currentDirectory;
    }

    private string GetDefaultOutputPath(string projectPath)
    {
        var projectRoot = _pathResolver.FindProjectRoot(
            projectPath,
            _config.DefaultProjectName);

        return projectRoot != null
            ? Path.Combine(projectRoot, _config.DefaultOutputFolderName)
            : Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                _config.DefaultOutputFolderName);
    }

    private void SaveCurrentSettings()
    {
        try
        {
            var settings = CreateCurrentSettings();
            _userSettingsService.Save(settings);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to save user settings");
        }
    }

    private UserSettings CreateCurrentSettings() => new()
    {
        LastProjectPath = GetTrimmedProjectPath(),
        LastOutputPath = GetTrimmedOutputPath(),
        LastUsed = DateTime.Now
    };

    private string GetTrimmedProjectPath() =>
        txtProjectPath.Text.Trim();

    private string GetTrimmedOutputPath() =>
        txtOutputDirectory.Text.Trim();

    private void BtnBrowseProject_Click(object? sender, EventArgs e)
    {
        BrowseFolder(
            "Select project root folder",
            txtProjectPath.Text,
            UpdateProjectPath);
    }

    private void UpdateProjectPath(string path)
    {
        SetProjectPath(path);
        SetOutputPath(GetDefaultOutputPath(path));
        UpdateStatus("Project folder updated");
        GenerateProjectTree();
        SaveCurrentSettings();
    }

    private void BtnBrowseOutput_Click(object? sender, EventArgs e)
    {
        BrowseFolder(
            "Select output folder",
            txtOutputDirectory.Text,
            UpdateOutputPath,
            true);
    }

    private void UpdateOutputPath(string path)
    {
        SetOutputPath(path);
        UpdateStatus("Output folder updated");
        SaveCurrentSettings();
    }

    private async void BtnStartScan_Click(object? sender, EventArgs e)
    {
        if (ValidateInputs())
            await PerformScan();
    }

    private void BtnCancel_Click(object? sender, EventArgs e)
    {
        RequestCancellation();
    }

    private void RequestCancellation()
    {
        _cancellationTokenSource?.Cancel();
        UpdateStatus("Cancelling operation...");
        _logger.LogWarning("Cancellation requested");
    }

    private async Task PerformScan()
    {
        _cancellationTokenSource = new CancellationTokenSource();

        try
        {
            PrepareForScanning();
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            await ExecuteScan();

            stopwatch.Stop();
            OnScanCompleted(stopwatch.Elapsed);
        }
        catch (OperationCanceledException)
        {
            OnScanCancelled();
        }
        catch (Exception ex)
        {
            OnScanError(ex);
        }
        finally
        {
            CleanupAfterScan();
        }
    }

    private void PrepareForScanning()
    {
        SetScanningState(true);
        ClearLog();
        UpdateStatus("Scanning in progress...");
        ResetProgressBar();
        LogScanStart();
    }

    private void ClearLog() =>
        txtLog.Clear();

    private void ResetProgressBar() =>
        progressBar.Value = 0;

    private void LogScanStart()
    {
        _logger.LogInformation("=== Starting project scan ===");
        _logger.LogInformation("Project: {ProjectPath}", txtProjectPath.Text);
        _logger.LogInformation("Output: {OutputPath}", txtOutputDirectory.Text);
    }

    private async Task ExecuteScan()
    {
        var projectPath = GetTrimmedProjectPath();
        var outputPath = GetTrimmedOutputPath();

        await Task.Run(
            async () => await _fileScanner.ScanAndGenerateAsync(
                projectPath,
                outputPath,
                _cancellationTokenSource!.Token),
            _cancellationTokenSource!.Token);
    }

    private void OnScanCompleted(TimeSpan elapsed)
    {
        UpdateStatus("Scan completed successfully");
        UpdateFileCountLabel($"Done! Time: {elapsed:mm\\:ss}");
        LogScanCompletion(elapsed);
        SaveCurrentSettings();
        ShowScanSuccessDialog();
    }

    private void UpdateFileCountLabel(string text) =>
        fileCountLabel.Text = text;

    private void LogScanCompletion(TimeSpan elapsed)
    {
        _logger.LogInformation("=== Scan completed successfully ===");
        _logger.LogInformation("Elapsed time: {ElapsedTime}", elapsed);
    }

    private void ShowScanSuccessDialog()
    {
        var message = CreateSuccessMessage();
        var result = ShowQuestionDialog(message, "Success");

        if (result == DialogResult.Yes)
            OpenOutputFolder();
    }

    private string CreateSuccessMessage() =>
        $"Scan completed successfully!\n\n" +
        $"Files saved to:\n{GetTrimmedOutputPath()}\n\n" +
        $"Open output folder?";

    private void OpenOutputFolder() =>
        OpenFolder(GetTrimmedOutputPath());

    private void OnScanCancelled()
    {
        UpdateStatus("Scan cancelled");
        UpdateFileCountLabel("Cancelled");
        _logger.LogWarning("Scan was cancelled by user");
    }

    private void OnScanError(Exception ex)
    {
        UpdateStatus("Scan error");
        UpdateFileCountLabel("Error");
        _logger.LogError(ex, "Error occurred during scan");
        ShowScanErrorDialog(ex);
    }

    private static void ShowScanErrorDialog(Exception ex)
    {
        var message = $"An error occurred during scan:\n\n" +
                     $"{ex.Message}\n\n" +
                     $"Check logs for details.";
        ShowErrorMessage(message);
    }

    private void CleanupAfterScan()
    {
        SetScanningState(false);
        DisposeCancellationToken();
    }

    private void DisposeCancellationToken()
    {
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
    }

    private void GenerateProjectTree()
    {
        try
        {
            var projectPath = GetTrimmedProjectPath();

            if (!IsValidProjectPath(projectPath))
            {
                ShowInvalidPathMessage();
                return;
            }

            DisplayProjectTree(projectPath);
            DisplayProjectStats(projectPath);
        }
        catch (Exception ex)
        {
            HandleTreeGenerationError(ex);
        }
        finally
        {
            RestoreCursor();
        }
    }

    private static bool IsValidProjectPath(string path) =>
        !string.IsNullOrEmpty(path) && Directory.Exists(path);

    private void ShowInvalidPathMessage()
    {
        txtTree.Text = "Please specify a valid project path";
        fileCountLabel.Text = string.Empty;
    }

    private void DisplayProjectTree(string projectPath)
    {
        SetWaitCursor();
        txtTree.Text = _treeGenerator.GenerateDirectoryTree(projectPath);
    }

    private void SetWaitCursor() =>
        Cursor = Cursors.WaitCursor;

    private void RestoreCursor() =>
        Cursor = Cursors.Default;

    private void DisplayProjectStats(string projectPath)
    {
        try
        {
            var stats = CalculateProjectStats(projectPath);
            var statsText = FormatProjectStats(stats);
            UpdateFileCountLabel(statsText);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error calculating project statistics");
        }
    }

    private ProjectStats CalculateProjectStats(string path)
    {
        var directories = GetValidDirectories(path);
        var files = GetValidFiles(path);

        return new ProjectStats
        {
            DirectoryCount = directories.Count(),
            FileCount = files.Count(),
            TotalSize = CalculateTotalSize(files)
        };
    }

    private IEnumerable<string> GetValidDirectories(string path) =>
        Directory.EnumerateDirectories(path, "*", SearchOption.AllDirectories)
            .Where(IsValidDirectory);

    private bool IsValidDirectory(string dir) =>
        !_validator.ShouldIgnoreDirectory(Path.GetFileName(dir));

    private IEnumerable<string> GetValidFiles(string path) =>
        Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories)
            .Where(IsValidFile);

    private bool IsValidFile(string file) =>
        !_validator.ShouldIgnoreFile(file);

    private static long CalculateTotalSize(IEnumerable<string> files) =>
        files.Sum(file => new FileInfo(file).Length);

    private static string FormatProjectStats(ProjectStats stats) =>
        $"Files: {stats.FileCount:N0} | " +
        $"Folders: {stats.DirectoryCount:N0} | " +
        $"Size: {FormatFileSize(stats.TotalSize)}";

    private void HandleTreeGenerationError(Exception ex)
    {
        txtTree.Text = $"Error generating tree: {ex.Message}";
        UpdateFileCountLabel("Error");
        _logger.LogError(ex, "Error generating project tree");
    }

    private bool ValidateInputs()
    {
        var projectPath = GetTrimmedProjectPath();
        var outputPath = GetTrimmedOutputPath();

        return ValidateProjectPath(projectPath) &&
               ValidateOutputPath(outputPath);
    }

    private bool ValidateProjectPath(string projectPath)
    {
        if (string.IsNullOrWhiteSpace(projectPath))
        {
            ShowValidationError("Please specify project path");
            return false;
        }

        if (!Directory.Exists(projectPath))
        {
            ShowValidationError($"Project directory does not exist:\n{projectPath}");
            return false;
        }

        return true;
    }

    private bool ValidateOutputPath(string outputPath)
    {
        if (string.IsNullOrWhiteSpace(outputPath))
        {
            ShowValidationError("Please specify output directory");
            return false;
        }

        return true;
    }

    private void ShowValidationError(string message)
    {
        ShowWarning(message);
        UpdateStatus("Validation error");
    }

    private void SetScanningState(bool isScanning)
    {
        UpdateControlsEnabled(!isScanning);
        UpdateButtonVisibility(isScanning);
        UpdateProgressBarState(isScanning);
    }

    private void UpdateControlsEnabled(bool enabled)
    {
        btnStartScan.Enabled = enabled;
        btnBrowseProject.Enabled = enabled;
        btnBrowseOutput.Enabled = enabled;
        txtProjectPath.Enabled = enabled;
        txtOutputDirectory.Enabled = enabled;
    }

    private void UpdateButtonVisibility(bool isScanning)
    {
        btnStartScan.Visible = !isScanning;
        btnCancel.Visible = isScanning;
        progressBar.Visible = isScanning;
        btnStartScan.Text = isScanning ? "⏳ Processing..." : "🚀 Start";
    }

    private void UpdateProgressBarState(bool isScanning)
    {
        if (isScanning)
            SetProgressBarMarquee();
        else
            ResetProgressBarState();
    }

    private void SetProgressBarMarquee()
    {
        progressBar.Style = ProgressBarStyle.Marquee;
        progressBar.MarqueeAnimationSpeed = 30;
    }

    private void ResetProgressBarState()
    {
        progressBar.Style = ProgressBarStyle.Continuous;
        progressBar.Value = 0;
    }

    private void UpdateStatus(string message) =>
        statusLabel.Text = message;

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (IsScanRunning())
        {
            e.Cancel = !ConfirmClose();
        }
        else
        {
            SaveCurrentSettings();
        }

        base.OnFormClosing(e);
    }

    private bool IsScanRunning() =>
        _cancellationTokenSource != null &&
        !_cancellationTokenSource.Token.IsCancellationRequested;

    private bool ConfirmClose()
    {
        var message = "Scan is still running. Are you sure you want to close?";
        var result = ShowQuestionDialog(message, "Confirm");

        if (result == DialogResult.Yes)
        {
            RequestCancellation();
            return true;
        }

        return false;
    }

    private static void BrowseFolder(
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
        {
            onSelected(dialog.SelectedPath);
        }
    }

    private static void OpenFolder(string path)
    {
        try
        {
            System.Diagnostics.Process.Start("explorer.exe", path);
        }
        catch
        {
        }
    }

    private static string FormatFileSize(long bytes)
    {
        string[] sizes = ["B", "KB", "MB", "GB", "TB"];
        double len = bytes;
        int order = 0;

        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }

        return $"{len:0.##} {sizes[order]}";
    }

    private static void ShowWarning(string message) =>
        MessageBox.Show(
            message,
            "Validation Error",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);

    private static void ShowErrorMessage(string message) =>
        MessageBox.Show(
            message,
            "Error",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);

    private static DialogResult ShowQuestionDialog(string message, string title) =>
        MessageBox.Show(
            message,
            title,
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Information);

    private class ProjectStats
    {
        public int FileCount { get; set; }
        public int DirectoryCount { get; set; }
        public long TotalSize { get; set; }
    }
}