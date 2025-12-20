namespace FileScanner.Core;

public sealed class ProjectAnalyzer(
    FileSystemServices fs,
    ScannerConfiguration cfg,
    ILogger<ProjectAnalyzer> log)
{
    public record Structure(
        IReadOnlyDictionary<string, List<string>> Groups,
        IReadOnlyList<string> Dirs);

    #region Enumerate

    public Structure Enumerate(string root, bool useFilters)
    {
        if (useFilters)
        {
            var f = Directory.GetFiles(
                root,
                "*.vcxproj.filters",
                SearchOption.TopDirectoryOnly);

            if (f.Length > 0)
                return FromFilters(f[0], root);
        }
        return FromPhysical(root);
    }

    Structure FromPhysical(string root)
    {
        var files = new List<string>();
        var dirs = new List<string>();

        Walk(root, files, dirs);

        var g = files
            .GroupBy(f => Path.GetDirectoryName(f)!)
            .ToDictionary(x => x.Key, x => x.ToList());

        return new(g, dirs);
    }

    void Walk(string dir, List<string> files, List<string> dirs)
    {
        if (fs.IgnoreDir(dir)) return;

        dirs.Add(dir);

        try
        {
            foreach (var f in Directory.EnumerateFiles(dir))
                if (!fs.IgnoreFile(f))
                    files.Add(f);

            foreach (var d in Directory.EnumerateDirectories(dir))
                Walk(d, files, dirs);
        }
        catch (UnauthorizedAccessException) { }
    }

    Structure FromFilters(string filterPath, string root)
    {
        var map = ParseFilters(filterPath);
        var g = new Dictionary<string, List<string>>(
            StringComparer.OrdinalIgnoreCase);
        var dirs = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            root
        };

        foreach (var (fp, filter) in map)
        {
            if (fs.IgnoreFile(fp)) continue;

            var ld = string.IsNullOrEmpty(filter)
                ? root
                : Path.GetFullPath(Path.Combine(root, filter));

            if (!g.TryGetValue(ld, out var l))
                g[ld] = l = [];
            l.Add(fp);

            var cur = ld;
            while (cur.Length > root.Length)
            {
                dirs.Add(cur);
                var p = Path.GetDirectoryName(cur);
                if (p == null || p.Length < root.Length)
                    break;
                cur = p;
            }
        }

        return new(g, [.. dirs.OrderBy(d => d)]);
    }

    Dictionary<string, string> ParseFilters(string path)
    {
        var r = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        try
        {
            var doc = XDocument.Load(path);
            var dir = Path.GetDirectoryName(path)!;
            XNamespace ns = "http://schemas.microsoft.com/developer/msbuild/2003";

            var nodes = doc.Descendants(ns + "ItemGroup")
                .SelectMany(g => g.Elements())
                .Where(e => e.Name.LocalName != "Filter");

            foreach (var n in nodes)
            {
                var inc = n.Attribute("Include")?.Value;
                if (string.IsNullOrEmpty(inc)) continue;

                var f = n.Elements(ns + "Filter").FirstOrDefault()?.Value ?? "";
                r[Path.GetFullPath(Path.Combine(dir, inc))] = f;
            }
        }
        catch (Exception ex)
        {
            log.LogWarning(ex, "Filter parse error");
        }

        return r;
    }

    #endregion

    #region Tree

    public string GenTree(Structure s, string root)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"📁 {Path.GetFileName(root)}");

        try
        {
            var adj = BuildAdj(s, root);
            BuildTree(root, adj, sb, "", 0);
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Tree error");
            sb.AppendLine("└── ❌ Error");
        }

        return sb.ToString();
    }

    Dictionary<string, SortedSet<string>> BuildAdj(Structure s, string root)
    {
        var adj = new Dictionary<string, SortedSet<string>>(
            StringComparer.OrdinalIgnoreCase);

        foreach (var d in s.Dirs.Where(
            d => !d.Equals(root, StringComparison.OrdinalIgnoreCase)))
        {
            var p = Path.GetDirectoryName(d);
            if (string.IsNullOrEmpty(p)) continue;

            if (!adj.TryGetValue(p, out var c))
                adj[p] = c = new(StringComparer.OrdinalIgnoreCase);
            c.Add(d);
        }

        foreach (var (dir, files) in s.Groups)
        {
            foreach (var f in files)
            {
                if (!adj.TryGetValue(dir, out var c))
                    adj[dir] = c = new(StringComparer.OrdinalIgnoreCase);
                c.Add(f);
            }
        }

        return adj;
    }

    void BuildTree(
        string cur,
        Dictionary<string, SortedSet<string>> adj,
        StringBuilder sb,
        string pre,
        int depth)
    {
        if (depth >= cfg.MaxTreeDepth || !adj.TryGetValue(cur, out var ch))
            return;

        var last = ch.LastOrDefault();

        foreach (var c in ch)
        {
            var isLast = c == last;
            var isDir = adj.ContainsKey(c);
            var icon = isDir ? "📁" : "📄";
            var connector = isLast ? "└── " : "├── ";

            sb.AppendLine($"{pre}{connector}{icon} {Path.GetFileName(c)}");

            if (isDir)
                BuildTree(
                    c,
                    adj,
                    sb,
                    pre + (isLast ? "    " : "│   "),
                    depth + 1);
        }
    }

    #endregion

    #region Stats

    public Task<ProjectStats> CalcStatsAsync(
        Structure s,
        CancellationToken ct) => Task.Run(() =>
        {
            long sz = 0;
            int cnt = 0;

            foreach (var f in s.Groups.SelectMany(g => g.Value))
            {
                ct.ThrowIfCancellationRequested();
                try
                {
                    sz += new FileInfo(f).Length;
                    cnt++;
                }
                catch { }
            }

            return new ProjectStats(cnt, s.Dirs.Count, sz);
        }, ct);

    public string FmtHeader(string tree, ProjectStats st)
    {
        var sb = new StringBuilder();

        sb.AppendLine("// =-=-=-=-=-=-=-=-=-=-=");
        sb.AppendLine("// PROJECT INFO");
        sb.AppendLine("// =-=-=-=-=-=-=-=-=-=-=");
        sb.AppendLine();

        sb.AppendLine("// TREE:");
        using (var r = new StringReader(tree))
            while (r.ReadLine() is { } ln)
                sb.AppendLine($"// {ln.Replace('\\', '/')}");
        sb.AppendLine();

        sb.AppendLine("// STATS:");
        sb.AppendLine($"// Files: {st.Files:N0}");
        sb.AppendLine($"// Dirs: {st.Dirs:N0}");
        sb.AppendLine($"// Size: {FileSystemServices.FmtSize(st.Size)}");
        sb.AppendLine();

        sb.AppendLine("// =-=-=-=-=-=-=-=-=-=-=");
        return sb.ToString();
    }

    #endregion
}