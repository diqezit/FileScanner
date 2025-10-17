// Scanning/Pipeline/ScanningPipeline.cs
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
        PostScanOptions options,
        CancellationToken cancellationToken = default)
    {
        var result = new PipelineResult();

        if (!ValidatePaths(projectPath, result))
            return result;

        var projectDir = new DirectoryPath(projectPath);
        var outputDir = CreateOutputDirectory(outputPath);

        GenerateProjectTree(projectDir, result);

        await CalculateStatistics(
            projectDir,
            result,
            cancellationToken);

        if (!await ProcessProjectFiles(
            projectDir,
            outputDir,
            result,
            cancellationToken))
        {
            return result;
        }

        await CreateUnifiedFile(
            projectDir,
            outputDir,
            result,
            cancellationToken);

        if (options.IsSplitEnabled)
        {
            await SplitUnifiedFile(
                outputDir,
                options.ChunkSize,
                result,
                cancellationToken);
        }

        FinalizeSuccessResult(outputDir, result);
        return result;
    }

    private static bool ValidatePaths(
        string projectPath,
        PipelineResult result)
    {
        result.AddStep("Validating paths");

        if (Directory.Exists(projectPath))
            return true;

        result.AddStep(
            $"ERROR: Project path does not exist: {projectPath}");
        result.ErrorMessage = "Project directory not found";
        return false;
    }

    private static DirectoryPath CreateOutputDirectory(string outputPath) =>
        new(Path.Combine(outputPath, "GeneratedProjectContent"));

    private void GenerateProjectTree(
        DirectoryPath projectDir,
        PipelineResult result)
    {
        result.AddStep("Generating project tree");
        try
        {
            result.ProjectTree = treeGenerator.GenerateDirectoryTree(projectDir);
        }
        catch (Exception ex)
        {
            LogWarning(result, "Could not generate tree", ex);
        }
    }

    private async Task CalculateStatistics(
        DirectoryPath projectDir,
        PipelineResult result,
        CancellationToken cancellationToken)
    {
        result.AddStep("Calculating statistics");
        try
        {
            result.Statistics = await statsCalculator.CalculateAsync(
                projectDir,
                cancellationToken);
        }
        catch (Exception ex)
        {
            LogWarning(result, "Could not calculate stats", ex);
        }
    }

    private async Task<bool> ProcessProjectFiles(
        DirectoryPath projectDir,
        DirectoryPath outputDir,
        PipelineResult result,
        CancellationToken cancellationToken)
    {
        result.AddStep("Processing project files");

        var success = await projectProcessor.ProcessProjectAsync(
            projectDir,
            outputDir,
            cancellationToken);

        if (success)
            return true;

        LogError(result, "Processing failed", "File processing failed");
        return false;
    }

    private async Task CreateUnifiedFile(
        DirectoryPath projectDir,
        DirectoryPath outputDir,
        PipelineResult result,
        CancellationToken cancellationToken)
    {
        result.AddStep("Creating unified file");
        try
        {
            await unifiedFileWriter.WriteUnifiedFileAsync(
                projectDir,
                outputDir,
                cancellationToken);
        }
        catch (Exception ex)
        {
            LogError(result, "Could not create unified file", ex.Message);
        }
    }

    private async Task SplitUnifiedFile(
        DirectoryPath outputDir,
        int chunkSize,
        PipelineResult result,
        CancellationToken cancellationToken)
    {
        result.AddStep("Splitting unified file");

        var unifiedFilePath = new FilePath(
            Path.Combine(outputDir.Value, "_United_All_Files.txt"));

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

    private static void LogWarning(
        PipelineResult result,
        string message,
        Exception ex) =>
        result.AddStep($"WARNING: {message}: {ex.Message}");

    private static void LogError(
        PipelineResult result,
        string step,
        string errorMessage)
    {
        result.AddStep($"ERROR: {step}");
        result.ErrorMessage = errorMessage;
    }
}