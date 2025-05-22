// IFileScanner.cs
namespace FileScanner;

public interface IFileScanner
{
    void ScanAndGenerate(string projectRootDirectory, string outputDirectory);
    string FindProjectRoot(string startPath, string projectName);
}