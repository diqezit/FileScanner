#nullable enable

namespace FileScanner.Analysis.Services;

public sealed class TreeGenerator(
    IOptions<ScannerConfiguration> options,
    ILogger<TreeGenerator> logger) : ITreeGenerator
{
    private readonly int _maxDepth = options.Value.MaxTreeDepth;
    private const string DirectoryIcon = "📁";
    private const string FileIcon = "📄";
    private const string ErrorIcon = "❌";

    public string GenerateDirectoryTree(
        ProjectStructure projectStructure,
        DirectoryPath rootPath)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{DirectoryIcon} {Path.GetFileName(rootPath.Value)}");

        try
        {
            var adjacencyList = BuildAdjacencyList(projectStructure, rootPath);
            BuildTreeRecursively(
                rootPath.Value,
                adjacencyList,
                sb,
                "",
                0);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Failed to generate directory tree for {Path}",
                rootPath.Value);
            sb.AppendLine($"└── {ErrorIcon} Error generating tree");
        }

        return sb.ToString();
    }

    private static Dictionary<string, SortedSet<string>> BuildAdjacencyList(
        ProjectStructure structure,
        DirectoryPath root)
    {
        var adjacencyList = new Dictionary<string, SortedSet<string>>(
            StringComparer.OrdinalIgnoreCase);

        AddDirectoryNodes(structure.AllDirectories, root, adjacencyList);
        AddFileNodes(structure.FileGroups, adjacencyList);

        return adjacencyList;
    }

    private static void AddDirectoryNodes(
        IReadOnlyList<DirectoryPath> directories,
        DirectoryPath root,
        Dictionary<string, SortedSet<string>> adjacencyList)
    {
        foreach (var dirPath in directories)
        {
            if (dirPath.Value.Equals(
                root.Value,
                StringComparison.OrdinalIgnoreCase))
                continue;

            AddNode(dirPath.Value, adjacencyList);
        }
    }

    private static void AddFileNodes(
        IReadOnlyDictionary<DirectoryPath, List<FilePath>> fileGroups,
        Dictionary<string, SortedSet<string>> adjacencyList)
    {
        foreach (var group in fileGroups)
            foreach (var file in group.Value)
                AddNode(file.Value, adjacencyList, group.Key.Value);
    }

    private static void AddNode(
        string path,
        Dictionary<string, SortedSet<string>> adjacencyList,
        string? parentPath = null)
    {
        parentPath ??= Path.GetDirectoryName(path);
        if (string.IsNullOrEmpty(parentPath))
            return;

        if (!adjacencyList.TryGetValue(parentPath, out var children))
        {
            children = new SortedSet<string>(
                StringComparer.OrdinalIgnoreCase);
            adjacencyList[parentPath] = children;
        }
        children.Add(path);
    }

    private void BuildTreeRecursively(
        string currentPath,
        Dictionary<string, SortedSet<string>> adjacencyList,
        StringBuilder sb,
        string prefix,
        int depth)
    {
        if (depth >= _maxDepth)
            return;
        if (!adjacencyList.TryGetValue(currentPath, out var children))
            return;

        var lastChild = children.LastOrDefault();
        foreach (var childPath in children)
        {
            ProcessNode(
                childPath,
                lastChild,
                adjacencyList,
                sb,
                prefix,
                depth);
        }
    }

    private void ProcessNode(
        string childPath,
        string? lastChild,
        Dictionary<string, SortedSet<string>> adjacencyList,
        StringBuilder sb,
        string prefix,
        int depth)
    {
        var isLast = childPath == lastChild;
        var connector = isLast ? "└── " : "├── ";
        var isDirectory = adjacencyList.ContainsKey(childPath);
        var icon = isDirectory ? DirectoryIcon : FileIcon;

        sb.AppendLine(
            $"{prefix}{connector}{icon} {Path.GetFileName(childPath)}");

        if (isDirectory)
        {
            var newPrefix = prefix + (isLast ? "    " : "│   ");
            BuildTreeRecursively(
                childPath,
                adjacencyList,
                sb,
                newPrefix,
                depth + 1);
        }
    }
}