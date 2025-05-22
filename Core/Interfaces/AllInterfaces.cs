namespace FileScanner.Core.Interfaces;

public interface IFileScanner
{
    Task ScanAndGenerateAsync(
        string projectRootDirectory,
        string outputDirectory,
        CancellationToken cancellationToken = default);
}

public interface IProjectPathResolver
{
    string? FindProjectRoot(string startPath, string projectFileName);
}

public interface IFileProcessor
{
    Task<bool> ProcessDirectoryAsync(
        string directoryPath,
        string outputDirectory,
        CancellationToken cancellationToken = default);
}

public interface IOutputFileNameGenerator
{
    string GenerateFileName(
        string directoryPath,
        string projectRootPath,
        string fileType);
}

public interface IDirectoryValidator
{
    bool ShouldIgnoreDirectory(string directoryName);
    bool ShouldIgnoreFile(string filePath);
}

public interface ITreeGenerator
{
    string GenerateDirectoryTree(string rootPath);
}

public interface IFileTypeClassifier
{
    string ClassifyFile(string filePath);
    IEnumerable<string> GetAllFileTypes();
}