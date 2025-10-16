// UI/Forms/MainFormLogic/MainFormUIInitializer.cs
namespace FileScanner.UI.Forms.MainFormLogic;

#pragma warning disable CS9113 // Parameter is unread.
internal class MainFormUIInitializer(MainForm form)
#pragma warning restore CS9113 // Parameter is unread.
{
    public void SetupUI(IEnumerable<Control> controls)
    {
        var formControls = controls.OfType<Control>().ToArray();

        if (formControls.FirstOrDefault(c => c.Name == "txtLog") is TextBox txtLog)
        {
            ConfigureTextBoxBehavior(
                textBox: txtLog,
                font: UITheme.LogFont,
                scrollBars: ScrollBars.Vertical,
                wordWrap: true
            );
        }

        if (formControls.FirstOrDefault(c => c.Name == "txtTree") is TextBox txtTree)
        {
            ConfigureTextBoxBehavior(
                textBox: txtTree,
                font: UITheme.TreeFont,
                scrollBars: ScrollBars.Both,
                wordWrap: false
            );
        }

        SetupTooltips(formControls);
        AttachButtonHoverEffects([.. formControls.OfType<Button>()]);
    }

    private static void ConfigureTextBoxBehavior(
        TextBox textBox,
        Font font,
        ScrollBars scrollBars,
        bool wordWrap)
    {
        textBox.ReadOnly = true;
        textBox.ScrollBars = scrollBars;
        textBox.WordWrap = wordWrap;
        textBox.Font = font;
    }

    private static void SetupTooltips(IEnumerable<Control> controls)
    {
        var toolTip = new ToolTip
        {
            AutoPopDelay = 5000,
            InitialDelay = 500,
            ReshowDelay = 100,
            ShowAlways = true
        };

        var tooltipMap = new Dictionary<string, string>
        {
            { "btnBrowseProject", "Select project folder" },
            { "btnBrowseOutput", "Select output folder" },
            { "btnStartScan", "Start project scan" },
            { "btnCancel", "Cancel operation" },
            { "txtProjectPath", "Path to project root folder" },
            { "txtOutputDirectory", "Folder for saving scan results" }
        };

        foreach (var control in controls)
        {
            if (tooltipMap.TryGetValue(control.Name, out var text))
            {
                toolTip.SetToolTip(control, text);
            }
        }
    }

    private void AttachButtonHoverEffects(IEnumerable<Button> buttons)
    {
        foreach (var button in buttons)
        {
            button.MouseEnter += Button_MouseEnter;
            button.MouseLeave += Button_MouseLeave;
        }
    }

    private void Button_MouseEnter(object? sender, EventArgs e)
    {
        if (sender is Button { Enabled: true } btn)
        {
            btn.Cursor = Cursors.Hand;
            btn.BackColor = ControlPaint.Light(btn.BackColor, 0.2f);
        }
    }

    private void Button_MouseLeave(object? sender, EventArgs e)
    {
        if (sender is Button btn)
        {
            btn.Cursor = Cursors.Default;
            btn.BackColor = GetDefaultButtonColor(btn.Name);
        }
    }

    private static Color GetDefaultButtonColor(string buttonName) => buttonName switch
    {
        "btnStartScan" => UITheme.AccentColor,
        "btnCancel" => UITheme.DestructiveColor,
        _ => UITheme.NeutralButton
    };
}