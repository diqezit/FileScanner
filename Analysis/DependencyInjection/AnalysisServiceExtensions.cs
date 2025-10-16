// File: Analysis/DependencyInjection/AnalysisServiceExtensions.cs
namespace FileScanner.Analysis.DependencyInjection;

public static class AnalysisServiceExtensions
{
    public static IServiceCollection AddAnalysisModule(this IServiceCollection services)
    {
        // Stateless services, safe as singletons
        services.AddSingleton<IDirectoryValidator, DirectoryValidator>();
        services.AddSingleton<IFileTypeClassifier, FileTypeClassifier>();
        services.AddSingleton<ITreeGenerator, TreeGenerator>();

        // Stateless calculator, perfect for transient
        services.AddTransient<IProjectStatisticsCalculator, ProjectStatisticsCalculator>();

        return services;
    }
}