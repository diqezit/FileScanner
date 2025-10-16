// Scanning/Services/ScanOrchestrator.cs
namespace FileScanner.Scanning.Services;

public sealed class ScanOrchestrator(
    IFileScanner fileScanner,
    IUnifiedFileWriter unifiedFileWriter,
    ILogger<ScanOrchestrator> logger)
{
    private const string OutputSubfolderName = "GeneratedProjectContent";

    public event EventHandler? ScanStarted;
    public event EventHandler<ScanCompletedEventArgs>? ScanCompleted;
    public event EventHandler? ScanCancelled;
    public event EventHandler<Exception>? ScanFailed;

    private CancellationTokenSource? _cancellationTokenSource;

    public bool IsScanRunning => _cancellationTokenSource is { IsCancellationRequested: false };

    public void RequestCancellation() => _cancellationTokenSource?.Cancel();

    public async Task PerformScanAsync(
        string projectPath,
        string baseOutputPath)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        try
        {
            await ExecuteScanPipelineAsync(
                new DirectoryPath(projectPath),
                new DirectoryPath(baseOutputPath),
                _cancellationTokenSource.Token);
        }
        catch (OperationCanceledException)
        {
            ScanCancelled?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            ScanFailed?.Invoke(this, ex);
        }
        finally
        {
            CleanupCancellationSource();
        }
    }

    private async Task ExecuteScanPipelineAsync(
        DirectoryPath projectPath,
        DirectoryPath baseOutputPath,
        CancellationToken token)
    {
        ScanStarted?.Invoke(this, EventArgs.Empty);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var finalOutputPath = new DirectoryPath(Path.Combine(baseOutputPath.Value, OutputSubfolderName));

        LogScanStart(projectPath, finalOutputPath);

        var scanSuccess = await fileScanner.ScanDirectoryAsync(
            projectPath,
            finalOutputPath,
            token);

        token.ThrowIfCancellationRequested();

        if (scanSuccess)
            await FinalizeScanAsync(projectPath, finalOutputPath, token);

        stopwatch.Stop();
        ScanCompleted?.Invoke(
            this,
            new ScanCompletedEventArgs(stopwatch.Elapsed, finalOutputPath.Value));
    }

    private async Task FinalizeScanAsync(
        DirectoryPath projectPath,
        DirectoryPath finalOutputPath,
        CancellationToken token)
    {
        logger.LogInformation("Creating unified output file");
        await unifiedFileWriter.WriteUnifiedFileAsync(
            projectPath,
            finalOutputPath,
            token);
    }

    private void LogScanStart(DirectoryPath projectPath, DirectoryPath finalOutputPath)
    {
        logger.LogInformation(
            "Starting full scan for project: {Project}",
            projectPath.Value);

        logger.LogInformation(
            "Final output will be in: {OutputDirectory}",
            finalOutputPath.Value);
    }

    private void CleanupCancellationSource()
    {
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
    }
}