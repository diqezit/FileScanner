namespace FileScanner.Scanning.Services;

public sealed class FileGrouper(
    IDirectoryValidator validator,
    IFileTypeClassifier classifier,
    ILogger<FileGrouper> logger) : IFileGrouper
{
    public Task<Dictionary<string, List<string>>> GroupFilesByTypeAsync(
        string directoryPath,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directoryPath);

        return Task.Run(() =>
        {
            var groups = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

            try
            {
                if (!Directory.Exists(directoryPath))
                    return groups;

                foreach (var file in Directory.EnumerateFiles(directoryPath))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (validator.ShouldIgnoreFile(file))
                        continue;

                    var fileType = classifier.ClassifyFile(file);
                    if (!groups.TryGetValue(fileType, out var list))
                        groups[fileType] = list = [];

                    list.Add(file);
                }

                foreach (var group in groups)
                    group.Value.Sort(StringComparer.OrdinalIgnoreCase);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Error grouping files in {Directory}", directoryPath);
            }

            return groups;
        }, cancellationToken);
    }
}