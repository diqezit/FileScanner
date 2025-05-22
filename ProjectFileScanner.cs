// ProjectFileScanner.cs
namespace FileScanner;

public class ProjectFileScanner(ILogger logger) : IFileScanner
{
    private string _currentProjectRootDirectory;

    private static readonly HashSet<string> _ignoredExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        ".csproj.user", ".suo", ".user", ".exe", ".dll", ".pdb", ".config", ".binlog", ".vspscc",
        ".designer.cs", ".ico", ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".db", ".sqlite",
        ".bak", ".tmp", ".log", ".json", ".xml", ".zip", ".rar", ".7z", ".txt", ".md", ".xaml", ".resources"
    };

    private static readonly HashSet<string> _ignoredDirectoryNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "bin", "obj", ".git", ".vs", "node_modules", "packages", "wwwroot", "dist"
    };

    public void ScanAndGenerate(string projectRootDirectory, string outputDirectory)
    {
        _currentProjectRootDirectory = projectRootDirectory;
        EnsureOutputDirectoryReady(outputDirectory);
        ProcessDirectory(projectRootDirectory, outputDirectory, "");
    }

    private void EnsureOutputDirectoryReady(string outputDirectory)
    {
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
            logger.Log($"Создана выходная папка: {outputDirectory}");
        }
        else
        {
            logger.Log("Выходная папка уже существует. Очищаем ее содержимое...");
            foreach (var file in Directory.GetFiles(outputDirectory, "*.txt"))
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    logger.Log($"Ошибка при удалении файла {file}: {ex.Message}");
                }
            }
        }
    }

    private void ProcessDirectory(string currentPath, string outputDirectory, string prefix)
    {
        var directoryInfo = new DirectoryInfo(currentPath);

        if (_ignoredDirectoryNames.Contains(directoryInfo.Name))
        {
            logger.Log($"Игнорируем папку: {Path.GetRelativePath(_currentProjectRootDirectory, currentPath)}");
            return;
        }

        List<string> fileContents = new List<string>();

        foreach (string file in Directory.GetFiles(currentPath))
        {
            string fileName = Path.GetFileName(file);
            string fileExtension = Path.GetExtension(file);

            if (_ignoredExtensions.Contains(fileExtension))
            {
                logger.Log($"Игнорируем файл: {Path.GetRelativePath(_currentProjectRootDirectory, file)}");
                continue;
            }

            try
            {
                fileContents.Add($"\n--- Файл: {Path.GetRelativePath(_currentProjectRootDirectory, file)} ---\n");
                fileContents.Add(File.ReadAllText(file));
            }
            catch (Exception ex)
            {
                fileContents.Add($"\n--- Ошибка чтения файла {Path.GetRelativePath(_currentProjectRootDirectory, file)}: {ex.Message} ---\n");
                logger.Log($"Ошибка чтения файла {Path.GetRelativePath(_currentProjectRootDirectory, file)}: {ex.Message}");
            }
        }

        if (fileContents.Any())
        {
            string outputFileName = GetOutputFileName(currentPath, _currentProjectRootDirectory, prefix);
            string fullOutputFilePath = Path.Combine(outputDirectory, $"{outputFileName}.txt");

            string dirForOutputFile = Path.GetDirectoryName(fullOutputFilePath);
            if (!Directory.Exists(dirForOutputFile))
            {
                Directory.CreateDirectory(dirForOutputFile);
            }

            File.AppendAllLines(fullOutputFilePath, fileContents);
            logger.Log($"Сгенерирован файл: {Path.GetFileName(fullOutputFilePath)}");
        }

        foreach (string directory in Directory.GetDirectories(currentPath))
        {
            string newPrefix = GetNewPrefixForSubdirectory(currentPath, directory, _currentProjectRootDirectory, prefix);
            ProcessDirectory(directory, outputDirectory, newPrefix);
        }
    }

    private string GetOutputFileName(string currentPath, string projectRootDirectory, string currentPrefix)
    {
        if (currentPath.Equals(projectRootDirectory, StringComparison.OrdinalIgnoreCase))
        {
            return "Project_Root";
        }

        string currentDirName = Path.GetFileName(currentPath);

        if (string.IsNullOrEmpty(currentPrefix))
        {
            return currentDirName.Replace(".", "_");
        }
        else
        {
            return $"{currentPrefix}_{currentDirName.Replace(".", "_")}";
        }
    }

    private string GetNewPrefixForSubdirectory(string parentPath, string subDirectoryPath, string projectRootDirectory, string currentPrefix)
    {
        string subDirName = Path.GetFileName(subDirectoryPath);

        if (parentPath.Equals(projectRootDirectory, StringComparison.OrdinalIgnoreCase))
        {
            return subDirName.Replace(".", "_");
        }
        else
        {
            if (string.IsNullOrEmpty(currentPrefix))
            {
                return Path.GetFileName(parentPath).Replace(".", "_");
            }
            else
            {
                return $"{currentPrefix}_{subDirName.Replace(".", "_")}";
            }
        }
    }

    public string FindProjectRoot(string startPath, string projectName)
    {
        string currentDir = startPath;
        while (currentDir != null && !currentDir.Equals(Path.GetPathRoot(currentDir), StringComparison.OrdinalIgnoreCase))
        {
            if (Directory.GetFiles(currentDir, projectName).Any())
            {
                return currentDir;
            }
            try
            {
                currentDir = Directory.GetParent(currentDir)?.FullName;
            }
            catch (Exception ex)
            {
                logger.Log($"Ошибка при поиске корня проекта: {ex.Message}");
                break;
            }
        }
        return null;
    }
}