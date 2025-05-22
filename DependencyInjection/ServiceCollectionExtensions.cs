// DependencyInjection/ServiceCollectionExtensions.cs
namespace FileScanner.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFileScanner(
        this IServiceCollection services)
    {
        services.AddConfiguration();
        services.AddAnalysisServices();
        services.AddFileOperationServices();
        services.AddPathManagementServices();
        services.AddScanningServices();
        services.AddUI();

        return services;
    }

    private static void AddConfiguration(this IServiceCollection services)
    {
        services.AddSingleton(sp =>
            Options.Create(ScannerConfiguration.Default));

        services.AddSingleton<IUserSettingsService, UserSettingsService>();
    }

    private static void AddAnalysisServices(this IServiceCollection services)
    {
        services.AddSingleton<IDirectoryValidator, DirectoryValidator>();
        services.AddSingleton<IFileTypeClassifier, FileTypeClassifier>();
        services.AddSingleton<ITreeGenerator, TreeGenerator>();
    }

    private static void AddFileOperationServices(this IServiceCollection services)
    {
        services.AddScoped<IFileReader, FileReader>();
        services.AddScoped<IFileWriter, FileWriter>();
        services.AddScoped<IUnifiedFileWriter, UnifiedFileWriter>();
        services.AddScoped<IOutputDirectoryCleaner, OutputDirectoryCleaner>();
    }

    private static void AddPathManagementServices(this IServiceCollection services)
    {
        services.AddSingleton<IOutputFileNameGenerator, OutputFileNameGenerator>();
        services.AddSingleton<IProjectPathResolver, ProjectPathResolver>();
    }

    private static void AddScanningServices(this IServiceCollection services)
    {
        services.AddScoped<IFileGrouper, FileGrouper>();
        services.AddScoped<IDirectoryProcessor, DirectoryProcessor>();
        services.AddScoped<IFileProcessor, FileProcessor>();
        services.AddScoped<IFileScanner, ProjectFileScanner>();
    }

    private static void AddUI(this IServiceCollection services)
    {
        services.AddTransient<MainForm>();
    }
}