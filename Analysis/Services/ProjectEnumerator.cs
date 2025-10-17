#nullable enable

namespace FileScanner.Analysis.Services;

public sealed class ProjectEnumerator(
    IDirectoryValidator validator,
    IVcxprojFilterParser filterParser) : IProjectEnumerator
{
    public ProjectStructure EnumerateProject(
        DirectoryPath projectRoot,
        bool useFilters)
    {
        if (useFilters)
        {
            var filtersFilePath = FindFiltersFile(projectRoot.Value);
            if (filtersFilePath is not null)
                return EnumerateFromFilters(filtersFilePath.Value, projectRoot);
        }
        return EnumerateFromPhysicalDirectories(projectRoot);
    }

    private ProjectStructure EnumerateFromFilters(
        FilePath filtersFilePath,
        DirectoryPath root)
    {
        var fileToFilterMap = filterParser.Parse(filtersFilePath);
        var fileGroups = new Dictionary<DirectoryPath, List<FilePath>>();
        var allDirectories = new HashSet<DirectoryPath> { root };

        foreach (var (filePath, filterPath) in fileToFilterMap)
        {
            ProcessFilterEntry(
                filePath,
                filterPath,
                root,
                fileGroups,
                allDirectories);
        }

        return new ProjectStructure(
            fileGroups,
            [.. allDirectories.OrderBy(d => d.Value)]);
    }

    private void ProcessFilterEntry(
        string filePath,
        string filterPath,
        DirectoryPath root,
        Dictionary<DirectoryPath, List<FilePath>> fileGroups,
        HashSet<DirectoryPath> allDirectories)
    {
        var file = new FilePath(filePath);
        if (validator.ShouldIgnoreFile(file))
            return;

        var logicalDir = string.IsNullOrEmpty(filterPath)
            ? root
            : new DirectoryPath(Path.GetFullPath(
                Path.Combine(root.Value, filterPath)));

        AddFilesToGroup(logicalDir, [file], fileGroups);
        CollectAllParentDirectories(logicalDir, root, allDirectories);
    }

    private static void CollectAllParentDirectories(
        DirectoryPath logicalDir,
        DirectoryPath root,
        HashSet<DirectoryPath> allDirectories)
    {
        var current = logicalDir;
        while (current.Value.Length > root.Value.Length)
        {
            allDirectories.Add(current);
            var parentPath = Path.GetDirectoryName(current.Value);
            if (parentPath is null || parentPath.Length < root.Value.Length)
                break;
            current = new DirectoryPath(parentPath);
        }
    }

    private ProjectStructure EnumerateFromPhysicalDirectories(
        DirectoryPath projectRoot)
    {
        var (allFiles, allDirs) = EnumeratePhysicalStructure(projectRoot);
        var fileGroups = GroupFilesByPhysicalDirectory(allFiles);
        return new ProjectStructure(fileGroups, allDirs);
    }

    private (List<FilePath>, List<DirectoryPath>) EnumeratePhysicalStructure(
        DirectoryPath projectRoot)
    {
        var allFiles = new List<FilePath>();
        var allDirs = new List<DirectoryPath>();
        WalkDirectoryTree(projectRoot, allFiles, allDirs);
        return (allFiles, allDirs);
    }

    private void WalkDirectoryTree(
        DirectoryPath currentDir,
        ICollection<FilePath> files,
        ICollection<DirectoryPath> directories)
    {
        if (validator.ShouldIgnoreDirectory(currentDir.Value))
            return;

        directories.Add(currentDir);
        try
        {
            foreach (var file in Directory.EnumerateFiles(currentDir.Value))
            {
                var filePath = new FilePath(file);
                if (!validator.ShouldIgnoreFile(filePath))
                    files.Add(filePath);
            }

            foreach (var subDir in Directory.EnumerateDirectories(currentDir.Value))
                WalkDirectoryTree(new DirectoryPath(subDir), files, directories);
        }
        catch (UnauthorizedAccessException) { }
    }

    private static Dictionary<DirectoryPath, List<FilePath>> GroupFilesByPhysicalDirectory(
        List<FilePath> allFiles) =>
        allFiles
            .GroupBy(f => new DirectoryPath(Path.GetDirectoryName(f.Value)!))
            .ToDictionary(g => g.Key, g => g.ToList());

    private static FilePath? FindFiltersFile(string rootPath)
    {
        var files = Directory.GetFiles(
            rootPath,
            "*.vcxproj.filters",
            SearchOption.TopDirectoryOnly);

        return files.Length > 0 ? new FilePath(files[0]) : null;
    }

    private static void AddFilesToGroup(
        DirectoryPath groupKey,
        IEnumerable<FilePath> filesToAdd,
        Dictionary<DirectoryPath, List<FilePath>> fileGroups)
    {
        if (!fileGroups.TryGetValue(groupKey, out var fileList))
        {
            fileList = [];
            fileGroups[groupKey] = fileList;
        }
        fileList.AddRange(filesToAdd);
    }
}