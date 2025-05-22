namespace FileScanner;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        // Set high DPI mode with API instead of manifest
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);

        ApplicationConfiguration.Initialize();

        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // Регистрация сервисов FileScanner
                services.AddFileScanner();

                // Настройка логирования
                services.AddLogging(builder =>
                {
                    builder.SetMinimumLevel(LogLevel.Debug);
                    builder.AddProvider(new FormLoggerProvider());
                    builder.AddFilter("Microsoft", LogLevel.Warning);
                    builder.AddFilter("System", LogLevel.Warning);
                });
            })
            .Build();

        // Создаем scope для получения scoped сервисов
        using var scope = host.Services.CreateScope();
        var form = scope.ServiceProvider.GetRequiredService<MainForm>();

        Application.Run(form);
    }
}