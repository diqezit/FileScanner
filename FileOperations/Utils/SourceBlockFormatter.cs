// File: FileOperations/Utils/SourceBlockFormatter.cs
namespace FileScanner.FileOperations.Utils;

// Isolates source file block formatting from file read logic
internal static class SourceBlockFormatter
{
    public static List<string> Format(FileContent fileContent) =>
    [
        FormattingConstants.Separator,
        $"// SOURCE FILE: {fileContent.RelativePath.Value}",
        FormattingConstants.Separator,
        string.Empty,
        fileContent.Content,
        string.Empty,
        string.Empty
    ];
}