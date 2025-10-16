// Analysis/Interfaces/IFileTypeClassifier.cs
namespace FileScanner.Analysis.Interfaces;

public interface IFileTypeClassifier
{
    string ClassifyFile(FilePath filePath);
    IEnumerable<string> GetAllFileTypes();
}