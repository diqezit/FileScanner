namespace FileScanner.Core;

public sealed class ScanEngine(
    FileSystemServices fs,
    ProjectAnalyzer az,
    ILogger<ScanEngine> log)
{
    CancellationTokenSource? _cts;

    public event EventHandler? Started;
    public event EventHandler<ScanDoneArgs>? Done;
    public event EventHandler? Cancelled;
    public event EventHandler<Exception>? Failed;

    public bool Running => _cts is { IsCancellationRequested: false };
    public void Cancel() => _cts?.Cancel();

    public async Task RunAsync(string proj, string outBase, ScanOptions opt)
    {
        _cts = new CancellationTokenSource();
        var ct = _cts.Token;

        try
        {
            Started?.Invoke(this, EventArgs.Empty);
            var sw = Stopwatch.StartNew();

            var r = await ExecAsync(proj, outBase, opt, ct);

            sw.Stop();

            if (r.Ok)
            {
                log.LogInformation("Done in {T}", sw.Elapsed);
                Done?.Invoke(this, new(sw.Elapsed, r.OutDir!));
            }
            else
            {
                throw new Exception(r.Error ?? "Unknown error");
            }
        }
        catch (OperationCanceledException)
        {
            log.LogInformation("Cancelled");
            Cancelled?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Failed");
            Failed?.Invoke(this, ex);
        }
        finally
        {
            _cts?.Dispose();
            _cts = null;
        }
    }

    async Task<ScanResult> ExecAsync(
        string proj,
        string outBase,
        ScanOptions opt,
        CancellationToken ct)
    {
        if (!Directory.Exists(proj))
            return new(false, Error: "Directory not found");

        var outDir = Path.Combine(outBase, "GeneratedProjectContent");
        fs.CleanDir(outDir);
        log.LogInformation("Output: {D}", outDir);

        log.LogInformation("Scanning: {P}", proj);
        var s = az.Enumerate(proj, opt.UseFilters);
        log.LogInformation("Found {N} groups", s.Groups.Count);
        ct.ThrowIfCancellationRequested();

        var stats = await az.CalcStatsAsync(s, ct);
        log.LogInformation(
            "Stats: {F} files, {D} dirs, {S}",
            stats.Files,
            stats.Dirs,
            FileSystemServices.FmtSize(stats.Size));

        var tree = az.GenTree(s, proj);
        var header = az.FmtHeader(tree, stats);
        ct.ThrowIfCancellationRequested();

        log.LogInformation("Processing...");
        foreach (var (dir, files) in s.Groups)
        {
            ct.ThrowIfCancellationRequested();
            await ProcessDirAsync(dir, files, proj, outDir, ct);
        }
        ct.ThrowIfCancellationRequested();

        log.LogInformation("Creating unified...");
        await fs.WriteUnifiedAsync(outDir, header, ct);

        // Split если включено
        if (opt.Split && opt.ChunkSize > 0)
        {
            var unifiedPath = Path.Combine(outDir, "_United_All_Files.txt");
            log.LogInformation(
                "Split enabled: chunk size = {S} chars",
                opt.ChunkSize);
            await fs.SplitAsync(unifiedPath, opt.ChunkSize, ct);
        }

        return new(true, outDir, tree, stats);
    }

    async Task ProcessDirAsync(
        string dir,
        List<string> files,
        string root,
        string outDir,
        CancellationToken ct)
    {
        var byType = fs.GroupByType(files);

        foreach (var (type, list) in byType)
        {
            if (list.Count == 0) continue;
            ct.ThrowIfCancellationRequested();

            var content = await fs.ReadFormatAsync(list, root, ct);
            if (content.Count == 0) continue;

            var name = fs.GenFileName(dir, root, type);
            await fs.WriteAsync(Path.Combine(outDir, $"{name}.txt"), content, ct);
        }
    }
}

public record ScanDoneArgs(TimeSpan Elapsed, string OutDir);