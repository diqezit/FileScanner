// Scanning/DependencyInjection/ScanningServiceExtensions.cs
namespace FileScanner.Scanning.DependencyInjection;

public static class ScanningServiceExtensions
{
    public static IServiceCollection AddScanningModule(this IServiceCollection services)
    {
        // Core scanning services are tied to a single operation scope
        services.AddScoped<IFileContentAggregator, FileContentAggregator>();
        services.AddScoped<IFileGrouper, FileGrouper>();

        // Register our new, unified service for project processing
        services.AddScoped<IProjectProcessor, ProjectProcessorService>();

        // Orchestrator for creating the pipeline
        services.AddTransient<ScanOrchestrator>();

        return services;
    }
}