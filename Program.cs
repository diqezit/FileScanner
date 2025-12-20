namespace FileScanner;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
        ApplicationConfiguration.Initialize();

        using var sp = BuildServices();
        using var scope = sp.CreateScope();

        Application.Run(scope.ServiceProvider.GetRequiredService<MainForm>());
    }

    static ServiceProvider BuildServices()
    {
        var s = new ServiceCollection();

        // Config
        s.AddSingleton(ScannerConfiguration.Default);
        s.AddSingleton<SettingsManager>();

        // Core
        s.AddSingleton<FileSystemServices>();
        s.AddSingleton<ProjectAnalyzer>();
        s.AddSingleton<FormLoggerProvider>();
        s.AddScoped<ScanEngine>();

        // UI
        s.AddTransient<MainForm>();

        // Logging
        s.AddLogging(b =>
        {
            b.SetMinimumLevel(LogLevel.Debug);
            b.AddFilter("Microsoft", LogLevel.Warning);
            b.AddFilter("System", LogLevel.Warning);
            b.Services.AddSingleton<ILoggerProvider>(
                p => p.GetRequiredService<FormLoggerProvider>());
        });

        return s.BuildServiceProvider();
    }
}