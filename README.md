# FileScanner

Simple tool for converting projects into text files, convenient for AI analysis or manual review.

![image](https://github.com/user-attachments/assets/ec8a7b50-e56d-4f0a-837f-88cca6c62b58)

## Why you need this

When working with large projects and wanting to show code to ChatGPT or other AI models, you have to copy files one by one. 
FileScanner does this automatically - scans the entire project and creates structured text files.
Which can then be used for databases.

## What it does

- Scans project recursively, ignoring junk like `bin`, `obj`, `.git`
- Separates files by types: C#, XAML, configs, etc. separately
- Shows project tree right in the interface
- Overwrites old files, doesn't create copies
- Saves to desktop by default

## Installation

Requires .NET 8. Download release or build yourself:

```bash
git clone https://github.com/diqezit/FileScanner.git
dotnet run
```

## How to use

1. Choose project folder
2. Specify where to save (or leave default)
3. Hit "Start"
4. Get files like `Project_Root(cs).txt`, `Controllers(xaml).txt`, etc.

Each file contains code with structure:
```
// Controllers/HomeController.cs
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    // your code
}

// Controllers/ApiController.cs
// next file...
```

## Technical details

Written in C# 12 + .NET 8 with WinForms. Uses:
- Dependency Injection for architecture
- Async/await for UI responsiveness
- SOLID principles, because I love clean code

Simple structure:
- `Core/` - scanning logic
- `UI/` - interface
- `Configuration/` - settings

## Configuration

Want to add new file types? Edit `FileTypeClassifier.cs`:

```csharp
[".ts"] = "typescript",
[".vue"] = "vue"
```

Ignore other folders? Change `ScannerConfiguration.cs`.

## Plans

- CLI version for automation
- Dark theme (yes, I know it's missing)
- Support for more languages
- Maybe web version if there's interest

## Bugs and suggestions

Write in Issues. Pull requests welcome, just stick to the code style.

## License

MIT - do whatever you want.

---

Made because I got tired of copying code file by file 🙃

**FileScanner** - инструмент для автоматического преобразования проектов в структурированные текстовые файлы. Сканирует код, разделяет по типам файлов (C#, XAML, конфиги) и создает удобные для анализа AI форматы. Избавляет от ручного копирования кода при работе с ChatGPT и другими LLM.
