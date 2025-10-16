// Scanning/Services/FileGrouper.cs
namespace FileScanner.Scanning.Services;

public sealed class FileGrouper(
    IFileTypeClassifier classifier,
    IOptions<ScannerConfiguration> options) : IFileGrouper
{
    private readonly HashSet<string>? _allowedExtensions =
        options.Value.AllowedExtensions is { Length: > 0 }
            ? new HashSet<string>(
                options.Value.AllowedExtensions,
                StringComparer.OrdinalIgnoreCase)
            : null;

    private readonly HashSet<string> _ignoredExtensions = new(
        options.Value.IgnoredExtensions,
        StringComparer.OrdinalIgnoreCase);

    public Task<Dictionary<string, List<FilePath>>> GroupFilesByTypeAsync(
        IEnumerable<string> filePaths,
        CancellationToken cancellationToken)
    {
        var groups = new Dictionary<string, List<FilePath>>(StringComparer.OrdinalIgnoreCase);

        foreach (var filePathStr in filePaths)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var filePath = new FilePath(filePathStr);
            if (ShouldIgnoreFile(filePath)) continue;

            var fileType = classifier.ClassifyFile(filePath);
            AddFileToGroup(groups, fileType, filePath);
        }

        SortFileGroups(groups);
        return Task.FromResult(groups);
    }

    private static void AddFileToGroup(
        Dictionary<string, List<FilePath>> groups,
        string fileType,
        FilePath filePath)
    {
        if (!groups.TryGetValue(fileType, out var fileList))
        {
            fileList = [];
            groups[fileType] = fileList;
        }
        fileList.Add(filePath);
    }

    private static void SortFileGroups(Dictionary<string, List<FilePath>> groups)
    {
        foreach (var group in groups.Values)
        {
            group.Sort((f1, f2) => StringComparer.OrdinalIgnoreCase.Compare(f1.Value, f2.Value));
        }
    }

    private bool ShouldIgnoreFile(FilePath filePath) =>
        _allowedExtensions is not null
            ? !IsFileInWhitelist(filePath)
            : IsFileInBlacklist(filePath);

    private bool IsFileInWhitelist(FilePath filePath) =>
        _allowedExtensions!.Contains(
            Path.GetExtension(filePath.Value) is { Length: > 0 } ext
                ? ext
                : Path.GetFileName(filePath.Value));

    private bool IsFileInBlacklist(FilePath filePath)
    {
        var extension = Path.GetExtension(filePath.Value);
        return !string.IsNullOrEmpty(extension) && _ignoredExtensions.Contains(extension);
    }
}