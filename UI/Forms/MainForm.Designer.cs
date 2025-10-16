// UI/Forms/MainForm.Designer.cs
#nullable enable

using FileScanner.UI.Forms.MainFormLogic;

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

    private void InitializeComponent()
    {
        SuspendLayout();

        CreateMainPanel();
        CreateInputPanels();
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

    // Decouples custom UI styling from generated layout code
    // Allows easier UI modifications without touching main initializer
    private void SetupCustomUI()
    {
        var allControls = new List<Control>
        {
            txtLog,
            txtTree,
            btnBrowseProject,
            btnBrowseOutput,
            btnStartScan,
            btnCancel,
            txtProjectPath,
            txtOutputDirectory
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

    private void CreateInputPanels()
    {
        var projectControls = ComponentFactory.CreateInputPanel(
            "Project Path",
            "Select the project's root folder",
            BtnBrowseProject_Click
        );
        projectPanel = projectControls.Panel;
        txtProjectPath = projectControls.TextBox;
        btnBrowseProject = projectControls.Button;

        // Name controls for lookup in the UI initializer
        txtProjectPath.Name = "txtProjectPath";
        btnBrowseProject.Name = "btnBrowseProject";
        projectPanel.Location = new Point(20, 20);

        var outputControls = ComponentFactory.CreateInputPanel(
            "Output Folder",
            "Select a folder to save the scan results",
            BtnBrowseOutput_Click
        );
        outputPanel = outputControls.Panel;
        txtOutputDirectory = outputControls.TextBox;
        btnBrowseOutput = outputControls.Button;

        txtOutputDirectory.Name = "txtOutputDirectory";
        btnBrowseOutput.Name = "btnBrowseOutput";
        outputPanel.Location = new Point(20, 105);
    }

    private void CreateActionButtons()
    {
        btnStartScan = ComponentFactory.CreateFlatButton(
            name: "btnStartScan",
            text: "\uE768 Start",
            font: UITheme.TitleFont,
            backColor: UITheme.AccentColor,
            foreColor: UITheme.LightText,
            textAlign: ContentAlignment.MiddleCenter
        );
        btnStartScan.Dock = DockStyle.Fill;
        btnStartScan.Click += BtnStartScan_Click;

        btnCancel = ComponentFactory.CreateFlatButton(
            name: "btnCancel",
            text: "\uE71A Stop",
            font: UITheme.TitleFont,
            backColor: UITheme.DestructiveColor,
            foreColor: UITheme.LightText,
            textAlign: ContentAlignment.MiddleCenter
        );
        btnCancel.Dock = DockStyle.Right;
        btnCancel.Size = new Size(80, 43);

        // Hide stop button until scan starts
        btnCancel.Visible = false;
        btnCancel.Click += BtnCancel_Click;

        buttonPanel = new Panel
        {
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Location = new Point(746, 185),
            Name = "buttonPanel",
            Size = new Size(148, 43)
        };
        buttonPanel.Controls.AddRange(new Control[] { btnStartScan, btnCancel });
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

    // Use a local function to avoid repeating panel setup code
    // It is only used here so it stays local
    private void CreateSplitContainerWithTextBoxes()
    {
        txtTree = ComponentFactory.CreateMultilineTextBox(
            Color.White,
            UITheme.TreeFont,
            UITheme.PrimaryText,
            ScrollBars.Both
        );
        txtTree.Name = "txtTree";

        txtLog = ComponentFactory.CreateMultilineTextBox(
            UITheme.DarkControlBackground,
            UITheme.LogFont,
            UITheme.MutedLightText,
            ScrollBars.Vertical
        );
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

        static void ConfigureSplitPanel(SplitterPanel panel, Color backColor, Control content)
        {
            panel.BackColor = backColor;
            panel.Padding = LayoutDefaults.SmallPadding;
            panel.Controls.Add(content);
        }

        ConfigureSplitPanel(splitContainer.Panel1, Color.White, txtTree);
        ConfigureSplitPanel(splitContainer.Panel2, Color.FromArgb(40, 40, 40), txtLog);
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
        statusStrip.Items.AddRange(new ToolStripItem[] { statusLabel, fileCountLabel });
    }

    private void AssembleFormControls()
    {
        mainPanel.Controls.AddRange(new Control[]
        {
            projectPanel,
            outputPanel,
            buttonPanel,
            progressBar,
            structureLabel,
            splitContainer
        });
        Controls.AddRange(new Control[] { mainPanel, statusStrip });
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
    }
}