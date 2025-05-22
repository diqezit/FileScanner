namespace FileScanner.Core.Services;

public sealed class FileTypeClassifier : IFileTypeClassifier
{
    private static readonly Dictionary<string, string> FileTypeMap =
        new(StringComparer.OrdinalIgnoreCase)
        {
            [".cs"] = "cs",
            [".xaml"] = "xaml",
            [".xaml.cs"] = "cs",
            [".csproj"] = "config",
            [".sln"] = "config",
            [".config"] = "config",
            [".resx"] = "resources"
        };

    public string ClassifyFile(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        if (filePath.EndsWith(".xaml.cs", StringComparison.OrdinalIgnoreCase))
            return "cs";

        var extension = Path.GetExtension(filePath);
        return FileTypeMap.TryGetValue(extension, out var fileType)
            ? fileType
            : "other";
    }

    public IEnumerable<string> GetAllFileTypes() =>
        FileTypeMap.Values.Distinct().Concat(["other"]);
}