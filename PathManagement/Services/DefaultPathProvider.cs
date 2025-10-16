// PathManagement/Services/DefaultPathProvider.cs
namespace FileScanner.PathManagement.Services;

public sealed class DefaultPathProvider(
    IProjectPathResolver pathResolver,
    IOptions<ScannerConfiguration> options) : IDefaultPathProvider
{
    private readonly string _defaultProjectName = options.Value.DefaultProjectName;

    public DirectoryPath GetDefaultProjectPath()
    {
        var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        return pathResolver.FindProjectRoot(currentDirectory, _defaultProjectName)
            ?? new DirectoryPath(currentDirectory);
    }

    public DirectoryPath GetDefaultOutputPath(DirectoryPath projectPath)
    {
        var projectRoot = pathResolver.FindProjectRoot(projectPath.Value, _defaultProjectName);
        if (projectRoot is not null)
            return projectRoot.Value;

        if (Directory.Exists(projectPath.Value))
            return projectPath;

        return new DirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
    }
}