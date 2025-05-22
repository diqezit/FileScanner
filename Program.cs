namespace FileScanner;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        var host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddFileScanner();
                services.AddLogging();
            })
            .Build();

        var form = host.Services.GetRequiredService<MainForm>();
        Application.Run(form);
    }
}