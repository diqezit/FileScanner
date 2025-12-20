namespace FileScanner.UI;

public sealed partial class MainForm : Form
{
    readonly MainController _ctl;
    bool _scanning;

    // Controls
    TextBox txtProj = null!, txtOut = null!, txtTree = null!, txtLog = null!, txtChunk = null!;
    Button btnBrowseProj = null!, btnBrowseOut = null!, btnStart = null!;
    CheckBox chkSplit = null!, chkFilters = null!;
    Label lblStatus = null!, lblStats = null!, lblChars = null!;
    ProgressBar progress = null!;
    TabControl tabs = null!;

    #region Properties

    public string ProjectPath
    {
        get => txtProj.Text.Trim();
        set => txtProj.Text = value;
    }

    public string OutputPath
    {
        get => txtOut.Text.Trim();
        set => txtOut.Text = value;
    }

    public string Tree { set => txtTree.Text = value; }
    public string Stats { set => lblStats.Text = value; }
    public string Status { set => lblStatus.Text = value; }
    public string ChunkText { set => txtChunk.Text = value; }

    public bool UseFilters => chkFilters.Checked;
    public bool Split => chkSplit.Checked;

    public int ChunkSize =>
        int.TryParse(txtChunk.Text, out var v) && v > 0 ? v : 0;

    #endregion

    #region Events

    public event EventHandler? LoadForm;
    public event EventHandler? StartScanClick;
    public event EventHandler? CancelScanClick;
    public event EventHandler? BrowseProjectClick;
    public event EventHandler? BrowseOutputClick;
    public event EventHandler? UseFiltersChanged;
    public event EventHandler<FormClosingEventArgs>? FormClosingEvent;

    #endregion

    public MainForm(
        ScanEngine eng,
        ProjectAnalyzer az,
        SettingsManager cfg,
        ScannerConfiguration sc,
        FormLoggerProvider lp)
    {
        InitUI();
        lp.Target = txtLog;
        _ctl = new(this, eng, az, cfg, sc);
    }

    #region Public Methods

    public void SetScanning(bool v)
    {
        _scanning = v;

        txtProj.Enabled = !v;
        txtOut.Enabled = !v;
        btnBrowseProj.Enabled = !v;
        btnBrowseOut.Enabled = !v;
        chkFilters.Enabled = !v;
        chkSplit.Enabled = !v;
        txtChunk.Enabled = !v;

        btnStart.Text = v ? "■ STOP" : "▶ START";
        btnStart.BackColor = v ? C.Danger : C.Primary;

        progress.Visible = v;
        progress.Style = v
            ? ProgressBarStyle.Marquee
            : ProgressBarStyle.Continuous;
    }

    public void SetWait(bool v) =>
        Cursor = v ? Cursors.WaitCursor : Cursors.Default;

    #endregion

    #region Theme

    static class C
    {
        public static readonly Color
            Bg = Color.FromArgb(250, 250, 250),
            Surface = Color.White,
            SurfaceAlt = Color.FromArgb(245, 245, 245),
            Border = Color.FromArgb(224, 224, 224),
            Primary = Color.FromArgb(59, 130, 246),
            Danger = Color.FromArgb(239, 68, 68),
            Text = Color.FromArgb(23, 23, 23),
            Muted = Color.FromArgb(115, 115, 115),
            Dark = Color.FromArgb(24, 24, 27),
            DarkText = Color.FromArgb(161, 161, 170);
    }

    static class F
    {
        public static readonly Font
            Def = new("Segoe UI", 9F),
            Title = new("Segoe UI Semibold", 14F),
            Label = new("Segoe UI Semibold", 9F),
            Mono = new("Cascadia Code", 9F),
            Btn = new("Segoe UI Semibold", 10F);
    }

    #endregion

    #region UI Init

