// Form1.Designer.cs
namespace FileScanner; // Изменил на file-scoped namespace

partial class Form1
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        this.label1 = new System.Windows.Forms.Label();
        this.txtProjectPath = new System.Windows.Forms.TextBox();
        this.btnBrowseProject = new System.Windows.Forms.Button();
        this.label2 = new System.Windows.Forms.Label();
        this.txtOutputDirectory = new System.Windows.Forms.TextBox();
        this.btnBrowseOutput = new System.Windows.Forms.Button();
        this.btnStartScan = new System.Windows.Forms.Button();
        this.txtLog = new System.Windows.Forms.TextBox();
        this.SuspendLayout();
        //
        // label1
        //
        this.label1.AutoSize = true;
        this.label1.Location = new System.Drawing.Point(12, 15);
        this.label1.Name = "label1";
        this.label1.Size = new System.Drawing.Size(102, 15);
        this.label1.TabIndex = 0;
        this.label1.Text = "Root Project Path:";
        //
        // txtProjectPath
        //
        this.txtProjectPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
        | System.Windows.Forms.AnchorStyles.Right)));
        this.txtProjectPath.Location = new System.Drawing.Point(120, 12);
        this.txtProjectPath.Name = "txtProjectPath";
        this.txtProjectPath.Size = new System.Drawing.Size(601, 23);
        this.txtProjectPath.TabIndex = 1;
        //
        // btnBrowseProject
        //
        this.btnBrowseProject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.btnBrowseProject.Location = new System.Drawing.Point(727, 11);
        this.btnBrowseProject.Name = "btnBrowseProject";
        this.btnBrowseProject.Size = new System.Drawing.Size(37, 23);
        this.btnBrowseProject.TabIndex = 2;
        this.btnBrowseProject.Text = "...";
        this.btnBrowseProject.UseVisualStyleBackColor = true;
        this.btnBrowseProject.Click += new System.EventHandler(this.btnBrowseProject_Click);
        //
        // label2
        //
        this.label2.AutoSize = true;
        this.label2.Location = new System.Drawing.Point(12, 44);
        this.label2.Name = "label2";
        this.label2.Size = new System.Drawing.Size(95, 15);
        this.label2.TabIndex = 3;
        this.label2.Text = "Output Directory:";
        //
        // txtOutputDirectory
        //
        this.txtOutputDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
        | System.Windows.Forms.AnchorStyles.Right)));
        this.txtOutputDirectory.Location = new System.Drawing.Point(120, 41);
        this.txtOutputDirectory.Name = "txtOutputDirectory";
        this.txtOutputDirectory.Size = new System.Drawing.Size(601, 23);
        this.txtOutputDirectory.TabIndex = 4;
        //
        // btnBrowseOutput
        //
        this.btnBrowseOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.btnBrowseOutput.Location = new System.Drawing.Point(727, 40);
        this.btnBrowseOutput.Name = "btnBrowseOutput";
        this.btnBrowseOutput.Size = new System.Drawing.Size(37, 23);
        this.btnBrowseOutput.TabIndex = 5;
        this.btnBrowseOutput.Text = "...";
        this.btnBrowseOutput.UseVisualStyleBackColor = true;
        this.btnBrowseOutput.Click += new System.EventHandler(this.btnBrowseOutput_Click);
        //
        // btnStartScan
        //
        this.btnStartScan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.btnStartScan.Location = new System.Drawing.Point(673, 70);
        this.btnStartScan.Name = "btnStartScan";
        this.btnStartScan.Size = new System.Drawing.Size(91, 29);
        this.btnStartScan.TabIndex = 6;
        this.btnStartScan.Text = "Start Scan";
        this.btnStartScan.UseVisualStyleBackColor = true;
        this.btnStartScan.Click += new System.EventHandler(this.btnStartScan_Click);
        //
        // txtLog
        //
        this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
        | System.Windows.Forms.AnchorStyles.Left)
        | System.Windows.Forms.AnchorStyles.Right)));
        this.txtLog.Location = new System.Drawing.Point(12, 105);
        this.txtLog.Multiline = true;
        this.txtLog.Name = "txtLog";
        this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        this.txtLog.Size = new System.Drawing.Size(752, 333);
        this.txtLog.TabIndex = 7;
        //
        // Form1
        //
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(776, 450);
        this.Controls.Add(this.txtLog);
        this.Controls.Add(this.btnStartScan);
        this.Controls.Add(this.btnBrowseOutput);
        this.Controls.Add(this.txtOutputDirectory);
        this.Controls.Add(this.label2);
        this.Controls.Add(this.btnBrowseProject);
        this.Controls.Add(this.txtProjectPath);
        this.Controls.Add(this.label1);
        this.Name = "Form1";
        this.Text = "Project File Scanner";
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox txtProjectPath;
    private System.Windows.Forms.Button btnBrowseProject;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox txtOutputDirectory;
    private System.Windows.Forms.Button btnBrowseOutput;
    private System.Windows.Forms.Button btnStartScan;
    private System.Windows.Forms.TextBox txtLog;
}