// FileScanner.Core/Interfaces/IFileReader.cs
namespace FileScanner.FileOperations.Interfaces;

public interface IFileReader
{
    Task<FileContent> ReadFileAsync(string filePath, string rootPath, CancellationToken cancellationToken);
}

public record FileContent(string RelativePath, string Content, bool IsSuccess);