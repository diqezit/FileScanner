// UI/DependencyInjection/UIServiceExtensions.cs
namespace FileScanner.UI.DependencyInjection;

public static class UIServiceExtensions
{
    public static IServiceCollection AddUIModule(this IServiceCollection services)
    {
        // MainForm is the entry point for the UI
        // It receives all necessary dependencies to construct its Controller
        services.AddTransient<MainForm>();
        return services;
    }
}