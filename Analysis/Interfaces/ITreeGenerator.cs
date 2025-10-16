// Analysis/Interfaces/ITreeGenerator.cs
namespace FileScanner.Analysis.Interfaces;

public interface ITreeGenerator
{
    string GenerateDirectoryTree(DirectoryPath rootPath);
}