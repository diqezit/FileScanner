namespace FileScanner.UI.Forms;

public partial class MainForm : Form
{
    private readonly IFileScanner _fileScanner;
    private readonly IProjectPathResolver _pathResolver;
    private readonly ITreeGenerator _treeGenerator;
    private readonly ILogger<MainForm> _logger;
    private readonly ScannerConfiguration _config;
    private CancellationTokenSource? _cancellationTokenSource;

    public MainForm(
        IFileScanner fileScanner,
        IProjectPathResolver pathResolver,
        ITreeGenerator treeGenerator,
        ILogger<MainForm> logger,
        IOptions<ScannerConfiguration> configuration)
    {
        InitializeComponent();

        _fileScanner = fileScanner;
        _pathResolver = pathResolver;
        _treeGenerator = treeGenerator;
        _logger = logger;
        _config = configuration.Value;

        SetupUI();
        SetDefaultPaths();
    }

    private void SetupUI()
    {
        txtLog.ReadOnly = true;
        txtLog.ScrollBars = ScrollBars.Vertical;
        txtLog.WordWrap = true;

        txtTree.ReadOnly = true;
        txtTree.ScrollBars = ScrollBars.Both;
        txtTree.WordWrap = false;

        SetupTooltips();
    }

    private void SetupTooltips()
    {
        var toolTip = new ToolTip();
        toolTip.SetToolTip(btnBrowseProject, "Выбрать папку проекта");
        toolTip.SetToolTip(btnBrowseOutput, "Выбрать выходную папку");
        toolTip.SetToolTip(btnStartScan, "Начать сканирование проекта");
        toolTip.SetToolTip(btnCancel, "Отменить операцию");
    }

