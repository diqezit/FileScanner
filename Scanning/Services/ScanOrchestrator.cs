// Scanning/Services/ScanOrchestrator.cs
namespace FileScanner.Scanning.Services;

public sealed class ScanOrchestrator(
    IServiceProvider serviceProvider,
    ILogger<ScanOrchestrator> logger)
{
    public event EventHandler? ScanStarted;
    public event EventHandler<ScanCompletedEventArgs>? ScanCompleted;
    public event EventHandler? ScanCancelled;
    public event EventHandler<Exception>? ScanFailed;

    private CancellationTokenSource? _cancellationTokenSource;

    public bool IsScanRunning =>
        _cancellationTokenSource is { IsCancellationRequested: false };

    public void RequestCancellation() =>
        _cancellationTokenSource?.Cancel();

    public async Task PerformScanAsync(
        string projectPath,
        string baseOutputPath,
        PostScanOptions options)
    {
        _cancellationTokenSource = new CancellationTokenSource();

        try
        {
            await ExecuteScanPipelineAsync(
                new DirectoryPath(projectPath),
                new DirectoryPath(baseOutputPath),
                options,
                _cancellationTokenSource.Token);
        }
        catch (OperationCanceledException)
        {
            HandleCancellation();
        }
        catch (Exception ex)
        {
            HandleFailure(ex);
        }
        finally
        {
            CleanupCancellationSource();
        }
    }

    private async Task ExecuteScanPipelineAsync(
        DirectoryPath projectPath,
        DirectoryPath baseOutputPath,
        PostScanOptions options,
        CancellationToken token)
    {
        NotifyScanStarted();
        var stopwatch = Stopwatch.StartNew();

        using var scope = serviceProvider.CreateScope();

        var pipeline = CreatePipeline(scope.ServiceProvider);

        LogPipelineStart(projectPath);

        var result = await pipeline.ExecuteAsync(
            projectPath.Value,
            baseOutputPath.Value,
            options,
            token);

        LogPipelineSteps(result);
        stopwatch.Stop();

        ProcessPipelineResult(result, stopwatch.Elapsed);
    }

    private void NotifyScanStarted() =>
        ScanStarted?.Invoke(this, EventArgs.Empty);

    private static ScanningPipeline CreatePipeline(IServiceProvider services) =>
        new(
            projectProcessor: services.GetRequiredService<IProjectProcessor>(),
            unifiedFileWriter: services.GetRequiredService<IUnifiedFileWriter>(),
            treeGenerator: services.GetRequiredService<ITreeGenerator>(),
            statsCalculator: services.GetRequiredService<IProjectStatisticsCalculator>(),
            fileSplitter: services.GetRequiredService<IFileSplitter>()
        );

    private void LogPipelineStart(DirectoryPath projectPath) =>
        logger.LogInformation(
            "Starting scan pipeline for project: {Project}",
            projectPath.Value);

    private void LogPipelineSteps(PipelineResult result)
    {
        foreach (var step in result.ExecutedSteps)
            logger.LogDebug("Pipeline step: {Step}", step);
    }

    private void ProcessPipelineResult(
        PipelineResult result,
        TimeSpan elapsed)
    {
        if (result.IsSuccess)
        {
            HandleSuccess(result, elapsed);
        }
        else
        {
            HandlePipelineError(result);
        }
    }

    private void HandleSuccess(
        PipelineResult result,
        TimeSpan elapsed)
    {
        logger.LogInformation(
            "Pipeline completed successfully in {Elapsed}",
            elapsed);

        ScanCompleted?.Invoke(
            this,
            new ScanCompletedEventArgs(
                elapsed,
                result.OutputDirectory!));
    }

    private void HandlePipelineError(PipelineResult result)
    {
        var error = new Exception(
            $"Pipeline failed: {result.ErrorMessage}");

        logger.LogError(error, "Pipeline execution failed");
        throw error;
    }

    private void HandleCancellation() =>
        ScanCancelled?.Invoke(this, EventArgs.Empty);

    private void HandleFailure(Exception ex) =>
        ScanFailed?.Invoke(this, ex);

    private void CleanupCancellationSource()
    {
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
    }
}