    void InitUI()
    {
        SuspendLayout();

        // Header
        var header = new Panel
        {
            Dock = DockStyle.Top,
            Height = 290,
            BackColor = C.Surface,
            Padding = new(32, 24, 32, 16)
        };

        var title = new Label
        {
            Text = "FileScanner",
            Font = F.Title,
            ForeColor = C.Text,
            Dock = DockStyle.Top,
            Height = 36
        };

        var subtitle = new Label
        {
            Text = "Scan project files and generate unified output",
            Font = F.Def,
            ForeColor = C.Muted,
            Dock = DockStyle.Top,
            Height = 24
        };

        // Input grid
        var grid = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 110,
            ColumnCount = 3,
            RowCount = 2
        };
        grid.ColumnStyles.Add(new(SizeType.Absolute, 80));
        grid.ColumnStyles.Add(new(SizeType.Percent, 100));
        grid.ColumnStyles.Add(new(SizeType.Absolute, 80));
        grid.RowStyles.Add(new(SizeType.Absolute, 52));
        grid.RowStyles.Add(new(SizeType.Absolute, 52));

        grid.Controls.Add(Lbl("Project"), 0, 0);
        txtProj = Txt();
        grid.Controls.Add(txtProj, 1, 0);
        btnBrowseProj = Btn("Browse");
        btnBrowseProj.Click += (s, e) => BrowseProjectClick?.Invoke(s, e);
        grid.Controls.Add(btnBrowseProj, 2, 0);

        grid.Controls.Add(Lbl("Output"), 0, 1);
        txtOut = Txt();
        grid.Controls.Add(txtOut, 1, 1);
        btnBrowseOut = Btn("Browse");
        btnBrowseOut.Click += (s, e) => BrowseOutputClick?.Invoke(s, e);
        grid.Controls.Add(btnBrowseOut, 2, 1);

