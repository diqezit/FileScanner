#nullable enable

namespace FileScanner.UI.Forms;

partial class MainForm
{
    private IContainer? components = null;
    private Label label1 = null!;
    private TextBox txtProjectPath = null!;
    private Button btnBrowseProject = null!;
    private Label label2 = null!;
    private TextBox txtOutputDirectory = null!;
    private Button btnBrowseOutput = null!;
    private Button btnStartScan = null!;
    private Button btnCancel = null!;
    private SplitContainer splitContainer = null!;
    private TextBox txtTree = null!;
    private TextBox txtLog = null!;
    private Label label3 = null!;
    private ProgressBar progressBar = null!;
    private StatusStrip statusStrip = null!;
    private ToolStripStatusLabel statusLabel = null!;
    private ToolStripStatusLabel fileCountLabel = null!;
    private Panel headerPanel = null!;
    private Panel mainPanel = null!;
    private GroupBox projectGroupBox = null!;
    private GroupBox outputGroupBox = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        headerPanel = new Panel();
        mainPanel = new Panel();
        projectGroupBox = new GroupBox();
        label1 = new Label();
        txtProjectPath = new TextBox();
        btnBrowseProject = new Button();
        outputGroupBox = new GroupBox();
        label2 = new Label();
        txtOutputDirectory = new TextBox();
        btnBrowseOutput = new Button();
        btnStartScan = new Button();
        btnCancel = new Button();
        progressBar = new ProgressBar();
        label3 = new Label();
        splitContainer = new SplitContainer();
        txtTree = new TextBox();
        txtLog = new TextBox();
        statusStrip = new StatusStrip();
        statusLabel = new ToolStripStatusLabel();
        fileCountLabel = new ToolStripStatusLabel();

        headerPanel.SuspendLayout();
        mainPanel.SuspendLayout();
        projectGroupBox.SuspendLayout();
        outputGroupBox.SuspendLayout();
        ((ISupportInitialize)splitContainer).BeginInit();
        splitContainer.Panel1.SuspendLayout();
        splitContainer.Panel2.SuspendLayout();
        splitContainer.SuspendLayout();
        statusStrip.SuspendLayout();
        SuspendLayout();

        // headerPanel
        headerPanel.BackColor = Color.FromArgb(32, 32, 32);
        headerPanel.Dock = DockStyle.Top;
        headerPanel.Location = new Point(0, 0);
        headerPanel.Margin = new Padding(3, 4, 3, 4);
        headerPanel.Name = "headerPanel";
        headerPanel.Size = new Size(914, 60);
        headerPanel.TabIndex = 0;
        headerPanel.Paint += HeaderPanel_Paint;

        // mainPanel
        mainPanel.BackColor = Color.FromArgb(248, 249, 250);
        mainPanel.Controls.Add(projectGroupBox);
        mainPanel.Controls.Add(outputGroupBox);
        mainPanel.Controls.Add(btnStartScan);
        mainPanel.Controls.Add(btnCancel);
        mainPanel.Controls.Add(progressBar);
        mainPanel.Controls.Add(label3);
        mainPanel.Controls.Add(splitContainer);
        mainPanel.Dock = DockStyle.Fill;
        mainPanel.Location = new Point(0, 60);
        mainPanel.Margin = new Padding(3, 4, 3, 4);
        mainPanel.Name = "mainPanel";
        mainPanel.Padding = new Padding(17, 20, 17, 20);
        mainPanel.Size = new Size(914, 534);
        mainPanel.TabIndex = 1;

        // projectGroupBox
        projectGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        projectGroupBox.BackColor = Color.White;
        projectGroupBox.Controls.Add(label1);
        projectGroupBox.Controls.Add(txtProjectPath);
        projectGroupBox.Controls.Add(btnBrowseProject);
        projectGroupBox.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        projectGroupBox.ForeColor = Color.FromArgb(64, 64, 64);
        projectGroupBox.Location = new Point(17, 20);
        projectGroupBox.Margin = new Padding(3, 4, 3, 4);
        projectGroupBox.Name = "projectGroupBox";
        projectGroupBox.Padding = new Padding(14, 16, 14, 16);
        projectGroupBox.Size = new Size(880, 87);
        projectGroupBox.TabIndex = 0;
        projectGroupBox.TabStop = false;
        projectGroupBox.Text = "📁 Путь к проекту";

