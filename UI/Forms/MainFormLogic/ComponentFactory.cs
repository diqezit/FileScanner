// UI/Forms/MainFormLogic/ComponentFactory.cs

namespace FileScanner.UI.Forms.MainFormLogic;

public record InputPanelControls(
    Panel Panel,
    TextBox TextBox,
    Button Button
);

internal static class ComponentFactory
{
    public static InputPanelControls CreateInputPanel(
        string title,
        string subtitle,
        EventHandler browseHandler)
    {
        var titleLabel = CreateLabel(
            text: title,
            font: UITheme.SubtitleFont,
            foreColor: UITheme.PrimaryText,
            location: new Point(0, 0));

        var subtitleLabel = CreateLabel(
            text: subtitle,
            font: UITheme.BodyFont,
            foreColor: UITheme.SecondaryText,
            location: new Point(0, 22));

        var textBox = CreatePathTextBox();
        var button = CreateBrowseButton(title, browseHandler);

        var panel = new Panel
        {
            Anchor = LayoutDefaults.TopLeftRight,
            Size = new Size(880, 70),
            BackColor = Color.Transparent
        };

        panel.Controls.AddRange(
        [
            titleLabel,
            subtitleLabel,
            textBox,
            button
        ]);

        return new InputPanelControls(panel, textBox, button);
    }

    public static Button CreateFlatButton(
        string name,
        string text,
        Font font,
        Color backColor,
        Color foreColor,
        ContentAlignment? textAlign = null)
    {
        var button = new Button
        {
            Name = name,
            Text = text,
            BackColor = backColor,
            FlatStyle = FlatStyle.Flat,
            Font = font,
            ForeColor = foreColor,
            UseVisualStyleBackColor = false
        };

        if (textAlign.HasValue)
            button.TextAlign = textAlign.Value;

        button.FlatAppearance.BorderSize = 0;
        return button;
    }

    public static TextBox CreateMultilineTextBox(
        Color backColor,
        Font font,
        Color foreColor,
        ScrollBars scrollBars)
    {
        return new TextBox
        {
            Dock = DockStyle.Fill,
            Multiline = true,
            BackColor = backColor,
            BorderStyle = BorderStyle.None,
            Font = font,
            ForeColor = foreColor,
            ScrollBars = scrollBars
        };
    }

    public static Icon CreateApplicationIcon()
    {
        using var bitmap = new Bitmap(32, 32);
        DrawApplicationIcon(bitmap);
        return Icon.FromHandle(bitmap.GetHicon());
    }

    private static void DrawApplicationIcon(Bitmap bitmap)
    {
        using var g = Graphics.FromImage(bitmap);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(Color.Transparent);

        using (var brush = new SolidBrush(UITheme.AccentColor))
            g.FillEllipse(brush, 2, 2, 28, 28);

        using var textBrush = new SolidBrush(UITheme.LightText);
        using var font = new Font("Segoe UI", 12F, FontStyle.Bold);
        using var format = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        g.DrawString(
            "FS",
            font,
            textBrush,
            new Rectangle(0, 0, 32, 32),
            format
        );
    }

    private static Label CreateLabel(
        string text,
        Font font,
        Color foreColor,
        Point location)
    {
        return new Label
        {
            Text = text,
            Font = font,
            ForeColor = foreColor,
            Location = location,
            AutoSize = true
        };
    }

    private static TextBox CreatePathTextBox()
    {
        return new TextBox
        {
            Anchor = LayoutDefaults.TopLeftRight,
            BackColor = UITheme.TextboxBackground,
            BorderStyle = BorderStyle.FixedSingle,
            Font = UITheme.CodeFont,
            Location = new Point(0, 44),
            Size = new Size(840, 24)
        };
    }

    private static Button CreateBrowseButton(
        string panelTitle,
        EventHandler clickHandler)
    {
        var button = CreateFlatButton(
            name: $"btnBrowse{panelTitle.Replace(" ", "")}",
            text: "\uE8B7",
            font: UITheme.IconFont,
            backColor: UITheme.NeutralButton,
            foreColor: UITheme.PrimaryText
        );

        button.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        button.Location = new Point(848, 44);
        button.Size = new Size(32, 24);
        button.Click += clickHandler;

        return button;
    }
}