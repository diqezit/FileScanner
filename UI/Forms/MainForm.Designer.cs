#nullable enable

namespace FileScanner.UI.Forms;

partial class MainForm
{
    private IContainer? components = null;
    private Panel mainPanel = null!;
    private Panel projectPanel = null!;
    private Panel outputPanel = null!;
    private Panel buttonPanel = null!;
    private Label structureLabel = null!;
    private TextBox txtProjectPath = null!;
    private TextBox txtOutputDirectory = null!;
    private TextBox txtTree = null!;
    private TextBox txtLog = null!;
    private Button btnBrowseProject = null!;
    private Button btnBrowseOutput = null!;
    private Button btnAction = null!;
    private SplitContainer splitContainer = null!;
    private ProgressBar progressBar = null!;
    private StatusStrip statusStrip = null!;
    private ToolStripStatusLabel statusLabel = null!;
    private ToolStripStatusLabel fileCountLabel = null!;
    private CheckBox chkSplitFile = null!;
    private TextBox txtChunkSize = null!;
    private Label lblChars = null!;
    private Panel exportSettingsPanel = null!;
    private CheckBox chkUseFilters = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
            components.Dispose();

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        SuspendLayout();

        CreateMainPanel();
        CreateProjectInputPanel();
        CreateOutputInputPanel();
        CreateExportSettingsPanel();
        CreateActionButtons();
        CreateProgressBar();
        CreateStructureLabel();
        CreateSplitContainerWithTextBoxes();
        CreateStatusStrip();
        AssembleFormControls();
        ConfigureFormProperties();
        SetupCustomUI();

