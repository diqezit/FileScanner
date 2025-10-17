// FileOperations/DependencyInjection/FileOperationServiceExtensions.cs
namespace FileScanner.FileOperations.DependencyInjection;

public static class FileOperationServiceExtensions
{
    public static IServiceCollection AddFileOperationsModule(this IServiceCollection services)
    {
        services.AddScoped<IFileReader, FileReader>();
        services.AddScoped<IFileWriter, FileWriter>();
        services.AddScoped<IUnifiedFileWriter, UnifiedFileWriter>();
        services.AddScoped<IOutputDirectoryCleaner, OutputDirectoryCleaner>();
        services.AddScoped<IFileSplitter, FileSplitter>();

        return services;
    }
}