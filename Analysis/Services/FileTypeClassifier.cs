namespace FileScanner.Analysis.Services;

public sealed class FileTypeClassifier(IOptions<ScannerConfiguration> options) : IFileTypeClassifier
{
    private readonly Dictionary<string, string> _fileTypeMap = new(
        options.Value.FileTypeMapping,
        StringComparer.OrdinalIgnoreCase);

    public string ClassifyFile(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        var extension = Path.GetExtension(filePath);
        return _fileTypeMap.GetValueOrDefault(extension, "other");
    }

    public IEnumerable<string> GetAllFileTypes() =>
        _fileTypeMap.Values.Distinct().Append("other").OrderBy(x => x);
}