// Analysis/Services/TreeGenerator.cs
namespace FileScanner.Analysis.Services;

public sealed class TreeGenerator(
    IDirectoryValidator validator,
    IOptions<ScannerConfiguration> options,
    ILogger<TreeGenerator> logger) : ITreeGenerator
{
    private readonly int _maxDepth = options.Value.MaxTreeDepth;

    private const string DirectoryIcon = "📁";
    private const string FileIcon = "📄";
    private const string ErrorIcon = "❌";

    private record FileSystemNode(string Path, bool IsDirectory);

    public string GenerateDirectoryTree(DirectoryPath rootPath)
    {
        if (!Directory.Exists(rootPath.Value))
            throw new DirectoryNotFoundException($"Directory not found: {rootPath.Value}");

        var sb = new StringBuilder();
        AppendRootLine(sb, rootPath.Value);

        try
        {
            BuildTree(rootPath.Value, sb, "", 0);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to generate directory tree for {Path}", rootPath.Value);
            sb.AppendLine($"└── {ErrorIcon} Error generating tree");
        }

        return sb.ToString();
    }

    private void BuildTree(string currentPath, StringBuilder sb, string prefix, int depth)
    {
        if (depth >= _maxDepth)
        {
            AppendErrorLine(sb, prefix, "Max depth exceeded");
            return;
        }

        if (!TryGetChildNodes(currentPath, out var nodes, out var errorMessage))
        {
            AppendErrorLine(sb, prefix, errorMessage);
            return;
        }

        for (int i = 0; i < nodes.Count; i++)
        {
            ProcessNode(nodes[i], sb, prefix, depth, i == nodes.Count - 1);
        }
    }

    private void ProcessNode(
        FileSystemNode node,
        StringBuilder sb,
        string prefix,
        int depth,
        bool isLast)
    {
        AppendNodeLine(sb, node, prefix, isLast);

        if (node.IsDirectory)
        {
            var newPrefix = prefix + (isLast ? "    " : "│   ");
            BuildTree(node.Path, sb, newPrefix, depth + 1);
        }
    }

    private bool TryGetChildNodes(
        string path,
        out List<FileSystemNode> nodes,
        out string errorMessage)
    {
        try
        {
            var directories = GetFilteredDirectories(path);
            var files = GetFilteredFiles(path);

            nodes = [..directories.Concat(files)
                .OrderBy(node => Path.GetFileName(node.Path), StringComparer.OrdinalIgnoreCase)];

            errorMessage = string.Empty;
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            nodes = [];
            errorMessage = "Access denied";
            return false;
        }
        catch (Exception ex)
        {
            nodes = [];
            logger.LogWarning(ex, "Error reading directory: {Path}", path);
            errorMessage = "Read error";
            return false;
        }
    }

    private IEnumerable<FileSystemNode> GetFilteredDirectories(string path) =>
        Directory.GetDirectories(path)
            .Where(dir => !validator.ShouldIgnoreDirectory(dir))
            .Select(dir => new FileSystemNode(dir, true));

    private IEnumerable<FileSystemNode> GetFilteredFiles(string path) =>
        Directory.GetFiles(path)
            .Where(file => !validator.ShouldIgnoreFile(new FilePath(file)))
            .Select(file => new FileSystemNode(file, false));

    private static void AppendRootLine(StringBuilder sb, string rootPath) =>
        sb.AppendLine($"{DirectoryIcon} {Path.GetFileName(rootPath) ?? rootPath}");

    private static void AppendNodeLine(
        StringBuilder sb,
        FileSystemNode node,
        string prefix,
        bool isLast)
    {
        var connector = isLast ? "└── " : "├── ";
        var icon = node.IsDirectory ? DirectoryIcon : FileIcon;
        var name = Path.GetFileName(node.Path);
        sb.AppendLine($"{prefix}{connector}{icon} {name}");
    }

    private static void AppendErrorLine(StringBuilder sb, string prefix, string message) =>
        sb.AppendLine($"{prefix}└── {ErrorIcon} {message}");
}