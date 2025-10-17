// Scanning/Pipeline/PROCESS_DOCUMENTATION.cs
namespace FileScanner.Scanning.Pipeline;

/// <summary>
/// THIS FILE DOCUMENTS THE COMPLETE SCANNING PROCESS
/// It exists purely for AI/developer understanding
/// </summary>
public static class ProcessDocumentation
{
    /// <summary>
    /// The scanning process transforms a project directory into a text report
    /// </summary>
    public const string ProcessOverview = """
        ==================================================
        FILE SCANNER - PROCESS DOCUMENTATION
        ==================================================
        
        PURPOSE:
        Transform a software project directory into a single text file
        containing all source code for analysis by AI or archival.
        
        INPUT: 
        - Project Directory Path (e.g., "C:\MyProject")
        - Output Directory Path (e.g., "C:\Output")
        
        OUTPUT: 
        - Multiple .txt files (one per directory/language combination)
        - One unified .txt file containing all source code
        
        TRANSFORMATION PIPELINE:
        1. Directory Path -> List of All Files
        2. All Files -> Valid Files (filtered by extension and directory)
        3. Valid Files -> Grouped by Type (C#, Python, JavaScript, etc)
        4. Grouped Files -> File Contents (read from disk with encoding)
        5. File Contents -> Text Files (one per directory/type combination)
        6. Text Files -> Single Unified File with metadata header
        
        EXAMPLE:
        Input: C:\MyProject
        Output: C:\Output\GeneratedProjectContent\_United_All_Files.txt
        """;

    /// <summary>
    /// Data flows through these transformations
    /// </summary>
    public static class DataFlow
    {
        public const string Discovery = "DirectoryPath -> IEnumerable<FilePath>";
        public const string Validation = "IEnumerable<FilePath> -> IEnumerable<ValidFilePath>";
        public const string Classification = "IEnumerable<ValidFilePath> -> Dictionary<FileType, List<FilePath>>";
        public const string Reading = "Dictionary<FileType, List<FilePath>> -> List<FileContent>";
        public const string Grouping = "List<FileContent> -> Dictionary<Directory, List<FileContent>>";
        public const string Writing = "Dictionary<Directory, List<FileContent>> -> List<TextFile>";
        public const string Unification = "List<TextFile> -> UnifiedTextFile";
    }

    /// <summary>
    /// Key services involved in the process
    /// </summary>
    public static class Services
    {
        public const string DirectoryValidator = "Filters out ignored directories like bin, obj, node_modules";
        public const string FileTypeClassifier = "Maps file extensions to language types";
        public const string FileReader = "Reads file content with proper encoding";
        public const string FileGrouper = "Groups files by their type within each directory";
        public const string FileWriter = "Writes grouped content to .txt files";
        public const string UnifiedFileWriter = "Merges all .txt files into one with metadata";
        public const string TreeGenerator = "Creates ASCII art directory tree";
        public const string ProjectStatisticsCalculator = "Counts files, directories, and total size";
    }

    /// <summary>
    /// Configuration that affects the process
    /// </summary>
    public static class Configuration
    {
        public const string IgnoredExtensions = "Files with these extensions are skipped (.exe, .dll, .pdb, etc)";
        public const string IgnoredDirectories = "These directories are not scanned (bin, obj, .git, etc)";
        public const string MaxFileSize = "Files larger than this are skipped (default: 100MB)";
        public const string MaxTreeDepth = "Directory tree generation stops at this depth (default: 50)";
        public const string BufferSize = "Buffer size for file I/O operations (default: 65536)";
    }
}