// FormLogger.cs
namespace FileScanner;

public class FormLogger(TextBox logTextBox) : ILogger
{
    public void Log(string message)
    {
        if (logTextBox.InvokeRequired)
        {
            logTextBox.Invoke(new Action(() =>
            {
                logTextBox.AppendText(message + Environment.NewLine);
            }));
        }
        else
        {
            logTextBox.AppendText(message + Environment.NewLine);
        }
    }
}