        // label1
        label1.AutoSize = true;
        label1.Font = new Font("Segoe UI", 8F);
        label1.ForeColor = Color.FromArgb(128, 128, 128);
        label1.Location = new Point(14, 29);
        label1.Name = "label1";
        label1.Size = new Size(167, 19);
        label1.TabIndex = 0;
        label1.Text = "Выберите папку проекта";

        // txtProjectPath
        txtProjectPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtProjectPath.BackColor = Color.FromArgb(245, 245, 245);
        txtProjectPath.BorderStyle = BorderStyle.FixedSingle;
        txtProjectPath.Font = new Font("Consolas", 8.5F);
        txtProjectPath.ForeColor = Color.FromArgb(32, 32, 32);
        txtProjectPath.Location = new Point(14, 51);
        txtProjectPath.Margin = new Padding(3, 4, 3, 4);
        txtProjectPath.Name = "txtProjectPath";
        txtProjectPath.Size = new Size(811, 24);
        txtProjectPath.TabIndex = 1;

        // btnBrowseProject
        btnBrowseProject.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnBrowseProject.BackColor = Color.FromArgb(0, 120, 215);
        btnBrowseProject.FlatAppearance.BorderSize = 0;
        btnBrowseProject.FlatStyle = FlatStyle.Flat;
        btnBrowseProject.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
        btnBrowseProject.ForeColor = Color.White;
        btnBrowseProject.Location = new Point(834, 49);
        btnBrowseProject.Margin = new Padding(3, 4, 3, 4);
        btnBrowseProject.Name = "btnBrowseProject";
        btnBrowseProject.Size = new Size(32, 31);
        btnBrowseProject.TabIndex = 2;
        btnBrowseProject.Text = "📂";
        btnBrowseProject.UseVisualStyleBackColor = false;
        btnBrowseProject.Click += BtnBrowseProject_Click;
        btnBrowseProject.MouseEnter += Button_MouseEnter;
        btnBrowseProject.MouseLeave += Button_MouseLeave;

        // outputGroupBox
        outputGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        outputGroupBox.BackColor = Color.White;
        outputGroupBox.Controls.Add(label2);
        outputGroupBox.Controls.Add(txtOutputDirectory);
        outputGroupBox.Controls.Add(btnBrowseOutput);
        outputGroupBox.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        outputGroupBox.ForeColor = Color.FromArgb(64, 64, 64);
        outputGroupBox.Location = new Point(17, 120);
        outputGroupBox.Margin = new Padding(3, 4, 3, 4);
        outputGroupBox.Name = "outputGroupBox";
        outputGroupBox.Padding = new Padding(14, 16, 14, 16);
        outputGroupBox.Size = new Size(880, 87);
        outputGroupBox.TabIndex = 1;
        outputGroupBox.TabStop = false;
        outputGroupBox.Text = "💾 Выходная папка";

        // label2
        label2.AutoSize = true;
        label2.Font = new Font("Segoe UI", 8F);
        label2.ForeColor = Color.FromArgb(128, 128, 128);
        label2.Location = new Point(14, 29);
        label2.Name = "label2";
        label2.Size = new Size(202, 19);
        label2.TabIndex = 0;
        label2.Text = "Папка для сохранения файлов";

        // txtOutputDirectory
        txtOutputDirectory.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtOutputDirectory.BackColor = Color.FromArgb(245, 245, 245);
        txtOutputDirectory.BorderStyle = BorderStyle.FixedSingle;
        txtOutputDirectory.Font = new Font("Consolas", 8.5F);
        txtOutputDirectory.ForeColor = Color.FromArgb(32, 32, 32);
        txtOutputDirectory.Location = new Point(14, 51);
        txtOutputDirectory.Margin = new Padding(3, 4, 3, 4);
        txtOutputDirectory.Name = "txtOutputDirectory";
        txtOutputDirectory.Size = new Size(811, 24);
        txtOutputDirectory.TabIndex = 1;

