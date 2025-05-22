// Form1.cs
namespace FileScanner;

public partial class Form1 : Form
{
    private IFileScanner _fileScanner;
    private ILogger _logger;

    public Form1()
    {
        InitializeComponent();
        Console.OutputEncoding = Encoding.UTF8;

        _logger = new FormLogger(txtLog);
        _fileScanner = new ProjectFileScanner(_logger);

        SetupUI();
        SetDefaultPaths();
    }

    private void SetupUI()
    {
        txtLog.ReadOnly = true;
        txtLog.ScrollBars = ScrollBars.Vertical;
        txtLog.WordWrap = false;
    }

    private void SetDefaultPaths()
    {
        string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string defaultProjectPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "source", "repos", "SpectrumNet", "SpectrumNet");
        string defaultOutputDirectory = Path.Combine(currentDirectory, "GeneratedProjectContent");

        string spectrumNetRoot = _fileScanner.FindProjectRoot(currentDirectory, "SpectrumNet.csproj");
        txtProjectPath.Text = !string.IsNullOrEmpty(spectrumNetRoot) ? spectrumNetRoot : defaultProjectPath;
        txtOutputDirectory.Text = defaultOutputDirectory;
    }

    private void btnBrowseProject_Click(object sender, EventArgs e)
    {
        using (var fbd = new FolderBrowserDialog())
        {
            fbd.Description = "Выберите корневую папку вашего проекта";
            fbd.SelectedPath = txtProjectPath.Text;
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                txtProjectPath.Text = fbd.SelectedPath;
            }
        }
    }

    private void btnBrowseOutput_Click(object sender, EventArgs e)
    {
        using (var fbd = new FolderBrowserDialog())
        {
            fbd.Description = "Выберите папку для сохранения сгенерированных файлов";
            fbd.SelectedPath = txtOutputDirectory.Text;
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                txtOutputDirectory.Text = fbd.SelectedPath;
            }
        }
    }

    private async void btnStartScan_Click(object sender, EventArgs e)
    {
        txtLog.Clear();
        _logger.Log("--- Генератор содержимого проекта ---");

        string projectRootDirectory = txtProjectPath.Text;
        string outputDirectory = txtOutputDirectory.Text;

        if (!Directory.Exists(projectRootDirectory))
        {
            _logger.Log($"Ошибка: Указанная папка проекта не существует: {projectRootDirectory}");
            return;
        }

        if (string.IsNullOrWhiteSpace(outputDirectory))
        {
            _logger.Log("Ошибка: Не указана папка для сохранения файлов.");
            return;
        }

        try
        {
            btnStartScan.Enabled = false;
            _logger.Log("Сканирование и генерация файлов...");

            await Task.Run(() => _fileScanner.ScanAndGenerate(projectRootDirectory, outputDirectory));

            _logger.Log("\nГенерация завершена!");
            _logger.Log($"Все файлы сохранены в: {outputDirectory}");
        }
        catch (Exception ex)
        {
            _logger.Log($"Произошла ошибка: {ex.Message}");
            _logger.Log(ex.StackTrace);
        }
        finally
        {
            btnStartScan.Enabled = true;
        }
    }
}