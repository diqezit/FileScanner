// File: PathManagement/DependencyInjection/PathManagementServiceExtensions.cs
namespace FileScanner.PathManagement.DependencyInjection;

public static class PathManagementServiceExtensions
{
    public static IServiceCollection AddPathManagementModule(this IServiceCollection services)
    {
        // Stateless helper services, safe as singletons
        services.AddSingleton<IOutputFileNameGenerator, OutputFileNameGenerator>();
        services.AddSingleton<IProjectPathResolver, ProjectPathResolver>();
        services.AddSingleton<IDefaultPathProvider, DefaultPathProvider>();

        return services;
    }
}