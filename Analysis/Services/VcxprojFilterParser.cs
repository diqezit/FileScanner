#nullable enable

namespace FileScanner.Analysis.Services;

public sealed class VcxprojFilterParser : IVcxprojFilterParser
{
    public Dictionary<string, string> Parse(FilePath filterFilePath)
    {
        if (!File.Exists(filterFilePath.Value))
            return [];

        try
        {
            var doc = XDocument.Load(filterFilePath.Value);
            return ParseFileNodes(doc, filterFilePath);
        }
        catch (Exception)
        {
            return [];
        }
    }

    private static Dictionary<string, string> ParseFileNodes(
        XDocument doc,
        FilePath filterFilePath)
    {
        var fileToFilterMap = new Dictionary<string, string>(
            StringComparer.OrdinalIgnoreCase);

        var projectDir = Path.GetDirectoryName(filterFilePath.Value)!;
        XNamespace ns = "http://schemas.microsoft.com/developer/msbuild/2003";

        var fileNodes = doc.Descendants(ns + "ItemGroup")
            .SelectMany(g => g.Elements())
            .Where(el => el.Name.LocalName != "Filter");

        foreach (var node in fileNodes)
        {
            ProcessNode(
                node,
                projectDir,
                ns,
                fileToFilterMap);
        }

        return fileToFilterMap;
    }

    private static void ProcessNode(
        XElement node,
        string projectDir,
        XNamespace ns,
        Dictionary<string, string> fileToFilterMap)
    {
        var fileRelativePath = node.Attribute("Include")?.Value;
        if (string.IsNullOrEmpty(fileRelativePath))
            return;

        var filterPath = node.Elements(ns + "Filter")
            .FirstOrDefault()?.Value ?? "";

        var fullPath = Path.GetFullPath(
            Path.Combine(projectDir, fileRelativePath));

        fileToFilterMap[fullPath] = filterPath;
    }
}