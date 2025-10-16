// UI/DependencyInjection/UIServiceExtensions.cs
namespace FileScanner.UI.DependencyInjection;

public static class UIServiceExtensions
{
    public static IServiceCollection AddUIModule(this IServiceCollection services)
    {
        services.AddTransient<MainForm>();
        return services;
    }
}