// File: UI/Forms/MainFormLogic/ScanUIHandler.cs
namespace FileScanner.UI.Forms.MainFormLogic;

// Translates scan lifecycle events into UI updates
public sealed class ScanUIHandler(MainForm form, ILogger logger)
{
    public void OnScanStarted(object? _, EventArgs __)
    {
        form.SetScanningState(true);
        form.ClearLog();
        form.UpdateStatus("Scanning in progress...");
        logger.LogInformation("=== Starting project scan ===");
        logger.LogInformation("Project: {ProjectPath}", form.GetTrimmedProjectPath());
        logger.LogInformation("Output: {OutputPath}", form.GetTrimmedOutputPath());
    }

    public void OnScanCompleted(object? _, ScanCompletedEventArgs e)
    {
        form.UpdateStatus("Scan completed successfully");
        form.UpdateFileCountLabel($"Done! Time: {e.Elapsed:mm\\:ss}");
        form.SetScanningState(false);
        logger.LogInformation("=== Scan completed successfully in {ElapsedTime} ===", e.Elapsed);

        // Use the correct path from the event args
        ShowScanSuccessDialog(e.OutputDirectory);
    }

    public void OnScanCancelled(object? _, EventArgs __)
    {
        form.UpdateStatus("Scan cancelled");
        form.UpdateFileCountLabel("Cancelled");
        form.SetScanningState(false);
        logger.LogWarning("Scan was cancelled by user");
    }

    public void OnScanFailed(object? _, Exception ex)
    {
        form.UpdateStatus("Scan error");
        form.UpdateFileCountLabel("Error");
        form.SetScanningState(false);
        logger.LogError(ex, "An error occurred during scan");
        UIHelper.ShowErrorMessage(
            $"An error occurred during scan:\n\n{ex.Message}\n\nCheck logs for details");
    }

    // Offer to open output folder to improve user workflow
    private void ShowScanSuccessDialog(string finalOutputPath)
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