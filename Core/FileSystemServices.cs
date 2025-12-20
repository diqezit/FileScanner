namespace FileScanner.Core;

public sealed class FileSystemServices(ScannerConfiguration cfg, ILogger<FileSystemServices> log)
{
    #region Validation

    public bool IgnoreDir(string p)
    {
        if (string.IsNullOrWhiteSpace(p)) return true;
        var n = Path.GetFileName(p);
        return string.IsNullOrWhiteSpace(n) ||
               cfg.IgnoredDirs.Contains(n) ||
               (n.StartsWith('.') && n.Length > 1);
    }

    public bool IgnoreFile(string p)
    {
        if (string.IsNullOrWhiteSpace(p)) return true;
        var n = Path.GetFileName(p);
        if (string.IsNullOrWhiteSpace(n) || (n.StartsWith('.') && n.Length > 1))
            return true;
        var ext = Path.GetExtension(p);
        return !string.IsNullOrEmpty(ext) && cfg.IgnoredExt.Contains(ext);
    }

    #endregion

    #region Classification

    public string Classify(string p) =>
        cfg.TypeMap.GetValueOrDefault(Path.GetExtension(p), "other");

    public Dictionary<string, List<string>> GroupByType(IEnumerable<string> files)
    {
        var g = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        foreach (var f in files)
        {
            if (IgnoreFile(f)) continue;
            var t = Classify(f);
            if (!g.TryGetValue(t, out var l)) g[t] = l = [];
            l.Add(f);
        }
        foreach (var v in g.Values)
            v.Sort(StringComparer.OrdinalIgnoreCase);
        return g;
    }

    #endregion

    #region Read

    public async Task<(string Content, bool Ok, string? Err)> ReadAsync(
        string p,
        CancellationToken ct)
    {
        try
        {
            var fi = new FileInfo(p);
            if (!fi.Exists) return ("", false, "Not found");
            if (fi.Length > cfg.MaxFileSize)
                return ("", false, $"Too large ({FmtSize(fi.Length)})");
            return (await File.ReadAllTextAsync(p, Encoding.UTF8, ct), true, null);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            log.LogWarning(ex, "Read error: {F}", p);
            return ("", false, ex.Message);
        }
    }

    public async Task<List<string>> ReadFormatAsync(
        IEnumerable<string> files,
        string root,
        CancellationToken ct)
    {
        var r = new List<string>();
        foreach (var f in files)
        {
            ct.ThrowIfCancellationRequested();
            var rel = Path.GetRelativePath(root, f).Replace('\\', '/');
            var (c, ok, err) = await ReadAsync(f, ct);

            r.Add("// =-=-=-=-=-=-=-=-=-=-=");
            r.Add($"// SOURCE: {rel}");
            r.Add("// =-=-=-=-=-=-=-=-=-=-=");
            r.Add("");
            r.Add(ok ? c : $"// ERROR: {err}");
            r.Add("");
            r.Add("");
        }
        return r;
    }

    #endregion

    #region Write

    public async Task WriteAsync(
        string path,
        IEnumerable<string> lines,
        CancellationToken ct)
    {
        var dir = Path.GetDirectoryName(path);
        if (dir != null) Directory.CreateDirectory(dir);

        await using var fs = new FileStream(
            path,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            cfg.BufferSize,
            true);
        await using var w = new StreamWriter(fs, Encoding.UTF8, cfg.BufferSize);

        foreach (var line in lines)
        {
            ct.ThrowIfCancellationRequested();
            await w.WriteLineAsync(line.AsMemory(), ct);
        }
        log.LogInformation("Created: {F}", Path.GetFileName(path));
    }

