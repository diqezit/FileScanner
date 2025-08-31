namespace FileScanner.Scanning.Services;

public sealed class FileGrouper(
    IFileTypeClassifier classifier,
    IOptions<ScannerConfiguration> options,
    ILogger<FileGrouper> logger) : IFileGrouper
{
    private readonly IFileTypeClassifier _classifier = classifier;
    private readonly ILogger<FileGrouper> _logger = logger;

    private readonly HashSet<string>? _allowedExtensions =
        options.Value.AllowedExtensions is { Length: > 0 }
            ? new HashSet<string>(
                options.Value.AllowedExtensions,
                StringComparer.OrdinalIgnoreCase)
            : null;

    private readonly HashSet<string> _ignoredExtensions = new(
        options.Value.IgnoredExtensions,
        StringComparer.OrdinalIgnoreCase);

    private readonly string[] _ignoredDirectoryNames = options.Value.IgnoredDirectories;

    public Task<Dictionary<string, List<string>>> GroupFilesByTypeAsync(
        string directoryPath,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directoryPath);

        if (IsDirectoryIgnored(directoryPath))
            return Task.FromResult(new Dictionary<string, List<string>>());

        return Task.Run(() => ProcessDirectory(directoryPath, cancellationToken), cancellationToken);
    }

    private Dictionary<string, List<string>> ProcessDirectory(
        string directoryPath,
        CancellationToken cancellationToken)
    {
        var groups = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        try
        {
            if (!Directory.Exists(directoryPath))
                return groups;

            EnumerateAndGroupFiles(directoryPath, groups, cancellationToken);
            SortFileGroups(groups);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(
                ex,
                "Ошибка при группировке файлов в директории: {Directory}",
                directoryPath);
        }

        return groups;
    }

    private void EnumerateAndGroupFiles(
        string directoryPath,
        Dictionary<string, List<string>> groups,
        CancellationToken cancellationToken)
    {
        foreach (var file in Directory.EnumerateFiles(directoryPath))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (ShouldIgnoreFile(file))
                continue;

            var fileType = _classifier.ClassifyFile(file);
            if (!groups.TryGetValue(fileType, out var fileList))
            {
                fileList = [];
                groups[fileType] = fileList;
            }

            fileList.Add(file);
        }
    }

    private static void SortFileGroups(Dictionary<string, List<string>> groups)
    {
        foreach (var group in groups.Values)
            group.Sort(StringComparer.OrdinalIgnoreCase);
    }

    private bool IsDirectoryIgnored(string directoryPath)
    {
        var dirName = new DirectoryInfo(directoryPath).Name;
        if (_ignoredDirectoryNames.Contains(dirName, StringComparer.OrdinalIgnoreCase))
        {
            _logger.LogDebug(
                "Директория {Directory} пропущена, так как находится в списке игнорируемых",
                directoryPath);
            return true;
        }

        return false;
    }

    private bool ShouldIgnoreFile(string filePath)
    {
        var extension = Path.GetExtension(filePath);

        if (string.IsNullOrEmpty(extension))
            return _allowedExtensions != null 
                && !_allowedExtensions.Contains(Path.GetFileName(filePath));

        return _allowedExtensions != null
            ? !_allowedExtensions.Contains(extension)
            : _ignoredExtensions.Contains(extension);
    }
}