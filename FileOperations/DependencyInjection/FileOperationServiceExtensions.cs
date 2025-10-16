// File: FileOperations/DependencyInjection/FileOperationServiceExtensions.cs
namespace FileScanner.FileOperations.DependencyInjection;

public static class FileOperationServiceExtensions
{
    public static IServiceCollection AddFileOperationsModule(this IServiceCollection services)
    {
        // These services are stateful per operation, use scoped lifetime
        services.AddScoped<IFileReader, FileReader>();
        services.AddScoped<IFileWriter, FileWriter>();
        services.AddScoped<IUnifiedFileWriter, UnifiedFileWriter>();
        services.AddScoped<IOutputDirectoryCleaner, OutputDirectoryCleaner>();

        return services;
    }
}