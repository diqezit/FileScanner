// Analysis/Models/FileTypes.cs
namespace FileScanner.Analysis.Models;

// Provides strongly-typed constants for file classifications
public static class FileTypes
{
    // .NET Ecosystem
    public const string CSharp = "csharp";
    public const string FSharp = "fsharp";
    public const string VbNet = "vbnet";
    public const string Project = "project";
    public const string Solution = "solution";
    public const string Razor = "razor";
    public const string Xaml = "xaml";
    public const string Resource = "resource";

    // C-like Languages
    public const string CPlusPlusHeader = "cplusplus_header";
    public const string CPlusPlusSource = "cplusplus_source";
    public const string CSource = "c_source";
    public const string ObjectiveC = "objective_c";

    // Modern Systems Languages
    public const string Go = "go";
    public const string Rust = "rust";
    public const string Swift = "swift";
    public const string Dart = "dart";
    public const string Zig = "zig";

    // JVM Ecosystem
    public const string Java = "java";
    public const string Kotlin = "kotlin";
    public const string Scala = "scala";
    public const string Groovy = "groovy";
    public const string Gradle = "gradle";
    public const string Maven = "maven";

    // Web Frontend
    public const string JavaScript = "javascript";
    public const string TypeScript = "typescript";
    public const string Html = "html";
    public const string Css = "css";
    public const string Sass = "sass";
    public const string Less = "less";
    public const string Vue = "vue";
    public const string Svelte = "svelte";
    public const string Ejs = "ejs";
    public const string Pug = "pug";

    // Web Backend & Scripting
    public const string Python = "python";
    public const string Ruby = "ruby";
    public const string Php = "php";
    public const string Perl = "perl";
    public const string Lua = "lua";
    public const string Elixir = "elixir";

    // Shell & Automation
    public const string Shell = "shell";
    public const string PowerShell = "powershell";
    public const string Batch = "batch";
    public const string Makefile = "makefile";

    // Functional Languages
    public const string Haskell = "haskell";
    public const string Lisp = "lisp";
    public const string Clojure = "clojure";
    public const string Erlang = "erlang";

    // Data, Schemas & Config
    public const string Json = "json";
    public const string Xml = "xml";
    public const string Yaml = "yaml";
    public const string Toml = "toml";
    public const string Ini = "ini";
    public const string Properties = "properties";
    public const string Config = "config";
    public const string Sql = "sql";
    public const string GraphQL = "graphql";
    public const string Protobuf = "protobuf";
    public const string Avro = "avro";
    public const string Env = "env";

    // Infrastructure & Build
    public const string Docker = "docker";
    public const string Terraform = "terraform";
    public const string Cmake = "cmake";
    public const string Nginx = "nginx";
    public const string Jenkinsfile = "jenkinsfile";

    // Scientific & Stats
    public const string RLang = "r_lang";
    public const string Julia = "julia";
    public const string Matlab = "matlab";

    // Documentation
    public const string Text = "text";
    public const string Markdown = "markdown";
    public const string Rst = "rst";
    public const string AsciiDoc = "asciidoc";

    // Default type for unclassified files
    public const string Other = "other";
}