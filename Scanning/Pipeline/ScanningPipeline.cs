#nullable enable

namespace FileScanner.Scanning.Pipeline;

public class ScanningPipeline(
    IProjectProcessor projectProcessor,
    IUnifiedFileWriter unifiedFileWriter,
    ITreeGenerator treeGenerator,
    IProjectStatisticsCalculator statsCalculator,
    IFileSplitter fileSplitter)
{
    public async Task<PipelineResult> ExecuteAsync(
        string projectPath,
        string outputPath,
        ScanOptions options,
        CancellationToken cancellationToken = default)
    {
        var result = new PipelineResult();

        if (!ValidatePaths(projectPath, result))
            return result;

        var projectDir = new DirectoryPath(projectPath);
        var outputDir = CreateOutputDirectory(outputPath);
        result.AddStep("Paths validated");

        var projectStructure = projectProcessor.EnumerateProject(
            projectDir,
            options.UseProjectFilters);

        result.AddStep(
            $"Project enumerated. Strategy: {(options.UseProjectFilters ? "Logical Filters" : "Physical Dirs")}");

        var stats = await statsCalculator.CalculateAsync(
            projectStructure,
            cancellationToken);

        var tree = treeGenerator.GenerateDirectoryTree(
            projectStructure,
            projectDir);

        var metadataHeader = MetadataHeaderFormatter.Format(tree, stats);
        result.AddStep("Metadata generated");
        result.ProjectTree = tree;
        result.Statistics = stats;
        cancellationToken.ThrowIfCancellationRequested();

        var success = await projectProcessor.ProcessProjectFilesAsync(
            projectStructure,
            projectDir,
            outputDir,
            cancellationToken);

        if (!success)
        {
            LogError(
                result,
                "Processing failed",
                "Intermediate file creation failed");
            return result;
        }
        result.AddStep("Intermediate files created");

        await unifiedFileWriter.WriteUnifiedFileAsync(
            metadataHeader,
            outputDir,
            cancellationToken);

        result.AddStep("Unified file created");

        if (options.IsSplitEnabled)
            await SplitUnifiedFile(
                outputDir,
                options.ChunkSize,
                result,
                cancellationToken);

        FinalizeSuccessResult(outputDir, result);
        return result;
    }

    private static bool ValidatePaths(string projectPath, PipelineResult result)
    {
        result.AddStep("Validating paths");
        if (Directory.Exists(projectPath))
            return true;

        result.AddStep($"ERROR: Project path does not exist: {projectPath}");
        result.ErrorMessage = "Project directory not found";
        return false;
    }

    private static DirectoryPath CreateOutputDirectory(string outputPath) =>
        new(Path.Combine(outputPath, "GeneratedProjectContent"));

    private async Task SplitUnifiedFile(
        DirectoryPath outputDir,
        int chunkSize,
        PipelineResult result,
        CancellationToken cancellationToken)
    {
        result.AddStep("Splitting unified file");

        var unifiedFilePath = new FilePath(Path.Combine(
            outputDir.Value,
            "_United_All_Files.txt"));

        await fileSplitter.SplitFileAsync(
            unifiedFilePath,
            chunkSize,
            cancellationToken);
    }

    private static void FinalizeSuccessResult(
        DirectoryPath outputDir,
        PipelineResult result)
    {
        result.AddStep("Completed successfully");
        result.OutputDirectory = outputDir.Value;
        result.IsSuccess = true;
    }

    private static void LogError(
        PipelineResult result,
        string step,
        string errorMessage)
    {
        result.AddStep($"ERROR: {step}");
        result.ErrorMessage = errorMessage;
    }
}