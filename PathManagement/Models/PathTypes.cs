// PathManagement/Models/PathTypes.cs
namespace FileScanner.PathManagement.Models;

// Using a class-based record allows for custom constructors while keeping immutability.
public readonly record struct DirectoryPath
{
    public string Value { get; }

    public DirectoryPath(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = Path.GetFullPath(value);
    }
}

public readonly record struct FilePath
{
    public string Value { get; }

    public FilePath(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = Path.GetFullPath(value);
    }
}

public readonly record struct RelativePath
{
    public string Value { get; }

    public RelativePath(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value.Replace('\\', '/');
    }
}