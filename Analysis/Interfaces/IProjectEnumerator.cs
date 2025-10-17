// Analysis/Interfaces/IProjectEnumerator.cs
namespace FileScanner.Analysis.Interfaces;

public record ProjectEnumerationResult(
    IEnumerable<FilePath> Files,
    IEnumerable<DirectoryPath> Directories);

public interface IProjectEnumerator
{
    ProjectEnumerationResult EnumerateProject(DirectoryPath projectRoot);
}