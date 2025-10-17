// Scanning/Pipeline/PipelineResult.cs
namespace FileScanner.Scanning.Pipeline;

public class PipelineResult
{
    public bool IsSuccess { get; set; }
    public string? OutputDirectory { get; set; }
    public string? ProjectTree { get; set; }
    public ProjectStatistics? Statistics { get; set; }
    public string? ErrorMessage { get; set; }
    public List<string> ExecutedSteps { get; } = [];

    public void AddStep(string stepName) =>
        ExecutedSteps.Add($"{DateTime.Now:HH:mm:ss.fff} - {stepName}");

    public string GetStepsAsString() =>
        string.Join(Environment.NewLine, ExecutedSteps);
}