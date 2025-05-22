namespace FileScanner.Core.Services;

public sealed class TreeGenerator(IDirectoryValidator validator) : ITreeGenerator
{
    public string GenerateDirectoryTree(string rootPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rootPath);

        if (!Directory.Exists(rootPath))
            return "Папка не найдена";

        var sb = new StringBuilder();
        var rootName = Path.GetFileName(rootPath) ?? rootPath;
        sb.AppendLine($"📁 {rootName}");

        GenerateTreeRecursive(rootPath, sb, "", true);
        return sb.ToString();
    }

    private void GenerateTreeRecursive(
        string currentPath,
        StringBuilder sb,
        string prefix,
        bool isRoot)
    {
        try
        {
            var directories = Directory.GetDirectories(currentPath)
                .Where(dir => !validator.ShouldIgnoreDirectory(
                    Path.GetFileName(dir)))
                .OrderBy(Path.GetFileName)
                .ToArray();

            var files = Directory.GetFiles(currentPath)
                .Where(file => !validator.ShouldIgnoreFile(file))
                .OrderBy(Path.GetFileName)
                .ToArray();

            var totalItems = directories.Length + files.Length;
            var currentIndex = 0;

            foreach (var directory in directories)
            {
                currentIndex++;
                var isLast = currentIndex == totalItems;
                var dirName = Path.GetFileName(directory);

                sb.AppendLine($"{prefix}{(isLast ? "└── " : "├── ")}📁 {dirName}");

                var newPrefix = prefix + (isLast ? "    " : "│   ");
                GenerateTreeRecursive(directory, sb, newPrefix, false);
            }

            foreach (var file in files)
            {
                currentIndex++;
                var isLast = currentIndex == totalItems;
                var fileName = Path.GetFileName(file);
                var icon = GetFileIcon(Path.GetExtension(file));

                sb.AppendLine(
                    $"{prefix}{(isLast ? "└── " : "├── ")}{icon} {fileName}");
            }
        }
        catch (UnauthorizedAccessException)
        {
            sb.AppendLine($"{prefix}├── ❌ Нет доступа");
        }
        catch (Exception)
        {
            sb.AppendLine($"{prefix}├── ❌ Ошибка чтения");
        }
    }

    private static string GetFileIcon(string extension) => extension.ToLower() switch
    {
        ".cs" => "📄",
        ".xaml" => "🎨",
        ".json" => "📋",
        ".xml" => "📋",
        ".config" => "⚙️",
        ".csproj" => "🔧",
        ".sln" => "📦",
        ".md" => "📝",
        ".txt" => "📃",
        ".resx" => "🌐",
        _ => "📄"
    };
}