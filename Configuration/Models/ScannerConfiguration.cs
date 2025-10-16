// Configuration/Models/ScannerConfiguration.cs
namespace FileScanner.Configuration.Models;

public sealed record ScannerConfiguration
{
    public string[]? AllowedExtensions { get; init; }
    public required string[] IgnoredExtensions { get; init; }
    public required string[] IgnoredDirectories { get; init; }
    public required string DefaultProjectName { get; init; }
    public required string DefaultOutputFolderName { get; init; }
    public required string RootFileName { get; init; }
    public int MaxFileSize { get; init; } = 100 * 1024 * 1024;
    public int MaxPathLength { get; init; } = 260;
    public int MaxFileNameLength { get; init; } = 255;
    public int MaxTreeDepth { get; init; } = 50;
    public int BufferSize { get; init; } = 65536;
    public int MaxRetries { get; init; } = 3;
    public int RetryDelayMs { get; init; } = 100;
    public int MaxParallelism { get; init; } = 0;

    public static ScannerConfiguration Default => new()
    {
        AllowedExtensions = null,

        IgnoredExtensions =
        [
            ".exe", ".dll", ".pdb", ".obj", ".o", ".a", ".lib", ".so", ".dylib", ".jar", ".class", ".pyc",
            ".cache", ".binlog", ".buildlog", ".tlog", ".suo", ".user", ".DS_Store",
            ".designer.cs", ".generated.cs", ".g.cs", ".g.i.cs", ".xaml.g.cs",
            ".ico", ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".svg", ".webp", ".psd", ".ai",
            ".mp3", ".mp4", ".wav", ".avi", ".mov", ".mkv", ".flv",
            ".db", ".sqlite", ".sqlite3", ".mdb", ".accdb",
            ".zip", ".rar", ".7z", ".tar", ".gz", ".bz2", ".xz", ".pkg",
            ".bak", ".tmp", ".temp", ".log", ".swp", "~",
            ".nupkg", ".snupkg", ".nuspec",
            ".pfx", ".snk", ".cer", ".crt", ".key", ".pem", ".jks"
        ],

        IgnoredDirectories =
        [
            "bin", "obj", "out", "build", "dist", "target", "x64",
            ".git", ".svn", ".hg", ".idea",
            ".vs", ".vscode", ".rider",
            "node_modules", "bower_components", "vendor", "packages", "Pods", "Carthage",
            "venv", ".venv", "env", ".env", "__pycache__",
            "TestResults", "$RECYCLE.BIN", "System Volume Information", "tmp", "temp",
            "GeneratedProjectContent", "coverage"
        ],

        DefaultProjectName = "*.csproj",
        DefaultOutputFolderName = "GeneratedProjectContent",
        RootFileName = "Project_Root"
    };

