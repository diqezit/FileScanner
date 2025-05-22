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

    public string GenerateDirectoryTree(string rootPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rootPath);

        if (!Directory.Exists(rootPath))
            throw new DirectoryNotFoundException("Directory not found");

        var sb = new StringBuilder();
        var rootName = Path.GetFileName(rootPath) ?? rootPath;
        sb.AppendLine($"{DirectoryIcon} {rootName}");

        try
        {
            GenerateTreeRecursive(rootPath, sb, "", 0);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error generating directory tree");
            sb.AppendLine($"└── {ErrorIcon} Error generating tree");
        }

        return sb.ToString();
    }

    private void GenerateTreeRecursive(string currentPath, StringBuilder sb, string prefix, int depth)
    {
        if (depth > _maxDepth)
        {
            sb.AppendLine($"{prefix}├── {ErrorIcon} Max depth exceeded");
            return;
        }

        try
        {
            var items = GetDirectoryItems(currentPath);
            ProcessItems(items, sb, prefix, depth, currentPath);
        }
        catch (UnauthorizedAccessException)
        {
            sb.AppendLine($"{prefix}├── {ErrorIcon} Access denied");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Error reading directory: {Path}", currentPath);
            sb.AppendLine($"{prefix}├── {ErrorIcon} Read error");
        }
    }

    private void ProcessItems((string[] Directories, string[] Files) items,
        StringBuilder sb, string prefix, int depth, string _)
    {
        var allItems = items.Directories.Concat(items.Files).ToArray();

        for (int i = 0; i < allItems.Length; i++)
        {
            var isLast = i == allItems.Length - 1;
            var connector = isLast ? "└── " : "├── ";
            var item = allItems[i];

            if (Directory.Exists(item))
            {
                var dirName = Path.GetFileName(item);
                sb.AppendLine($"{prefix}{connector}{DirectoryIcon} {dirName}");

                var newPrefix = prefix + (isLast ? "    " : "│   ");
                GenerateTreeRecursive(item, sb, newPrefix, depth + 1);
            }
            else
            {
                var fileName = Path.GetFileName(item);
                sb.AppendLine($"{prefix}{connector}{FileIcon} {fileName}");
            }
        }
    }

    private (string[] Directories, string[] Files) GetDirectoryItems(string path)
    {
        var directories = Directory.GetDirectories(path)
            .Where(dir => !validator.ShouldIgnoreDirectory(Path.GetFileName(dir)))
            .OrderBy(d => Path.GetFileName(d), StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var files = Directory.GetFiles(path)
            .Where(file => !validator.ShouldIgnoreFile(file))
            .OrderBy(f => Path.GetFileName(f), StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return (directories, files);
    }
}