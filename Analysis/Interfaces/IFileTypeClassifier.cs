// FileScanner.Core/Interfaces/IFileTypeClassifier.cs
namespace FileScanner.Analysis.Interfaces;

public interface IFileTypeClassifier
{
    string ClassifyFile(string filePath);
    IEnumerable<string> GetAllFileTypes();
}