    public Dictionary<string, string> FileTypeMapping { get; init; } = new
        (StringComparer.OrdinalIgnoreCase)
    {
        // .NET Ecosystem
        [".cs"] = FileTypes.CSharp,
        [".fs"] = FileTypes.FSharp,
        [".vb"] = FileTypes.VbNet,
        [".csproj"] = FileTypes.Project,
        [".fsproj"] = FileTypes.Project,
        [".vbproj"] = FileTypes.Project,
        [".sln"] = FileTypes.Solution,
        [".xaml"] = FileTypes.Xaml,
        [".razor"] = FileTypes.Razor,
        [".cshtml"] = FileTypes.Razor,
        [".rc"] = FileTypes.Resource,

        // C-like Languages
        [".h"] = FileTypes.CPlusPlusHeader,
        [".hpp"] = FileTypes.CPlusPlusHeader,
        [".hh"] = FileTypes.CPlusPlusHeader,
        [".cpp"] = FileTypes.CPlusPlusSource,
        [".cc"] = FileTypes.CPlusPlusSource,
        [".cxx"] = FileTypes.CPlusPlusSource,
        [".c"] = FileTypes.CSource,
        [".m"] = FileTypes.ObjectiveC,
        [".mm"] = FileTypes.ObjectiveC,

        // Modern Systems Languages
        [".go"] = FileTypes.Go,
        [".rs"] = FileTypes.Rust,
        [".swift"] = FileTypes.Swift,
        [".dart"] = FileTypes.Dart,
        [".zig"] = FileTypes.Zig,

        // JVM Ecosystem
        [".java"] = FileTypes.Java,
        [".kt"] = FileTypes.Kotlin,
        [".kts"] = FileTypes.Kotlin,
        [".scala"] = FileTypes.Scala,
        [".groovy"] = FileTypes.Groovy,
        [".gradle"] = FileTypes.Gradle,
        ["pom.xml"] = FileTypes.Maven,

        // Web Frontend
        [".js"] = FileTypes.JavaScript,
        [".mjs"] = FileTypes.JavaScript,
        [".ts"] = FileTypes.TypeScript,
        [".tsx"] = FileTypes.TypeScript,
        [".html"] = FileTypes.Html,
        [".htm"] = FileTypes.Html,
        [".css"] = FileTypes.Css,
        [".scss"] = FileTypes.Sass,
        [".sass"] = FileTypes.Sass,
        [".less"] = FileTypes.Less,
        [".vue"] = FileTypes.Vue,
        [".svelte"] = FileTypes.Svelte,
        [".ejs"] = FileTypes.Ejs,
        [".pug"] = FileTypes.Pug,

        // Web Backend & Scripting
        [".py"] = FileTypes.Python,
        [".rb"] = FileTypes.Ruby,
        [".php"] = FileTypes.Php,
        [".pl"] = FileTypes.Perl,
        [".lua"] = FileTypes.Lua,
        [".ex"] = FileTypes.Elixir,
        [".exs"] = FileTypes.Elixir,

        // Shell & Automation
        [".sh"] = FileTypes.Shell,
        [".bash"] = FileTypes.Shell,
        [".zsh"] = FileTypes.Shell,
        [".ps1"] = FileTypes.PowerShell,
        [".bat"] = FileTypes.Batch,
        [".cmd"] = FileTypes.Batch,
        ["Makefile"] = FileTypes.Makefile,

        // Functional Languages
        [".hs"] = FileTypes.Haskell,
        [".lisp"] = FileTypes.Lisp,
        [".clj"] = FileTypes.Clojure,
        [".erl"] = FileTypes.Erlang,

        // Data, Schemas & Config
        [".json"] = FileTypes.Json,
        [".xml"] = FileTypes.Xml,
        [".yml"] = FileTypes.Yaml,
        [".yaml"] = FileTypes.Yaml,
        [".toml"] = FileTypes.Toml,
        [".ini"] = FileTypes.Ini,
        [".properties"] = FileTypes.Properties,
        [".config"] = FileTypes.Config,
        [".env"] = FileTypes.Env,
        [".sql"] = FileTypes.Sql,
        [".graphql"] = FileTypes.GraphQL,
        [".gql"] = FileTypes.GraphQL,
        [".proto"] = FileTypes.Protobuf,
        [".avsc"] = FileTypes.Avro,

        // Infrastructure & Build
        ["Dockerfile"] = FileTypes.Docker,
        [".dockerfile"] = FileTypes.Docker,
        [".tf"] = FileTypes.Terraform,
        ["CMakeLists.txt"] = FileTypes.Cmake,
        ["nginx.conf"] = FileTypes.Nginx,
        ["Jenkinsfile"] = FileTypes.Jenkinsfile,

        // Scientific & Stats
        [".r"] = FileTypes.RLang,
        [".jl"] = FileTypes.Julia,
        [".m"] = FileTypes.Matlab, // Note: conflicts with Objective-C, filename check needed if specific

        // Documentation
        [".txt"] = FileTypes.Text,
        [".md"] = FileTypes.Markdown,
        [".rst"] = FileTypes.Rst,
        [".adoc"] = FileTypes.AsciiDoc,
    };
}