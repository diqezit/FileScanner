#nullable enable

namespace FileScanner.UI.Forms;

public partial class MainForm : Form, IMainFormView
{
    private readonly MainController _controller;
    private bool _isScanning = false;

    public MainForm(
        ScanOrchestrator scanOrchestrator,
        IUserSettingsService userSettingsService,
        IDefaultPathProvider defaultPathProvider,
        ITreeGenerator treeGenerator,
        IProjectStatisticsCalculator statsCalculator,
        FormLoggerProvider formLoggerProvider,
        ILogger<SettingsController> settingsLogger,
        IOptions<ScannerConfiguration> scannerOptions)
    {
        InitializeComponent();
        formLoggerProvider.LogTextBox = this.txtLog;

        txtChunkSize.Text = scannerOptions.Value.DefaultChunkSize.ToString();

        _controller = new MainController(
            this,
            scanOrchestrator,
            defaultPathProvider,
            userSettingsService,
            treeGenerator,
            statsCalculator,
            settingsLogger);

        _controller.Initialize();
    }

    public string ProjectPath
    {
        get => txtProjectPath.Text.Trim();
        set => txtProjectPath.Text = value;
    }

    public string OutputPath
    {
        get => txtOutputDirectory.Text.Trim();
        set => txtOutputDirectory.Text = value;
    }

    public string ProjectTreeText { set => txtTree.Text = value; }
    public string StatisticsText { set => fileCountLabel.Text = value; }

    public bool IsSplitEnabled => chkSplitFile.Checked;
    public int ChunkSizeInChars => int.TryParse(txtChunkSize.Text, out var size) ? size : 0;

    public event EventHandler? LoadForm;
    public event EventHandler? StartScanClick;
    public event EventHandler? CancelScanClick;
    public event EventHandler? BrowseProjectClick;
    public event EventHandler? BrowseOutputClick;

    public void SetScanningState(bool isScanning)
    {
        _isScanning = isScanning;

        UpdateInputControlsState(!isScanning);
        UpdateActionButtonState(isScanning);
        UpdateProgressBarState(isScanning);
    }

    private void UpdateInputControlsState(bool isEnabled)
    {
        txtProjectPath.Enabled = isEnabled;
        txtOutputDirectory.Enabled = isEnabled;
        btnBrowseProject.Enabled = isEnabled;
        btnBrowseOutput.Enabled = isEnabled;
        chkSplitFile.Enabled = isEnabled;
        txtChunkSize.Enabled = isEnabled;
        lblChars.Enabled = isEnabled;
    }

    private void UpdateActionButtonState(bool isScanning)
    {
        btnAction.Enabled = true;

        if (isScanning)
        {
            btnAction.Text = "\uE71A Stop";
            btnAction.BackColor = UITheme.DestructiveColor;
        }
        else
        {
            btnAction.Text = "\uE768 Start";
            btnAction.BackColor = UITheme.AccentColor;
        }
    }

    private void UpdateProgressBarState(bool isScanning)
    {
        progressBar.Visible = isScanning;

        if (isScanning)
        {
            progressBar.Style = ProgressBarStyle.Marquee;
        }
        else
        {
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.Value = 0;
        }
    }

    public void ShowInvalidPathInPreview()
    {
        txtTree.Text = "Please specify a valid project path";
        fileCountLabel.Text = string.Empty;
    }

    public void SetWaitCursor(bool isWaiting) =>
        Cursor = isWaiting ? Cursors.WaitCursor : Cursors.Default;

    public void UpdateStatus(string message) =>
        statusLabel.Text = message;

    public void UpdateFileCountLabel(string text) =>
        fileCountLabel.Text = text;

    private void MainForm_Load(object? sender, EventArgs e) =>
        LoadForm?.Invoke(sender, e);

    private void BtnAction_Click(object? sender, EventArgs e)
    {
        if (_isScanning)
            CancelScanClick?.Invoke(this, EventArgs.Empty);
        else
            StartScanClick?.Invoke(this, EventArgs.Empty);
    }

    private void BtnBrowseProject_Click(object? sender, EventArgs e) =>
        BrowseProjectClick?.Invoke(sender, e);

    private void BtnBrowseOutput_Click(object? sender, EventArgs e) =>
        BrowseOutputClick?.Invoke(sender, e);

    private void ChkSplitFile_CheckedChanged(object? sender, EventArgs e)
    {
        bool isChecked = chkSplitFile.Checked;
        txtChunkSize.Visible = isChecked;
        lblChars.Visible = isChecked;
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _controller.HandleFormClosing(e);
        base.OnFormClosing(e);
    }
}