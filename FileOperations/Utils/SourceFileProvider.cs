// FileOperations/Services/SourceFileProvider.cs
namespace FileScanner.FileOperations.Utils;

// This utility class is responsible only for finding and filtering source files
internal static class SourceFileProvider
{
    private const string FilePattern = "*.txt";
    private const string UnifiedFileName = "_United_All_Files.txt";

    // Finds all relevant files, excluding the final unified file itself.
    // Files are ordered to ensure a predictable and consistent output.
    public static string[] GetFilesToUnite(string directory) =>
        [..Directory.GetFiles(directory, FilePattern)
            .Where(file => !Path.GetFileName(file)
                .Equals(UnifiedFileName, StringComparison.OrdinalIgnoreCase))
            .OrderBy(file => file, StringComparer.OrdinalIgnoreCase)];
}