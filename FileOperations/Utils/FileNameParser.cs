// File: FileOperations/Utils/FileNameParser.cs
namespace FileScanner.FileOperations.Utils;

// Decodes project-specific file naming conventions
internal static partial class FileNameParser
{
    // Matches type suffix, e.g., "MyModule_csharp.txt"
    [GeneratedRegex(@"^(.+)_[^_]+(\.txt)$", RegexOptions.Compiled)]
    private static partial Regex UnderscorePattern();

    // Matches file splitter suffix, e.g., "MyFile(PART_1-Symbols_10000).txt"
    [GeneratedRegex(@"^(.+)KATEX_INLINE_OPENPART_\d+-Symbols_\d+KATEX_INLINE_CLOSE(\.txt)$", RegexOptions.Compiled)]
    private static partial Regex SplitterPartPattern();

    // Extracts module name from intermediate file names
    // Handles two formats: type-suffixed and chunked files
    public static string ExtractModuleName(string filePath)
    {
        var fileName = Path.GetFileNameWithoutExtension(filePath);

        var parenIndex = fileName.IndexOf('(');
        if (parenIndex > 0 && SplitterPartPattern().IsMatch(Path.GetFileName(filePath)))
            // check for splitter pattern to avoid false positives
            return fileName[..parenIndex];

        var underscoreIndex = fileName.LastIndexOf('_');
        if (underscoreIndex > 0)
            return fileName[..underscoreIndex];

        return fileName;
    }

    // Creates a user-friendly file name for display in headers
    // Removes temporary suffixes like type or chunk number
    public static string GetCleanFileName(string filePath)
    {
        var fileName = Path.GetFileName(filePath);

        var matchUnderscore = UnderscorePattern().Match(fileName);
        if (matchUnderscore.Success)
            return $"{matchUnderscore.Groups[1].Value}{matchUnderscore.Groups[2].Value}";

        var matchSplitter = SplitterPartPattern().Match(fileName);
        if (matchSplitter.Success)
            return $"{matchSplitter.Groups[1].Value}{matchSplitter.Groups[2].Value}";

        return fileName;
    }
}