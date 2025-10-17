// UI/Controllers/ScanFeedbackController.cs
namespace FileScanner.UI.Controllers;

public sealed class ScanFeedbackController(IMainFormView view)
{
    public void HandleScanStarted()
    {
        view.SetScanningState(true);
        view.UpdateStatus("Scanning in progress...");
    }

    public void HandleScanCompleted(ScanCompletedEventArgs e)
    {
        view.SetScanningState(false);
        view.UpdateStatus("Scan completed successfully");
        view.UpdateFileCountLabel($"Done! Time: {e.Elapsed:mm\\:ss}");

        ShowScanSuccessDialog(e.OutputDirectory);
    }

    public void HandleScanCancelled()
    {
        view.SetScanningState(false);
        view.UpdateStatus("Scan cancelled");
        view.UpdateFileCountLabel("Cancelled");
    }

    public void HandleScanFailed(Exception ex)
    {
        view.SetScanningState(false);
        view.UpdateStatus("Scan error");
        view.UpdateFileCountLabel("Error");

        UIHelper.ShowErrorMessage(
            $"An error occurred during scan:\n\n{ex.Message}\n\nCheck logs for details");
    }

    private static void ShowScanSuccessDialog(string finalOutputPath)
    {
        var message = $"""
            Scan completed successfully!
            
            Files saved to:
            {finalOutputPath}
            
            Open output folder?
            """;

        if (UIHelper.ShowQuestionDialog(message, "Success") == DialogResult.Yes)
            UIHelper.OpenFolder(finalOutputPath);
    }
}