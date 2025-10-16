// DependencyInjection/ServiceCollectionExtensions.cs
namespace FileScanner.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFileScanner(this IServiceCollection services) =>
        services
            .AddConfigurationModule()
            .AddAnalysisModule()
            .AddFileOperationsModule()
            .AddPathManagementModule()
            .AddScanningModule()
            .AddUIModule();
}