// Configuration/DependencyInjection/ConfigurationServiceExtensions.cs
namespace FileScanner.Configuration.DependencyInjection;

public static class ConfigurationServiceExtensions
{
    public static IServiceCollection AddConfigurationModule(this IServiceCollection services)
    {
        services.AddSingleton(sp =>
            Options.Create(ScannerConfiguration.Default));

        services.AddSingleton<ISettingsPathProvider, LocalSettingsPathProvider>();
        services.AddSingleton<IUserSettingsService, UserSettingsService>();

        return services;
    }
}