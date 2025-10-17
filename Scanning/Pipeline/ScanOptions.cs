// File: Scanning/Pipeline/ScanOptions.cs
namespace FileScanner.Scanning.Pipeline;

// Consolidates all user-configurable scan settings
public record ScanOptions(
    bool UseProjectFilters,
    bool IsSplitEnabled,
    int ChunkSize
);