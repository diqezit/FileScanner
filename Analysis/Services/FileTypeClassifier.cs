// Analysis/Services/FileTypeClassifier.cs
namespace FileScanner.Analysis.Services;

public sealed class FileTypeClassifier(IOptions<ScannerConfiguration> options)
    : IFileTypeClassifier
{
    private readonly Dictionary<string, string> _fileTypeMap = new(
        options.Value.FileTypeMapping,
        StringComparer.OrdinalIgnoreCase);

    public string ClassifyFile(FilePath filePath)
    {
        var extension = Path.GetExtension(filePath.Value);
        return _fileTypeMap.GetValueOrDefault(extension, FileTypes.Other);
    }

    public IEnumerable<string> GetAllFileTypes() =>
        _fileTypeMap.Values.Distinct().Append(FileTypes.Other).OrderBy(x => x);
}