    public async Task WriteUnifiedAsync(
        string outDir,
        string header,
        CancellationToken ct)
    {
        var files = Directory.GetFiles(outDir, "*.txt")
            .Where(f => !Path.GetFileName(f)
                .StartsWith("_United", StringComparison.OrdinalIgnoreCase))
            .OrderBy(f => f)
            .ToArray();

        if (files.Length == 0)
        {
            log.LogWarning("No files to unite");
            return;
        }

        var path = Path.Combine(outDir, "_United_All_Files.txt");
        await using var w = new StreamWriter(path, false, Encoding.UTF8);

        await w.WriteLineAsync("// =-=-=-=-=-=-=-=-=-=-=");
        await w.WriteLineAsync("// UNIFIED PROJECT");
        await w.WriteLineAsync($"// Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        await w.WriteLineAsync($"// Files: {files.Length}");
        await w.WriteLineAsync("// =-=-=-=-=-=-=-=-=-=-=");
        await w.WriteLineAsync();
        await w.WriteLineAsync(header);

        string? mod = null;
        foreach (var f in files)
        {
            ct.ThrowIfCancellationRequested();
            var m = ExtractModule(f);
            if (m != mod)
            {
                await w.WriteLineAsync();
                await w.WriteLineAsync("// ████████████████████████████████████████");
                await w.WriteLineAsync($"// ███ MODULE: {m.ToUpper()}");
                await w.WriteLineAsync("// ████████████████████████████████████████");
                await w.WriteLineAsync();
                mod = m;
            }

            await w.WriteLineAsync("// =-=-=-=-=-=-=-=-=-=-=");
            await w.WriteLineAsync($"// FILE: {CleanName(f)}");
            await w.WriteLineAsync("// =-=-=-=-=-=-=-=-=-=-=");
            await w.WriteLineAsync();
            await w.WriteLineAsync(await File.ReadAllTextAsync(f, ct));
            await w.WriteLineAsync();
        }
        log.LogInformation("Created unified file");
    }

    public async Task SplitAsync(
        string srcPath,
        int chunkSize,
        CancellationToken ct)
    {
        if (!File.Exists(srcPath))
        {
            log.LogWarning("Split source not found: {F}", srcPath);
            return;
        }

        var content = await File.ReadAllTextAsync(srcPath, ct);
        if (content.Length <= chunkSize)
        {
            log.LogInformation("File smaller than chunk, skip split");
            return;
        }

        log.LogInformation(
            "Splitting {F} ({L} chars) into ~{S} char chunks",
            Path.GetFileName(srcPath),
            content.Length,
            chunkSize);

        var dir = Path.GetDirectoryName(srcPath)!;
        var name = Path.GetFileNameWithoutExtension(srcPath);
        var ext = Path.GetExtension(srcPath);

        int part = 1;
        int pos = 0;

        while (pos < content.Length)
        {
            ct.ThrowIfCancellationRequested();

            var len = Math.Min(chunkSize, content.Length - pos);
            var chunk = content.Substring(pos, len);

            var chunkName = $"{name}(PART_{part}-Symbols_{len}){ext}";
            var chunkPath = Path.Combine(dir, chunkName);

            await File.WriteAllTextAsync(chunkPath, chunk, ct);
            log.LogInformation("Created chunk: {F}", chunkName);

            pos += len;
            part++;
        }

        log.LogInformation("Split complete: {N} parts", part - 1);
    }

    public void CleanDir(string dir)
    {
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
            return;
        }

        var files = Directory.GetFiles(dir, "*.txt");
        foreach (var f in files)
            try { File.Delete(f); } catch { }

        if (files.Length > 0)
            log.LogInformation("Cleaned {N} files", files.Length);
    }

    #endregion

    #region Names

    public string GenFileName(string dir, string root, string type)
    {
        var b = dir.Equals(root, StringComparison.OrdinalIgnoreCase)
            ? "Root"
            : Sanitize(Path.GetRelativePath(root, dir));

        if (!type.Equals("other", StringComparison.OrdinalIgnoreCase))
            b = $"{b}_{type}";

        return b.Length > 200
            ? $"{b[..191]}_{Math.Abs(b.GetHashCode()):X8}"
            : b;
    }

    static string Sanitize(string p)
    {
        var inv = Path.GetInvalidFileNameChars()
            .Concat([Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar])
            .ToHashSet();

        var sb = new StringBuilder();
        foreach (var c in p)
            if (!inv.Contains(c))
                sb.Append(c == ' ' ? '_' : c);

        var r = sb.ToString();
        return string.IsNullOrWhiteSpace(r) ? "unnamed" : r;
    }

    static string ExtractModule(string p)
    {
        var n = Path.GetFileNameWithoutExtension(p);
        var i = n.IndexOf('(');
        if (i > 0) return n[..i];
        i = n.LastIndexOf('_');
        return i > 0 ? n[..i] : n;
    }

    static string CleanName(string p)
    {
        var n = Path.GetFileName(p);

        var m = Regex.Match(n, @"^(.+)_[^_]+(\.txt)$");
        if (m.Success)
            return $"{m.Groups[1].Value}{m.Groups[2].Value}";

        m = Regex.Match(n, @"^(.+)\(PART_\d+-Symbols_\d+\)(\.txt)$");
        return m.Success
            ? $"{m.Groups[1].Value}{m.Groups[2].Value}"
            : n;
    }

    #endregion

    #region Util

    public static string FmtSize(long b)
    {
        string[] s = ["B", "KB", "MB", "GB"];
        double v = b;
        int i = 0;
        while (v >= 1024 && i < s.Length - 1)
        {
            v /= 1024;
            i++;
        }
        return $"{v:0.##} {s[i]}";
    }

    #endregion
}