        ResumeLayout(false);
        PerformLayout();
    }

    private void SetupCustomUI()
    {
        var allControls = new List<Control>
        {
            txtLog,
            txtTree,
            btnBrowseProject,
            btnBrowseOutput,
            btnAction,
            txtProjectPath,
            txtOutputDirectory,
            chkSplitFile,
            chkUseFilters,
            txtChunkSize
        };
        var initializer = new MainFormUIInitializer(this);
        initializer.SetupUI(allControls);
    }

    private void CreateMainPanel()
    {
        mainPanel = new Panel
        {
            BackColor = UITheme.FormBackground,
            Dock = DockStyle.Fill,
            Padding = LayoutDefaults.MediumPadding,
            Location = new Point(0, 0),
            Name = "mainPanel",
            Size = new Size(914, 594)
        };
    }

    private void CreateProjectInputPanel()
    {
        var projectControls = ComponentFactory.CreateInputPanel(
            "Project Path",
            "Select the project's root folder",
            BtnBrowseProject_Click);

        projectPanel = projectControls.Panel;
        txtProjectPath = projectControls.TextBox;
        btnBrowseProject = projectControls.Button;
        txtProjectPath.Name = "txtProjectPath";
        btnBrowseProject.Name = "btnBrowseProject";
        projectPanel.Location = new Point(20, 20);
    }

    private void CreateOutputInputPanel()
    {
        var outputControls = ComponentFactory.CreateInputPanel(
            "Output Folder",
            "Select a folder to save the scan results",
            BtnBrowseOutput_Click);

        outputPanel = outputControls.Panel;
        txtOutputDirectory = outputControls.TextBox;
        btnBrowseOutput = outputControls.Button;
        txtOutputDirectory.Name = "txtOutputDirectory";
        btnBrowseOutput.Name = "btnBrowseOutput";
        outputPanel.Location = new Point(20, 105);
    }

    private void CreateExportSettingsPanel()
    {
        exportSettingsPanel = new Panel
        {
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            Location = new Point(20, 175),
            Size = new Size(700, 55),
            Name = "exportSettingsPanel"
        };

        chkSplitFile = new CheckBox
        {
            Text = "Split output file by character limit",
            Location = new Point(0, 5),
            AutoSize = true,
            ForeColor = UITheme.PrimaryText,
            Name = "chkSplitFile"
        };
        chkSplitFile.CheckedChanged += ChkSplitFile_CheckedChanged;

        txtChunkSize = new TextBox
        {
            Location = new Point(280, 3),
            Size = new Size(120, 27),
            Visible = false,
            Name = "txtChunkSize"
        };

        lblChars = new Label
        {
            Text = "chars per file",
            Location = new Point(405, 7),
            AutoSize = true,
            Visible = false,
            ForeColor = UITheme.SecondaryText,
            Name = "lblChars"
        };

        chkUseFilters = new CheckBox
        {
            Text = "Use project filters (.vcxproj.filters)",
            Location = new Point(0, 30),
            AutoSize = true,
            ForeColor = UITheme.PrimaryText,
            Name = "chkUseFilters"
        };

        exportSettingsPanel.Controls.AddRange(
            [chkSplitFile, txtChunkSize, lblChars, chkUseFilters]);
    }

    private void CreateActionButtons()
    {
        btnAction = ComponentFactory.CreateFlatButton(
            name: "btnAction",
            text: "\uE768 Start",
            font: UITheme.TitleFont,
            backColor: UITheme.AccentColor,
            foreColor: UITheme.LightText,
            textAlign: ContentAlignment.MiddleCenter);
        btnAction.Dock = DockStyle.Fill;
        btnAction.Click += BtnAction_Click;

        buttonPanel = new Panel
        {
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Location = new Point(746, 185),
            Name = "buttonPanel",
            Size = new Size(148, 43)
        };
        buttonPanel.Controls.Add(btnAction);
    }

    private void CreateProgressBar()
    {
        progressBar = new ProgressBar
        {
            Anchor = LayoutDefaults.TopLeftRight,
            Location = new Point(20, 238),
            Name = "progressBar",
            Size = new Size(874, 8),
            Style = ProgressBarStyle.Marquee,
            Visible = false
        };
    }

    private void CreateStructureLabel()
    {
        structureLabel = new Label
        {
            Text = "Project Structure",
            Font = UITheme.TitleFont,
            ForeColor = UITheme.PrimaryText,
            Location = new Point(20, 256),
            Name = "structureLabel",
            AutoSize = true
        };
    }

    private void CreateSplitContainerWithTextBoxes()
    {
        txtTree = ComponentFactory.CreateMultilineTextBox(
            Color.White,
            UITheme.TreeFont,
            UITheme.PrimaryText,
            ScrollBars.Both);
        txtTree.Name = "txtTree";

        txtLog = ComponentFactory.CreateMultilineTextBox(
            UITheme.DarkControlBackground,
            UITheme.LogFont,
            UITheme.MutedLightText,
            ScrollBars.Vertical);
        txtLog.Name = "txtLog";

        splitContainer = new SplitContainer
        {
            Anchor = LayoutDefaults.AllSides,
            BackColor = UITheme.FormBackground,
            Location = new Point(20, 283),
            Name = "splitContainer",
            Orientation = Orientation.Horizontal,
            Size = new Size(874, 285),
            SplitterDistance = 180,
            SplitterWidth = 5
        };

        static void ConfigureSplitPanel(
            SplitterPanel panel,
            Color backColor,
            Control content)
        {
            panel.BackColor = backColor;
            panel.Padding = LayoutDefaults.SmallPadding;
            panel.Controls.Add(content);
        }

        ConfigureSplitPanel(splitContainer.Panel1, Color.White, txtTree);
        ConfigureSplitPanel(
            splitContainer.Panel2,
            Color.FromArgb(40, 40, 40),
            txtLog);
    }

    private void CreateStatusStrip()
    {
        statusLabel = new ToolStripStatusLabel
        {
            Text = "Ready",
            ForeColor = UITheme.LightText
        };

        fileCountLabel = new ToolStripStatusLabel
        {
            ForeColor = UITheme.MutedLightText,
            Spring = true,
            TextAlign = ContentAlignment.MiddleRight
        };

        statusStrip = new StatusStrip
        {
            BackColor = UITheme.StatusStripBackground,
            ImageScalingSize = new Size(20, 20),
            Location = new Point(0, 594),
            Name = "statusStrip",
            Size = new Size(914, 26)
        };

        statusStrip.Items.AddRange(
            [statusLabel, fileCountLabel]);
    }

    private void AssembleFormControls()
    {
        mainPanel.Controls.AddRange(
        [
            projectPanel,
            outputPanel,
            exportSettingsPanel,
            buttonPanel,
            progressBar,
            structureLabel,
            splitContainer
        ]);
        Controls.AddRange([mainPanel, statusStrip]);
    }

    private void ConfigureFormProperties()
    {
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = UITheme.FormBackground;
        ClientSize = new Size(914, 620);
        Font = new Font("Segoe UI", 9F);
        Icon = ComponentFactory.CreateApplicationIcon();
        MinimumSize = new Size(912, 604);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "FileScanner";
        Load += MainForm_Load;
    }
}