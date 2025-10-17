// FileOperations/Interfaces/IFileReader.cs
namespace FileScanner.FileOperations.Interfaces;

public interface IFileReader
{
    Task<FileContent> ReadFileAsync(
        FilePath filePath,
        DirectoryPath rootPath,
        CancellationToken cancellationToken);
}

public record FileContent(
    RelativePath RelativePath,
    string Content,
    bool IsSuccess,
    string? ErrorReason = null
);