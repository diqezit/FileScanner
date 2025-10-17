// FileOperations/Interfaces/IFileSplitter.cs
namespace FileScanner.FileOperations.Interfaces;

public interface IFileSplitter
{
    Task SplitFileAsync(
        FilePath sourceFile,
        long chunkSizeInChars,
        CancellationToken cancellationToken);
}