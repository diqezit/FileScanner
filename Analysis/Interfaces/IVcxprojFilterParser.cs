// File: Analysis/Interfaces/IVcxprojFilterParser.cs
namespace FileScanner.Analysis.Interfaces;

// Defines a contract for parsing Visual Studio project filter files
public interface IVcxprojFilterParser
{
    // Returns a map of full file paths to their logical filter paths
    Dictionary<string, string> Parse(FilePath filterFilePath);
}