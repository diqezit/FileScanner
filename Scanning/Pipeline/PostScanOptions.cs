// Scanning/Pipeline/PostScanOptions.cs
namespace FileScanner.Scanning.Pipeline;

public record PostScanOptions(
    bool IsSplitEnabled,
    int ChunkSize);