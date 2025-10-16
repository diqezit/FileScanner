// File: UI/Models/ProjectStatistics.cs
namespace FileScanner.Analysis.Models;

public record ProjectStatistics(
    int FileCount,
    int DirectoryCount,
    long TotalSize);