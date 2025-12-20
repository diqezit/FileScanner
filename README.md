# FileScanner

.NET 8 Windows utility that scans project directories and bundles source code into a single structured text file — ideal for LLM context, code review, or archiving.

<img width="961" height="760" alt="image" src="https://github.com/user-attachments/assets/a5916def-4f31-4dea-a121-1ee5000d1b01" />


## ✨ Features

- **Recursive Scanning** — collects all source files from project directories
- **Smart Grouping** — classifies files by type (C#, Python, JSON, etc.)
- **Filtering** — skips `bin`, `obj`, `node_modules`, `.git` and binary files
- **Tree View** — generates visual directory structure
- **Statistics** — shows file count, folder count, total size
- **Split Output** — optionally splits large files by character limit
- **VS Filters Support** — respects `.vcxproj.filters` for C++ projects
- **Unified Output** — creates `_United_All_Files.txt` with metadata header

## 🚀 Quick Start

```bash
git clone https://github.com/diqezit/FileScanner.git
cd FileScanner
dotnet build
dotnet run
```

**Requirements:** .NET 8 SDK

## 📁 Project Structure

```
FileScanner/
├── Core/
│   ├── Configuration.cs      # Settings, models, path types
│   ├── FileSystemServices.cs # Read/write, validation, formatting
│   ├── ProjectAnalyzer.cs    # Tree generation, statistics
│   └── ScanEngine.cs         # Scan orchestration
├── UI/
│   ├── MainForm.cs           # WinForms UI
│   ├── MainController.cs     # UI logic
│   └── FormLoggerProvider.cs # UI logging
├── Program.cs                # Entry point + DI
└── GlobalUsings.cs
```

## ⚙️ Configuration

Edit `Configuration.cs` to customize:

| Setting | Description |
|---------|-------------|
| `IgnoredExt` | Extensions to skip (`.exe`, `.dll`, `.png`...) |
| `IgnoredDirs` | Folders to skip (`bin`, `obj`, `node_modules`...) |
| `TypeMap` | Extension → type mapping (`.cs` → `csharp`) |
| `MaxTreeDepth` | Tree generation depth limit |
| `DefaultChunk` | Default split size (chars) |

## 🔧 Tech Stack

- **.NET 8** / Windows Forms
- **Microsoft.Extensions.DependencyInjection**
- **Microsoft.Extensions.Logging**

## 📄 License

MIT