        // Options
        var opts = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 36,
            Padding = new(76, 4, 0, 0)
        };

        chkFilters = Chk("Use .vcxproj.filters");
        chkFilters.CheckedChanged += (s, e) => UseFiltersChanged?.Invoke(s, e);

        chkSplit = Chk("Split output");
        chkSplit.Margin = new(24, 0, 4, 0);
        chkSplit.CheckedChanged += (_, _) =>
        {
            txtChunk.Visible = lblChars.Visible = chkSplit.Checked;
        };

        txtChunk = new()
        {
            Width = 100,
            Text = "100000",
            Font = F.Def,
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = C.SurfaceAlt,
            Visible = false,
            Margin = new(0, 0, 4, 0)
        };

        lblChars = new()
        {
            Text = "chars",
            Font = F.Def,
            ForeColor = C.Muted,
            AutoSize = true,
            Visible = false,
            Padding = new(0, 4, 0, 0)
        };

        opts.Controls.AddRange([chkFilters, chkSplit, txtChunk, lblChars]);

        // Start button
        btnStart = new()
        {
            Text = "▶ START",
            Dock = DockStyle.Top,
            Height = 44,
            Font = F.Btn,
            FlatStyle = FlatStyle.Flat,
            BackColor = C.Primary,
            ForeColor = Color.White,
            Cursor = Cursors.Hand
        };
        btnStart.FlatAppearance.BorderSize = 0;
        btnStart.Click += (s, e) =>
        {
            if (_scanning)
                CancelScanClick?.Invoke(s, e);
            else
                StartScanClick?.Invoke(s, e);
        };

        // Progress
        progress = new()
        {
            Dock = DockStyle.Top,
            Height = 3,
            Style = ProgressBarStyle.Marquee,
            Visible = false,
            MarqueeAnimationSpeed = 20
        };

        header.Controls.AddRange([progress, btnStart, opts, grid, subtitle, title]);

        // Content
        var content = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = C.Bg,
            Padding = new(32, 16, 32, 16)
        };

        tabs = new() { Dock = DockStyle.Fill, Font = F.Def };

        var tabTree = new TabPage("Structure")
        {
            BackColor = C.Surface,
            Padding = new(12)
        };
        txtTree = new()
        {
            Dock = DockStyle.Fill,
            Multiline = true,
            Font = F.Mono,
            BackColor = C.Surface,
            ForeColor = C.Text,
            BorderStyle = BorderStyle.None,
            ScrollBars = ScrollBars.Both,
            ReadOnly = true,
            WordWrap = false
        };
        tabTree.Controls.Add(txtTree);

        var tabLogs = new TabPage("Logs")
        {
            BackColor = C.Dark,
            Padding = new(12)
        };
        txtLog = new()
        {
            Dock = DockStyle.Fill,
            Multiline = true,
            Font = F.Mono,
            BackColor = C.Dark,
            ForeColor = C.DarkText,
            BorderStyle = BorderStyle.None,
            ScrollBars = ScrollBars.Vertical,
            ReadOnly = true,
            WordWrap = true
        };
        tabLogs.Controls.Add(txtLog);

        tabs.TabPages.AddRange([tabTree, tabLogs]);
        content.Controls.Add(tabs);

        // Footer
        var footer = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 40,
            BackColor = C.Surface,
            Padding = new(32, 10, 32, 10)
        };

        lblStatus = new()
        {
            Text = "Ready",
            Font = F.Def,
            ForeColor = C.Text,
            Dock = DockStyle.Left,
            AutoSize = true
        };

        lblStats = new()
        {
            Text = "",
            Font = F.Def,
            ForeColor = C.Muted,
            Dock = DockStyle.Right,
            AutoSize = true
        };

        footer.Controls.AddRange([lblStatus, lblStats]);

        // Form
        Controls.AddRange([content, header, footer]);

        AutoScaleMode = AutoScaleMode.Dpi;
        BackColor = C.Bg;
        ClientSize = new(960, 720);
        DoubleBuffered = true;
        Font = F.Def;
        MinimumSize = new(800, 600);
        StartPosition = FormStartPosition.CenterScreen;
        Text = "FileScanner";

        Load += (s, e) => LoadForm?.Invoke(s, e);
        FormClosing += (s, e) => FormClosingEvent?.Invoke(s, e);

        try { Icon = MakeIcon(); } catch { }

        ResumeLayout(false);
    }

    static Label Lbl(string t) => new()
    {
        Text = t,
        Font = F.Label,
        ForeColor = C.Text,
        Dock = DockStyle.Fill,
        TextAlign = ContentAlignment.MiddleLeft
    };

    static TextBox Txt() => new()
    {
        Dock = DockStyle.Fill,
        Font = F.Mono,
        BackColor = C.SurfaceAlt,
        BorderStyle = BorderStyle.FixedSingle,
        Margin = new(0, 8, 8, 8)
    };

    static Button Btn(string t)
    {
        var b = new Button
        {
            Text = t,
            Dock = DockStyle.Fill,
            Font = F.Label,
            FlatStyle = FlatStyle.Flat,
            BackColor = C.SurfaceAlt,
            ForeColor = C.Text,
            Cursor = Cursors.Hand,
            Margin = new(0, 8, 0, 8)
        };
        b.FlatAppearance.BorderColor = C.Border;
        b.FlatAppearance.BorderSize = 1;
        return b;
    }

    static CheckBox Chk(string t) => new()
    {
        Text = t,
        Font = F.Def,
        ForeColor = C.Text,
        AutoSize = true,
        Margin = new(0, 2, 0, 0)
    };

    static Icon MakeIcon()
    {
        using var bmp = new Bitmap(32, 32);
        using var g = Graphics.FromImage(bmp);

        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(Color.Transparent);

        using var br = new SolidBrush(C.Primary);
        g.FillRectangle(br, 2, 2, 28, 28);

        using var pen = new Pen(Color.White, 2);
        g.DrawLine(pen, 8, 10, 24, 10);
        g.DrawLine(pen, 8, 16, 20, 16);
        g.DrawLine(pen, 8, 22, 16, 22);

        return Icon.FromHandle(bmp.GetHicon());
    }

    #endregion
}