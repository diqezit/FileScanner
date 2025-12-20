namespace FileScanner.UI;

public sealed class MainController
{
    readonly MainForm _v;
    readonly ScanEngine _eng;
    readonly ProjectAnalyzer _az;
    readonly SettingsManager _cfg;
    readonly ScannerConfiguration _sc;

    public MainController(
        MainForm v,
        ScanEngine eng,
        ProjectAnalyzer az,
        SettingsManager cfg,
        ScannerConfiguration sc)
    {
        _v = v;
        _eng = eng;
        _az = az;
        _cfg = cfg;
        _sc = sc;

        Wire();
    }

    void Wire()
    {
        _v.LoadForm += (_, _) =>
        {
            LoadSettings();
            UpdatePreview();
        };

        _v.StartScanClick += (_, _) => Start();
        _v.CancelScanClick += (_, _) => _eng.Cancel();
        _v.BrowseProjectClick += (_, _) => BrowseProject();
        _v.BrowseOutputClick += (_, _) => BrowseOutput();
        _v.UseFiltersChanged += (_, _) => UpdatePreview();
        _v.FormClosingEvent += OnClosing;

        _eng.Started += (_, _) =>
        {
            _v.SetScanning(true);
            _v.Status = "Scanning...";
        };

        _eng.Done += (_, e) => OnDone(e);

        _eng.Cancelled += (_, _) =>
        {
            _v.SetScanning(false);
            _v.Status = "Cancelled";
            _v.Stats = "";
        };

        _eng.Failed += (_, ex) => OnFail(ex);
    }

    void Start()
    {
        if (!Validate()) return;

        var opt = new ScanOptions(
            _v.UseFilters,
            _v.Split,
            _v.ChunkSize);

        _ = _eng.RunAsync(_v.ProjectPath, _v.OutputPath, opt);
    }

    void BrowseProject()
    {
        using var d = new FolderBrowserDialog
        {
            Description = "Select project",
            SelectedPath = _v.ProjectPath
        };

        if (d.ShowDialog() == DialogResult.OK)
        {
            _v.ProjectPath = d.SelectedPath;
            _v.OutputPath = d.SelectedPath;
            UpdatePreview();
        }
    }

    void BrowseOutput()
    {
        using var d = new FolderBrowserDialog
        {
            Description = "Select output",
            SelectedPath = _v.OutputPath,
            ShowNewFolderButton = true
        };

        if (d.ShowDialog() == DialogResult.OK)
            _v.OutputPath = d.SelectedPath;
    }

    void OnClosing(object? _, FormClosingEventArgs e)
    {
        if (_eng.Running)
        {
            var r = MessageBox.Show(
                "Scan running. Close?",
                "Confirm",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (r == DialogResult.Yes)
                _eng.Cancel();
            else
                e.Cancel = true;
        }
        else
        {
            _cfg.Save(_v.ProjectPath, _v.OutputPath);
        }
    }

    void OnDone(ScanDoneArgs e)
    {
        _v.SetScanning(false);
        _v.Status = "Done";
        _v.Stats = $"Time: {e.Elapsed:mm\\:ss}";

        _cfg.Save(_v.ProjectPath, _v.OutputPath);

        var r = MessageBox.Show(
            $"Done!\n\n{e.OutDir}\n\nOpen folder?",
            "Success",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Information);

        if (r == DialogResult.Yes)
            try { Process.Start("explorer.exe", e.OutDir); } catch { }
    }

    void OnFail(Exception ex)
    {
        _v.SetScanning(false);
        _v.Status = "Error";
        _v.Stats = "";

        MessageBox.Show(
            $"Error:\n\n{ex.Message}",
            "Error",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }

    async void UpdatePreview()
    {
        if (string.IsNullOrWhiteSpace(_v.ProjectPath) ||
            !Directory.Exists(_v.ProjectPath))
        {
            _v.Tree = "Select valid project path";
            _v.Stats = "";
            return;
        }

        _v.SetWait(true);

        try
        {
            var s = await Task.Run(() =>
                _az.Enumerate(_v.ProjectPath, _v.UseFilters));

            var tree = await Task.Run(() =>
                _az.GenTree(s, _v.ProjectPath));

            var stats = await _az.CalcStatsAsync(s, CancellationToken.None);

            _v.Tree = tree;
            _v.Stats = $"Files: {stats.Files:N0} | " +
                       $"Dirs: {stats.Dirs:N0} | " +
                       $"Size: {FileSystemServices.FmtSize(stats.Size)}";
        }
        catch (Exception ex)
        {
            _v.Tree = $"Error: {ex.Message}";
            _v.Stats = "";
        }
        finally
        {
            _v.SetWait(false);
        }
    }

    void LoadSettings()
    {
        var s = _cfg.Load();

        _v.ProjectPath = s.ProjectPath ??
            AppDomain.CurrentDomain.BaseDirectory;

        _v.OutputPath = s.OutputPath ?? _v.ProjectPath;
        _v.ChunkText = _sc.DefaultChunk.ToString();
    }

    bool Validate()
    {
        if (string.IsNullOrWhiteSpace(_v.ProjectPath) ||
            !Directory.Exists(_v.ProjectPath))
        {
            MessageBox.Show(
                $"Invalid project:\n{_v.ProjectPath}",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return false;
        }

        if (string.IsNullOrWhiteSpace(_v.OutputPath))
        {
            MessageBox.Show(
                "Specify output",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return false;
        }

        return true;
    }
}