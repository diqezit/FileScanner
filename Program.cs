// File: Program.cs
namespace FileScanner;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
        ApplicationConfiguration.Initialize();

        var host = CreateHost();

        using var scope = host.Services.CreateScope();
        var form = scope.ServiceProvider.GetRequiredService<MainForm>();

        Application.Run(form);
    }

    private static IHost CreateHost() =>
        Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) => ConfigureApplicationServices(services))
            .Build();

    private static void ConfigureApplicationServices(IServiceCollection services)
    {
        var formLoggerProvider = new FormLoggerProvider();
        services.AddSingleton(formLoggerProvider);
        services.AddFileScanner();

        services.AddLogging(builder =>
        {
            builder.SetMinimumLevel(LogLevel.Debug);
            builder.AddProvider(formLoggerProvider);
            // Filter noisy logs from framework components
            builder.AddFilter("Microsoft", LogLevel.Warning);
            builder.AddFilter("System", LogLevel.Warning);
        });
    }
}