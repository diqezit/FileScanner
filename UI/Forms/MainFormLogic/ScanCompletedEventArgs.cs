// File: UI/Forms/MainFormLogic/ScanCompletedEventArgs.cs
namespace FileScanner.UI.Forms.MainFormLogic;

// Carries data for a successful scan completion event
public class ScanCompletedEventArgs(TimeSpan elapsed, string outputDirectory) : EventArgs
{
    public TimeSpan Elapsed { get; } = elapsed;
    public string OutputDirectory { get; } = outputDirectory;
}