    private void SetDefaultPaths()
    {
        var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var defaultProjectPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "source", "repos", "SpectrumNet", "SpectrumNet");
        var defaultOutputDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            _config.DefaultOutputFolderName);

        var projectRoot = _pathResolver.FindProjectRoot(
            currentDirectory,
            _config.DefaultProjectName);
        txtProjectPath.Text = projectRoot ?? defaultProjectPath;
        txtOutputDirectory.Text = defaultOutputDirectory;

        statusLabel.Text = "Готов к работе";
        GenerateProjectTree();
    }

    private void BtnBrowseProject_Click(object? sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = "Выберите корневую папку проекта",
            SelectedPath = txtProjectPath.Text,
            UseDescriptionForTitle = true
        };

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            txtProjectPath.Text = dialog.SelectedPath;
            statusLabel.Text = "Папка проекта обновлена";
            GenerateProjectTree();
        }
    }

    private void BtnBrowseOutput_Click(object? sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = "Выберите папку для сохранения файлов",
            SelectedPath = txtOutputDirectory.Text,
            UseDescriptionForTitle = true
        };

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            txtOutputDirectory.Text = dialog.SelectedPath;
            statusLabel.Text = "Выходная папка обновлена";
        }
    }

    private async void BtnStartScan_Click(object? sender, EventArgs e)
    {
        if (!ValidateInputs())
            return;

        _cancellationTokenSource = new CancellationTokenSource();

        try
        {
            SetScanningState(true);
            txtLog.Clear();

            statusLabel.Text = "Сканирование в процессе...";
            _logger.LogInformation("=== Запуск сканирования проекта ===");

            await _fileScanner.ScanAndGenerateAsync(
                txtProjectPath.Text.Trim(),
                txtOutputDirectory.Text.Trim(),
                _cancellationTokenSource.Token);

            statusLabel.Text = "Сканирование завершено успешно";
            fileCountLabel.Text = "Готово!";
            _logger.LogInformation("=== Сканирование успешно завершено ===");
        }
        catch (OperationCanceledException)
        {
            statusLabel.Text = "Сканирование отменено";
            _logger.LogWarning("Сканирование было отменено пользователем");
        }
        catch (Exception ex)
        {
            statusLabel.Text = "Ошибка сканирования";
            _logger.LogError(ex, "Произошла ошибка во время сканирования");
        }
        finally
        {
            SetScanningState(false);
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }
    }

    private void BtnCancel_Click(object? sender, EventArgs e)
    {
        _cancellationTokenSource?.Cancel();
        statusLabel.Text = "Отмена операции...";
    }

    private void GenerateProjectTree()
    {
        try
        {
            var projectPath = txtProjectPath.Text.Trim();
            if (!string.IsNullOrEmpty(projectPath))
            {
                txtTree.Text = _treeGenerator.GenerateDirectoryTree(projectPath);
                var fileCount = CountFiles(projectPath);
                fileCountLabel.Text = $"Файлов: {fileCount}";
            }
        }
        catch (Exception ex)
        {
            txtTree.Text = $"Ошибка генерации дерева: {ex.Message}";
            fileCountLabel.Text = "Ошибка";
        }
    }

    private static int CountFiles(string path)
    {
        try
        {
            return Directory.GetFiles(path, "*", SearchOption.AllDirectories).Length;
        }
        catch
        {
            return 0;
        }
    }

    private bool ValidateInputs()
    {
        var projectPath = txtProjectPath.Text.Trim();
        var outputPath = txtOutputDirectory.Text.Trim();

        if (string.IsNullOrWhiteSpace(projectPath))
        {
            ShowError("Пожалуйста, укажите путь к проекту");
            return false;
        }

        if (string.IsNullOrWhiteSpace(outputPath))
        {
            ShowError("Пожалуйста, укажите выходную директорию");
            return false;
        }

        if (!Directory.Exists(projectPath))
        {
            ShowError($"Указанная директория проекта не существует:\n{projectPath}");
            return false;
        }

        return true;
    }

    private void ShowError(string message)
    {
        MessageBox.Show(
            message,
            "Ошибка валидации",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);
        statusLabel.Text = "Ошибка валидации";
    }

    private void SetScanningState(bool isScanning)
    {
        btnStartScan.Enabled = !isScanning;
        btnBrowseProject.Enabled = !isScanning;
        btnBrowseOutput.Enabled = !isScanning;
        txtProjectPath.Enabled = !isScanning;
        txtOutputDirectory.Enabled = !isScanning;
        btnCancel.Visible = isScanning;
        progressBar.Visible = isScanning;

        btnStartScan.Text = isScanning ? "⏳ Обработка..." : "🚀 Старт";

        if (isScanning)
            progressBar.Style = ProgressBarStyle.Marquee;
        else
            progressBar.Style = ProgressBarStyle.Continuous;
    }

    private void Button_MouseEnter(object? sender, EventArgs e)
    {
        if (sender is Button btn && btn.Enabled)
        {
            var originalColor = btn.BackColor;
            btn.BackColor = ControlPaint.Light(originalColor, 0.2f);
        }
    }

    private void Button_MouseLeave(object? sender, EventArgs e)
    {
        if (sender is Button btn)
        {
            btn.BackColor = btn.Name switch
            {
                "btnStartScan" => Color.FromArgb(16, 137, 62),
                "btnCancel" => Color.FromArgb(196, 43, 28),
                _ => Color.FromArgb(0, 120, 215)
            };
        }
    }

    private void HeaderPanel_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        // Рисуем логотип
        using var brush = new SolidBrush(Color.FromArgb(0, 120, 215));
        g.FillEllipse(brush, 15, 10, 25, 25);

        using var textBrush = new SolidBrush(Color.White);
        using var font = new Font("Segoe UI", 9F, FontStyle.Bold);
        var rect = new Rectangle(15, 10, 25, 25);
        var format = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };
        g.DrawString("FS", font, textBrush, rect, format);

        // Заголовок приложения
        using var titleBrush = new SolidBrush(Color.White);
        using var titleFont = new Font("Segoe UI", 11F, FontStyle.Bold);
        g.DrawString(
            "FileScanner - Генератор содержимого проекта v1.0",
            titleFont,
            titleBrush,
            50,
            14);
    }

    private static Icon CreateApplicationIcon()
    {
        var bitmap = new Bitmap(32, 32);
        using var g = Graphics.FromImage(bitmap);
        g.SmoothingMode = SmoothingMode.AntiAlias;

        using var brush = new SolidBrush(Color.FromArgb(0, 120, 215));
        g.FillEllipse(brush, 2, 2, 28, 28);

        using var textBrush = new SolidBrush(Color.White);
        using var font = new Font("Segoe UI", 10F, FontStyle.Bold);
        var rect = new Rectangle(0, 0, 32, 32);
        var format = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };
        g.DrawString("FS", font, textBrush, rect, format);

        return Icon.FromHandle(bitmap.GetHicon());
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _cancellationTokenSource?.Cancel();
        base.OnFormClosing(e);
    }
}