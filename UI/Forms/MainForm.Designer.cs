#nullable enable

namespace FileScanner.UI.Forms;

partial class MainForm
{
    private IContainer? components = null;

    private Panel headerPanel = null!;
    private Panel mainPanel = null!;
    private GroupBox projectGroupBox = null!;
    private GroupBox outputGroupBox = null!;
    private Panel buttonPanel = null!;
    private Label label1 = null!;
    private Label label2 = null!;
    private Label label3 = null!;
    private TextBox txtProjectPath = null!;
    private TextBox txtOutputDirectory = null!;
    private TextBox txtTree = null!;
    private TextBox txtLog = null!;
    private Button btnBrowseProject = null!;
    private Button btnBrowseOutput = null!;
    private Button btnStartScan = null!;
    private Button btnCancel = null!;
    private SplitContainer splitContainer = null!;
    private ProgressBar progressBar = null!;
    private StatusStrip statusStrip = null!;
    private ToolStripStatusLabel statusLabel = null!;
    private ToolStripStatusLabel fileCountLabel = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
            components.Dispose();
        base.Dispose(disposing);
    }

    private void SetupUI()
    {
        ConfigureTextBoxes();
        SetupTooltips();
        SetupEventHandlers();
    }

    private void ConfigureTextBoxes()
    {
        ConfigureLogTextBox();
        ConfigureTreeTextBox();
    }

    private void ConfigureLogTextBox()
    {
        txtLog.ReadOnly = true;
        txtLog.ScrollBars = ScrollBars.Vertical;
        txtLog.WordWrap = true;
        txtLog.Font = new Font("Cascadia Code", 8F);
        FormLoggerProvider.SetLogTextBox(txtLog);
    }

    private void ConfigureTreeTextBox()
    {
        txtTree.ReadOnly = true;
        txtTree.ScrollBars = ScrollBars.Both;
        txtTree.WordWrap = false;
        txtTree.Font = new Font("Cascadia Code", 9F);
    }

    private void SetupTooltips()
    {
        var toolTip = CreateToolTip();
        ApplyTooltips(toolTip);
    }

    private static ToolTip CreateToolTip() => new()
    {
        AutoPopDelay = 5000,
        InitialDelay = 500,
        ReshowDelay = 100,
        ShowAlways = true
    };

    private void ApplyTooltips(ToolTip toolTip)
    {
        toolTip.SetToolTip(btnBrowseProject, "Select project folder");
        toolTip.SetToolTip(btnBrowseOutput, "Select output folder");
        toolTip.SetToolTip(btnStartScan, "Start project scan");
        toolTip.SetToolTip(btnCancel, "Cancel operation");
        toolTip.SetToolTip(txtProjectPath, "Path to project root folder");
        toolTip.SetToolTip(txtOutputDirectory, "Folder for saving scan results");
    }

    private void SetupEventHandlers()
    {
        SetupButtonEventHandlers();
        headerPanel.Paint += HeaderPanel_Paint;
    }

    private void SetupButtonEventHandlers()
    {
        btnBrowseProject.MouseEnter += Button_MouseEnter;
        btnBrowseProject.MouseLeave += Button_MouseLeave;
        btnBrowseOutput.MouseEnter += Button_MouseEnter;
        btnBrowseOutput.MouseLeave += Button_MouseLeave;
        btnStartScan.MouseEnter += Button_MouseEnter;
        btnStartScan.MouseLeave += Button_MouseLeave;
        btnCancel.MouseEnter += Button_MouseEnter;
        btnCancel.MouseLeave += Button_MouseLeave;
    }

    private void Button_MouseEnter(object? sender, EventArgs e)
    {
        if (sender is Button btn && btn.Enabled)
        {
            ApplyHoverEffect(btn);
        }
    }

    private static void ApplyHoverEffect(Button button)
    {
        button.Cursor = Cursors.Hand;
        button.BackColor = ControlPaint.Light(button.BackColor, 0.2f);
    }

    private void Button_MouseLeave(object? sender, EventArgs e)
    {
        if (sender is Button btn)
        {
            RestoreButtonAppearance(btn);
        }
    }

    private static void RestoreButtonAppearance(Button button)
    {
        button.Cursor = Cursors.Default;
        button.BackColor = GetButtonDefaultColor(button.Name);
    }

    private static Color GetButtonDefaultColor(string buttonName) => buttonName switch
    {
        "btnStartScan" => Color.FromArgb(16, 137, 62),
        "btnCancel" => Color.FromArgb(196, 43, 28),
        _ => Color.FromArgb(0, 120, 215)
    };

    private void HeaderPanel_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        DrawHeaderBackground(g);
        DrawLogo(g);
        DrawTitle(g);
        DrawSubtitle(g);
    }

    private void DrawHeaderBackground(Graphics g)
    {
        using var brush = new LinearGradientBrush(
            headerPanel.ClientRectangle,
            Color.FromArgb(28, 28, 28),
            Color.FromArgb(45, 45, 45),
            LinearGradientMode.Horizontal);
        g.FillRectangle(brush, headerPanel.ClientRectangle);
    }

    private static void DrawLogo(Graphics g)
    {
        DrawLogoBackground(g);
        DrawLogoText(g);
    }

    private static void DrawLogoBackground(Graphics g)
    {
        using var logoBrush = new LinearGradientBrush(
            new Rectangle(15, 10, 40, 40),
            Color.FromArgb(0, 122, 204),
            Color.FromArgb(0, 88, 156),
            LinearGradientMode.Vertical);
        g.FillEllipse(logoBrush, 15, 10, 40, 40);
    }

    private static void DrawLogoText(Graphics g)
    {
        using var textBrush = new SolidBrush(Color.White);
        using var logoFont = new Font("Segoe UI", 14F, FontStyle.Bold);
        var logoRect = new Rectangle(15, 10, 40, 40);
        var format = CreateCenterFormat();
        g.DrawString("FS", logoFont, textBrush, logoRect, format);
    }

    private static StringFormat CreateCenterFormat() => new()
    {
        Alignment = StringAlignment.Center,
        LineAlignment = StringAlignment.Center
    };

    private static void DrawTitle(Graphics g)
    {
        using var titleBrush = new SolidBrush(Color.White);
        using var titleFont = new Font("Segoe UI", 14F, FontStyle.Bold);
        g.DrawString("FileScanner", titleFont, titleBrush, 65, 12);
    }

    private static void DrawSubtitle(Graphics g)
    {
        using var subtitleFont = new Font("Segoe UI", 9F);
        using var subtitleBrush = new SolidBrush(Color.FromArgb(180, 180, 180));
        g.DrawString(
            "Project Content Generator v1.0",
            subtitleFont,
            subtitleBrush,
            65,
            32);
    }

    private void InitializeComponent()
    {
        CreateControls();
        ConfigureForm();
        ConfigureControls();
        LayoutControls();
    }

    private void CreateControls()
    {
        headerPanel = new Panel();
        mainPanel = new Panel();
        projectGroupBox = new GroupBox();
        outputGroupBox = new GroupBox();
        buttonPanel = new Panel();
        label1 = new Label();
        label2 = new Label();
        label3 = new Label();
        txtProjectPath = new TextBox();
        txtOutputDirectory = new TextBox();
        txtTree = new TextBox();
        txtLog = new TextBox();
        btnBrowseProject = new Button();
        btnBrowseOutput = new Button();
        btnStartScan = new Button();
        btnCancel = new Button();
        splitContainer = new SplitContainer();
        progressBar = new ProgressBar();
        statusStrip = new StatusStrip();
        statusLabel = new ToolStripStatusLabel();
        fileCountLabel = new ToolStripStatusLabel();
    }

    private void ConfigureForm()
    {
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(248, 249, 250);
        ClientSize = new Size(914, 620);
        Font = new Font("Segoe UI", 9F);
        Icon = CreateApplicationIcon();
        Margin = new Padding(3, 4, 3, 4);
        MinimumSize = new Size(912, 604);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "FileScanner v1.0 - Project Content Generator";
    }

    private void ConfigureControls()
    {
        ConfigureHeaderPanel();
        ConfigureMainPanel();
        ConfigureProjectGroupBox();
        ConfigureOutputGroupBox();
        ConfigureButtonPanel();
        ConfigureProgressBar();
        ConfigureStructureLabel();
        ConfigureSplitContainer();
        ConfigureStatusStrip();
    }

    private void ConfigureHeaderPanel()
    {
        headerPanel.BackColor = Color.FromArgb(32, 32, 32);
        headerPanel.Dock = DockStyle.Top;
        headerPanel.Location = new Point(0, 0);
        headerPanel.Margin = new Padding(3, 4, 3, 4);
        headerPanel.Name = "headerPanel";
        headerPanel.Size = new Size(914, 60);
        headerPanel.TabIndex = 0;
    }

    private void ConfigureMainPanel()
    {
        mainPanel.BackColor = Color.FromArgb(248, 249, 250);
        mainPanel.Dock = DockStyle.Fill;
        mainPanel.Location = new Point(0, 60);
        mainPanel.Margin = new Padding(3, 4, 3, 4);
        mainPanel.Name = "mainPanel";
        mainPanel.Padding = new Padding(17, 20, 17, 20);
        mainPanel.Size = new Size(914, 534);
        mainPanel.TabIndex = 1;
    }

    private void ConfigureProjectGroupBox()
    {
        ConfigureGroupBox(
            projectGroupBox,
            "📁 Project Path",
            new Point(17, 20));

        ConfigureLabel(
            label1,
            "Select project folder",
            new Point(14, 29));

        ConfigureTextBox(
            txtProjectPath,
            new Point(14, 51));

        ConfigureBrowseButton(
            btnBrowseProject,
            new Point(838, 51),
            BtnBrowseProject_Click);
    }

    private void ConfigureOutputGroupBox()
    {
        ConfigureGroupBox(
            outputGroupBox,
            "💾 Output Folder",
            new Point(17, 115));

        ConfigureLabel(
            label2,
            "Folder for saving files",
            new Point(14, 29));

        ConfigureTextBox(
            txtOutputDirectory,
            new Point(14, 51));

        ConfigureBrowseButton(
            btnBrowseOutput,
            new Point(838, 51),
            BtnBrowseOutput_Click);
    }

    private void ConfigureGroupBox(
        GroupBox groupBox,
        string text,
        Point location)
    {
        groupBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        groupBox.BackColor = Color.White;
        groupBox.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        groupBox.ForeColor = Color.FromArgb(64, 64, 64);
        groupBox.Location = location;
        groupBox.Margin = new Padding(3, 4, 3, 4);
        groupBox.Name = $"{text.ToLower()}GroupBox";
        groupBox.Padding = new Padding(14, 16, 14, 16);
        groupBox.Size = new Size(880, 87);
        groupBox.TabIndex = 0;
        groupBox.TabStop = false;
        groupBox.Text = text;
    }

    private static void ConfigureLabel(
        Label label,
        string text,
        Point location)
    {
        label.AutoSize = true;
        label.Font = new Font("Segoe UI", 8F);
        label.ForeColor = Color.FromArgb(128, 128, 128);
        label.Location = location;
        label.Name = $"label{text.GetHashCode()}";
        label.Size = new Size(130, 19);
        label.TabIndex = 0;
        label.Text = text;
    }

    private static void ConfigureTextBox(
        TextBox textBox,
        Point location)
    {
        textBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        textBox.BackColor = Color.FromArgb(245, 245, 245);
        textBox.BorderStyle = BorderStyle.FixedSingle;
        textBox.Font = new Font("Consolas", 8.5F);
        textBox.ForeColor = Color.FromArgb(32, 32, 32);
        textBox.Location = location;
        textBox.Margin = new Padding(3, 4, 3, 4);
        textBox.Name = $"txt{location.Y}";
        textBox.Size = new Size(820, 24);
        textBox.TabIndex = 1;
    }

    private void ConfigureBrowseButton(
    Button button,
    Point location,
    EventHandler clickHandler)
    {
        button.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        button.BackColor = Color.FromArgb(0, 120, 215);
        button.FlatAppearance.BorderSize = 0;
        button.FlatStyle = FlatStyle.Flat;
        button.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
        button.ForeColor = Color.White;
        button.Location = location;
        button.Margin = new Padding(3, 4, 3, 4);
        button.Name = $"btn{location.Y}";
        button.Size = new Size(28, 24);
        button.TabIndex = 2;
        button.Text = "📂";
        button.UseVisualStyleBackColor = false;
        button.Click += clickHandler;
    }

    private void ConfigureButtonPanel()
    {
        buttonPanel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        buttonPanel.Location = new Point(763, 210);
        buttonPanel.Name = "buttonPanel";
        buttonPanel.Size = new Size(134, 43);
        buttonPanel.TabIndex = 2;

        ConfigureStartButton();
        ConfigureCancelButton();
    }

    private void ConfigureStartButton()
    {
        btnStartScan.BackColor = Color.FromArgb(16, 137, 62);
        btnStartScan.Dock = DockStyle.Fill;
        btnStartScan.FlatAppearance.BorderSize = 0;
        btnStartScan.FlatStyle = FlatStyle.Flat;
        btnStartScan.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnStartScan.ForeColor = Color.White;
        btnStartScan.Location = new Point(0, 0);
        btnStartScan.Margin = new Padding(3, 4, 3, 4);
        btnStartScan.Name = "btnStartScan";
        btnStartScan.Size = new Size(134, 43);
        btnStartScan.TabIndex = 0;
        btnStartScan.Text = "🚀 Start";
        btnStartScan.UseVisualStyleBackColor = false;
        btnStartScan.Click += BtnStartScan_Click;
    }

    private void ConfigureCancelButton()
    {
        btnCancel.BackColor = Color.FromArgb(196, 43, 28);
        btnCancel.Dock = DockStyle.Right;
        btnCancel.FlatAppearance.BorderSize = 0;
        btnCancel.FlatStyle = FlatStyle.Flat;
        btnCancel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnCancel.ForeColor = Color.White;
        btnCancel.Location = new Point(70, 0);
        btnCancel.Margin = new Padding(3, 4, 3, 4);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(64, 43);
        btnCancel.TabIndex = 1;
        btnCancel.Text = "✕ Stop";
        btnCancel.UseVisualStyleBackColor = false;
        btnCancel.Visible = false;
        btnCancel.Click += BtnCancel_Click;
    }

    private void ConfigureProgressBar()
    {
        progressBar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        progressBar.Location = new Point(17, 260);
        progressBar.Margin = new Padding(3, 4, 3, 4);
        progressBar.Name = "progressBar";
        progressBar.Size = new Size(880, 6);
        progressBar.Style = ProgressBarStyle.Marquee;
        progressBar.TabIndex = 3;
        progressBar.Visible = false;
    }

    private void ConfigureStructureLabel()
    {
        label3.AutoSize = true;
        label3.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        label3.ForeColor = Color.FromArgb(64, 64, 64);
        label3.Location = new Point(17, 276);
        label3.Name = "label3";
        label3.Size = new Size(139, 20);
        label3.TabIndex = 4;
        label3.Text = "🌳 Project Structure";
    }

    private void ConfigureSplitContainer()
    {
        ConfigureSplitContainerBase();
        ConfigureTreePanel();
        ConfigureLogPanel();
    }

    private void ConfigureSplitContainerBase()
    {
        splitContainer.Anchor = AnchorStyles.Top |
                               AnchorStyles.Bottom |
                               AnchorStyles.Left |
                               AnchorStyles.Right;
        splitContainer.BackColor = Color.FromArgb(240, 240, 240);
        splitContainer.Location = new Point(17, 303);
        splitContainer.Margin = new Padding(3, 4, 3, 4);
        splitContainer.Name = "splitContainer";
        splitContainer.Orientation = Orientation.Horizontal;
        splitContainer.Size = new Size(880, 205);
        splitContainer.SplitterDistance = 145;
        splitContainer.SplitterWidth = 6;
        splitContainer.TabIndex = 5;
    }

    private void ConfigureTreePanel()
    {
        splitContainer.Panel1.BackColor = Color.White;
        splitContainer.Panel1.Padding = new Padding(2);

        txtTree.BackColor = Color.White;
        txtTree.BorderStyle = BorderStyle.None;
        txtTree.Dock = DockStyle.Fill;
        txtTree.Font = new Font("Cascadia Code", 9F);
        txtTree.ForeColor = Color.FromArgb(32, 32, 32);
        txtTree.Location = new Point(2, 2);
        txtTree.Margin = new Padding(3, 4, 3, 4);
        txtTree.Multiline = true;
        txtTree.Name = "txtTree";
        txtTree.ScrollBars = ScrollBars.Both;
        txtTree.Size = new Size(876, 141);
        txtTree.TabIndex = 0;
    }

    private void ConfigureLogPanel()
    {
        splitContainer.Panel2.BackColor = Color.FromArgb(40, 40, 40);
        splitContainer.Panel2.Padding = new Padding(2);

        txtLog.BackColor = Color.FromArgb(30, 30, 30);
        txtLog.BorderStyle = BorderStyle.None;
        txtLog.Dock = DockStyle.Fill;
        txtLog.Font = new Font("Cascadia Code", 8F);
        txtLog.ForeColor = Color.FromArgb(220, 220, 220);
        txtLog.Location = new Point(2, 2);
        txtLog.Margin = new Padding(3, 4, 3, 4);
        txtLog.Multiline = true;
        txtLog.Name = "txtLog";
        txtLog.ScrollBars = ScrollBars.Vertical;
        txtLog.Size = new Size(876, 52);
        txtLog.TabIndex = 0;
    }

    private void ConfigureStatusStrip()
    {
        statusStrip.BackColor = Color.FromArgb(32, 32, 32);
        statusStrip.ImageScalingSize = new Size(20, 20);
        statusStrip.Items.AddRange(new ToolStripItem[] {
            statusLabel,
            fileCountLabel
        });
        statusStrip.Location = new Point(0, 594);
        statusStrip.Name = "statusStrip";
        statusStrip.Padding = new Padding(1, 0, 16, 0);
        statusStrip.Size = new Size(914, 26);
        statusStrip.TabIndex = 2;

        ConfigureStatusLabels();
    }

    private void ConfigureStatusLabels()
    {
        statusLabel.ForeColor = Color.White;
        statusLabel.Name = "statusLabel";
        statusLabel.Size = new Size(112, 20);
        statusLabel.Text = "Ready";

        fileCountLabel.ForeColor = Color.FromArgb(180, 180, 180);
        fileCountLabel.Name = "fileCountLabel";
        fileCountLabel.Size = new Size(785, 20);
        fileCountLabel.Spring = true;
        fileCountLabel.TextAlign = ContentAlignment.MiddleRight;
    }

    private void LayoutControls()
    {
        SuspendLayoutForAllControls();
        AddControlsToContainers();
        ResumeLayoutForAllControls();
    }

    private void SuspendLayoutForAllControls()
    {
        headerPanel.SuspendLayout();
        mainPanel.SuspendLayout();
        projectGroupBox.SuspendLayout();
        outputGroupBox.SuspendLayout();
        buttonPanel.SuspendLayout();
        ((ISupportInitialize)splitContainer).BeginInit();
        splitContainer.Panel1.SuspendLayout();
        splitContainer.Panel2.SuspendLayout();
        splitContainer.SuspendLayout();
        statusStrip.SuspendLayout();
        SuspendLayout();
    }

    private void AddControlsToContainers()
    {
        projectGroupBox.Controls.Add(btnBrowseProject);
        projectGroupBox.Controls.Add(txtProjectPath);
        projectGroupBox.Controls.Add(label1);

        outputGroupBox.Controls.Add(btnBrowseOutput);
        outputGroupBox.Controls.Add(txtOutputDirectory);
        outputGroupBox.Controls.Add(label2);

        buttonPanel.Controls.Add(btnCancel);
        buttonPanel.Controls.Add(btnStartScan);

        splitContainer.Panel1.Controls.Add(txtTree);
        splitContainer.Panel2.Controls.Add(txtLog);

        mainPanel.Controls.Add(projectGroupBox);
        mainPanel.Controls.Add(outputGroupBox);
        mainPanel.Controls.Add(buttonPanel);
        mainPanel.Controls.Add(progressBar);
        mainPanel.Controls.Add(label3);
        mainPanel.Controls.Add(splitContainer);

        Controls.Add(mainPanel);
        Controls.Add(headerPanel);
        Controls.Add(statusStrip);
    }

    private void ResumeLayoutForAllControls()
    {
        headerPanel.ResumeLayout(false);
        mainPanel.ResumeLayout(false);
        mainPanel.PerformLayout();
        projectGroupBox.ResumeLayout(false);
        projectGroupBox.PerformLayout();
        outputGroupBox.ResumeLayout(false);
        outputGroupBox.PerformLayout();
        buttonPanel.ResumeLayout(false);
        splitContainer.Panel1.ResumeLayout(false);
        splitContainer.Panel1.PerformLayout();
        splitContainer.Panel2.ResumeLayout(false);
        splitContainer.Panel2.PerformLayout();
        ((ISupportInitialize)splitContainer).EndInit();
        splitContainer.ResumeLayout(false);
        statusStrip.ResumeLayout(false);
        statusStrip.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    private static Icon CreateApplicationIcon()
    {
        var bitmap = CreateIconBitmap();
        return Icon.FromHandle(bitmap.GetHicon());
    }

    private static Bitmap CreateIconBitmap()
    {
        var bitmap = new Bitmap(32, 32);
        using var g = Graphics.FromImage(bitmap);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(Color.Transparent);

        DrawIconBackground(g);
        DrawIconText(g);

        return bitmap;
    }

    private static void DrawIconBackground(Graphics g)
    {
        using var brush = new SolidBrush(Color.FromArgb(0, 120, 215));
        g.FillEllipse(brush, 2, 2, 28, 28);
    }

    private static void DrawIconText(Graphics g)
    {
        using var textBrush = new SolidBrush(Color.White);
        using var font = new Font("Segoe UI", 12F, FontStyle.Bold);
        var rect = new Rectangle(0, 0, 32, 32);
        var format = CreateCenterFormat();
        g.DrawString("FS", font, textBrush, rect, format);
    }
}