// Analysis/Services/ProjectEnumerator.cs
namespace FileScanner.Analysis.Services;

public sealed class ProjectEnumerator(IDirectoryValidator validator) : IProjectEnumerator
{
    public ProjectEnumerationResult EnumerateProject(DirectoryPath projectRoot)
    {
        var files = new List<FilePath>();
        var directories = new List<DirectoryPath>();

        // The recursive walk starts here, populating the lists
        WalkDirectoryTree(
            projectRoot,
            files,
            directories);

        return new ProjectEnumerationResult(
            files,
            directories);
    }

    // Recursively traverses file system to find all valid items
    // This encapsulates complex traversal logic in one place
    private void WalkDirectoryTree(
        DirectoryPath currentDir,
        ICollection<FilePath> files,
        ICollection<DirectoryPath> directories)
    {
        var directoryName = Path.GetFileName(currentDir.Value);

        // Prune ignored branches early for performance
        if (validator.ShouldIgnoreDirectory(directoryName))
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
            {
                WalkDirectoryTree(
                    new DirectoryPath(subDir),
                    files,
                    directories);
            }
        }
        catch (UnauthorizedAccessException)
        {
            // Silently ignore inaccessible directories
            // Prevents a single permission error from failing entire scan
        }
    }
}