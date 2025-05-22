namespace FileScanner.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFileScanner(this IServiceCollection services)
    {
        services.AddSingleton<IOptions<ScannerConfiguration>>(provider =>
            Options.Create(ScannerConfiguration.Default));

        services.AddSingleton<IProjectPathResolver, ProjectPathResolver>();
        services.AddSingleton<IDirectoryValidator, DirectoryValidator>();
        services.AddSingleton<IOutputFileNameGenerator, OutputFileNameGenerator>();
        services.AddSingleton<IFileTypeClassifier, FileTypeClassifier>();
        services.AddSingleton<IFileProcessor, FileProcessor>();
        services.AddSingleton<IFileScanner, ProjectFileScanner>();
        services.AddSingleton<ITreeGenerator, TreeGenerator>();

        services.AddTransient<MainForm>();

        return services;
    }
}