        // btnBrowseOutput
        btnBrowseOutput.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnBrowseOutput.BackColor = Color.FromArgb(0, 120, 215);
        btnBrowseOutput.FlatAppearance.BorderSize = 0;
        btnBrowseOutput.FlatStyle = FlatStyle.Flat;
        btnBrowseOutput.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
        btnBrowseOutput.ForeColor = Color.White;
        btnBrowseOutput.Location = new Point(834, 49);
        btnBrowseOutput.Margin = new Padding(3, 4, 3, 4);
        btnBrowseOutput.Name = "btnBrowseOutput";
        btnBrowseOutput.Size = new Size(32, 31);
        btnBrowseOutput.TabIndex = 2;
        btnBrowseOutput.Text = "📂";
        btnBrowseOutput.UseVisualStyleBackColor = false;
        btnBrowseOutput.Click += BtnBrowseOutput_Click;
        btnBrowseOutput.MouseEnter += Button_MouseEnter;
        btnBrowseOutput.MouseLeave += Button_MouseLeave;

        // btnStartScan
        btnStartScan.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnStartScan.BackColor = Color.FromArgb(16, 137, 62);
        btnStartScan.FlatAppearance.BorderSize = 0;
        btnStartScan.FlatStyle = FlatStyle.Flat;
        btnStartScan.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnStartScan.ForeColor = Color.White;
        btnStartScan.Location = new Point(660, 220);
        btnStartScan.Margin = new Padding(3, 4, 3, 4);
        btnStartScan.Name = "btnStartScan";
        btnStartScan.Size = new Size(137, 43);
        btnStartScan.TabIndex = 2;
        btnStartScan.Text = "🚀 Старт";
        btnStartScan.UseVisualStyleBackColor = false;
        btnStartScan.Click += BtnStartScan_Click;
        btnStartScan.MouseEnter += Button_MouseEnter;
        btnStartScan.MouseLeave += Button_MouseLeave;

        // btnCancel
        btnCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnCancel.BackColor = Color.FromArgb(196, 43, 28);
        btnCancel.FlatAppearance.BorderSize = 0;
        btnCancel.FlatStyle = FlatStyle.Flat;
        btnCancel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnCancel.ForeColor = Color.White;
        btnCancel.Location = new Point(803, 220);
        btnCancel.Margin = new Padding(3, 4, 3, 4);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(80, 43);
        btnCancel.TabIndex = 3;
        btnCancel.Text = "❌ Стоп";
        btnCancel.UseVisualStyleBackColor = false;
        btnCancel.Visible = false;
        btnCancel.Click += BtnCancel_Click;
        btnCancel.MouseEnter += Button_MouseEnter;
        btnCancel.MouseLeave += Button_MouseLeave;

        // progressBar
        progressBar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        progressBar.Location = new Point(17, 273);
        progressBar.Margin = new Padding(3, 4, 3, 4);
        progressBar.Name = "progressBar";
        progressBar.Size = new Size(880, 8);
        progressBar.Style = ProgressBarStyle.Marquee;
        progressBar.TabIndex = 4;
        progressBar.Visible = false;

        // label3
        label3.AutoSize = true;
        label3.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        label3.ForeColor = Color.FromArgb(64, 64, 64);
        label3.Location = new Point(17, 293);
        label3.Name = "label3";
        label3.Size = new Size(168, 20);
        label3.TabIndex = 5;
        label3.Text = "🌳 Структура проекта";

