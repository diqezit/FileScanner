// Scanning/DependencyInjection/ScanningServiceExtensions.cs
namespace FileScanner.Scanning.DependencyInjection;

public static class ScanningServiceExtensions
{
    public static IServiceCollection AddScanningModule(this IServiceCollection services)
    {
        // Core scanning services are tied to a single operation scope
        services.AddScoped<IFileContentAggregator, FileContentAggregator>();
        services.AddScoped<IFileGrouper, FileGrouper>();
        services.AddScoped<IDirectoryProcessor, DirectoryProcessor>();
        services.AddScoped<IFileProcessor, FileProcessor>();

        // The main scanning service implementation
        services.AddScoped<IFileScanner, FileScannerService>();

        // Orchestrator is lightweight and created for each scan
        services.AddTransient<ScanOrchestrator>();

        return services;
    }
}