// FileOperations/Utils/MetadataHeaderFormatter.cs
namespace FileScanner.FileOperations.Utils;

internal static class MetadataHeaderFormatter
{
    private const string Separator = "// =-=-=-=-=-=-=-=-=-=-=";

    public static string Format(string tree, ProjectStatistics stats)
    {
        var sb = new StringBuilder();

        sb.AppendLine(Separator);
        sb.AppendLine("// PROJECT STRUCTURE & STATISTICS");
        sb.AppendLine(Separator);
        sb.AppendLine();

        AppendSection(sb, "DIRECTORY TREE", tree);
        AppendSection(sb, "STATISTICS", FormatStats(stats));

        sb.AppendLine(Separator);
        return sb.ToString();
    }

    private static string FormatStats(ProjectStatistics stats) =>
        new StringBuilder()
            .AppendLine($"Total Files: {stats.FileCount:N0}")
            .AppendLine($"Total Directories: {stats.DirectoryCount:N0}")
            .Append($"Total Size: {UIHelper.FormatFileSize(stats.TotalSize)}")
            .ToString();

    private static void AppendSection(StringBuilder sb, string title, string content)
    {
        sb.AppendLine($"// {title}:");

        using var reader = new StringReader(content);
        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            sb.AppendLine($"// {line.Replace('\\', '/')}");
        }
        sb.AppendLine();
    }
}