        // splitContainer
        splitContainer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom |
                                 AnchorStyles.Left | AnchorStyles.Right;
        splitContainer.BackColor = Color.FromArgb(220, 220, 220);
        splitContainer.Location = new Point(17, 320);
        splitContainer.Margin = new Padding(3, 4, 3, 4);
        splitContainer.Name = "splitContainer";
        splitContainer.Orientation = Orientation.Horizontal;
        splitContainer.Size = new Size(880, 188);
        splitContainer.SplitterDistance = 134;
        splitContainer.SplitterWidth = 8;
        splitContainer.TabIndex = 6;

        // splitContainer.Panel1
        splitContainer.Panel1.BackColor = Color.White;
        splitContainer.Panel1.Controls.Add(txtTree);
        splitContainer.Panel1.Padding = new Padding(1);

        // splitContainer.Panel2
        splitContainer.Panel2.BackColor = Color.FromArgb(45, 45, 45);
        splitContainer.Panel2.Controls.Add(txtLog);
        splitContainer.Panel2.Padding = new Padding(1);

        // txtTree
        txtTree.BackColor = Color.FromArgb(252, 252, 252);
        txtTree.BorderStyle = BorderStyle.None;
        txtTree.Dock = DockStyle.Fill;
        txtTree.Font = new Font("Cascadia Code", 8F);
        txtTree.ForeColor = Color.FromArgb(32, 32, 32);
        txtTree.Location = new Point(1, 1);
        txtTree.Margin = new Padding(3, 4, 3, 4);
        txtTree.Multiline = true;
        txtTree.Name = "txtTree";
        txtTree.ScrollBars = ScrollBars.Both;
        txtTree.Size = new Size(878, 132);
        txtTree.TabIndex = 0;

        // txtLog
        txtLog.BackColor = Color.FromArgb(30, 30, 30);
        txtLog.BorderStyle = BorderStyle.None;
        txtLog.Dock = DockStyle.Fill;
        txtLog.Font = new Font("Cascadia Code", 7.5F);
        txtLog.ForeColor = Color.FromArgb(220, 220, 220);
        txtLog.Location = new Point(1, 1);
        txtLog.Margin = new Padding(3, 4, 3, 4);
        txtLog.Multiline = true;
        txtLog.Name = "txtLog";
        txtLog.ScrollBars = ScrollBars.Vertical;
        txtLog.Size = new Size(878, 44);
        txtLog.TabIndex = 0;

        // statusStrip
        statusStrip.BackColor = Color.FromArgb(32, 32, 32);
        statusStrip.ImageScalingSize = new Size(20, 20);
        statusStrip.Items.AddRange(new ToolStripItem[] { statusLabel, fileCountLabel });
        statusStrip.Location = new Point(0, 594);
        statusStrip.Name = "statusStrip";
        statusStrip.Padding = new Padding(1, 0, 16, 0);
        statusStrip.Size = new Size(914, 26);
        statusStrip.TabIndex = 2;

        // statusLabel
        statusLabel.ForeColor = Color.White;
        statusLabel.Name = "statusLabel";
        statusLabel.Size = new Size(112, 20);
        statusLabel.Text = "Готов к работе";

        // fileCountLabel
        fileCountLabel.ForeColor = Color.FromArgb(180, 180, 180);
        fileCountLabel.Name = "fileCountLabel";
        fileCountLabel.Size = new Size(785, 20);
        fileCountLabel.Spring = true;
        fileCountLabel.TextAlign = ContentAlignment.MiddleRight;

        // MainForm
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(248, 249, 250);
        ClientSize = new Size(914, 620);
        Controls.Add(mainPanel);
        Controls.Add(headerPanel);
        Controls.Add(statusStrip);
        Font = new Font("Segoe UI", 9F);
        Icon = CreateApplicationIcon();
        Margin = new Padding(3, 4, 3, 4);
        MinimumSize = new Size(912, 604);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "FileScanner v1.0 - Генератор содержимого проекта";

        headerPanel.ResumeLayout(false);
        mainPanel.ResumeLayout(false);
        mainPanel.PerformLayout();
        projectGroupBox.ResumeLayout(false);
        projectGroupBox.PerformLayout();
        outputGroupBox.ResumeLayout(false);
        outputGroupBox.PerformLayout();
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
}