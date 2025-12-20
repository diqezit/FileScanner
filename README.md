# FileScanner

FileScanner is a .NET 8 Windows utility that scans a project directory, collects all relevant source code files, and bundles them into a single, structured text file. This is perfect for code analysis, creating project archives, or feeding an entire codebase into Large Language Models (LLMs).

![FileScanner UI](https://github.com/user-attachments/assets/6012bfdb-ac3e-49c5-99ce-8bc09526dd8d)

## 🚀 Features

*   **Comprehensive Scanning**: Recursively scans directories to gather all source files.
*   **Smart Classification**: Automatically identifies and groups files by language or type (C#, Python, JSON, etc.).
*   **Flexible Filtering**: Ignores junk directories (`bin`, `obj`, `node_modules`) and file types (`.exe`, `.dll`) for a clean output.
*   **Structure Visualization**: Generates a directory tree view for a clear overview of the project structure.
*   **Detailed Statistics**: Calculates the total number of files, directories, and their combined size.
*   **Structured Output**: Creates individual `.txt` files for each file group per directory, and one master `_United_All_Files.txt` that combines everything with metadata.
*   **Modern UI**: A simple, responsive interface with a dark theme, real-time progress, and logging.
*   **Modular Architecture**: Built with Dependency Injection for clean, testable, and extendable code.
*   **Settings Persistence**: Remembers your last used project and output paths.

## ⚙️ How It Works

1.  **Select a Directory**: You choose the root directory of the project you want to scan.
2.  **Get a Preview**: The app instantly shows the project's directory tree and key statistics.
3.  **Start the Scan**: You click the "Start" button.
4.  **Filter and Group**: The scanner walks through all subfolders, skipping configured exceptions (like `.git` or `bin`). It groups files by type (e.g., `csharp`, `json`).
5.  **Create Intermediate Files**: For each directory with source code, the scanner creates grouped `.txt` files. For example, `main.cs` and `utils.cs` in a `Services` folder become a single `Services(csharp).txt`.
6.  **Generate Final Output**: A single `_United_All_Files.txt` is created. It starts with a header containing the project tree and stats, followed by the content of every `.txt` file generated in the previous step.

## 🛠️ Tech Stack

*   **Platform**: .NET 8
*   **Framework**: Windows Forms (WinForms) with custom rendering for a modern look.
*   **Architecture**: Dependency Injection (`Microsoft.Extensions.DependencyInjection`).
*   **Logging**: `Microsoft.Extensions.Logging` with a custom provider for UI output.

## 🏗️ Project Architecture

The project has a modular structure, with each responsibility separated into its own folder. This makes the code easy to navigate, maintain, and extend.

*   `Analysis`: Services for project analysis (directory validation, file classification, tree generation, stat calculation).
*   `Configuration`: Manages application configuration and user settings.
*   `FileOperations`: Handles reading, writing, and consolidating files.
*   `PathManagement`: Logic for resolving project paths and generating output file names.
*   `Scanning`: The core scanning logic, process orchestration, and file processing.
*   `UI`: Everything related to the user interface, including forms, UI logic, custom components, and themes.
*   `DependencyInjection`: Wires all the modules together using a DI container.

## ⚙️ Configuration

The main scanner configuration (ignored files, directories, file type mappings) is hardcoded in `ScannerConfiguration.cs`. This simplifies deployment but can easily be moved to an external `appsettings.json` file if needed.

Key settings include:
*   `IgnoredExtensions`: An array of file extensions to ignore.
*   `IgnoredDirectories`: An array of directory names to ignore.
*   `FileTypeMapping`: A dictionary that maps file extensions to their classification.
*   `MaxTreeDepth`: The maximum depth for the generated directory tree.

## 🚀 How to Build and Run

1.  **Prerequisites**:
    *   .NET 8 SDK
    *   Visual Studio 2022 or later with the ".NET Desktop Development" workload.

2.  **Build**:
    *   Clone the repository: `git clone https://github.com/your-username/FileScanner.git`
    *   Open `FileScanner.sln` in Visual Studio.
    *   Build the solution (Build -> Build Solution or `Ctrl+Shift+B`).

3.  **Run**:
    *   Run the project from within Visual Studio (Start button or `F5`).
    *   Alternatively, find the executable in the `bin/Debug/net8.0-windows/` folder.

## 📄 License

This project is licensed under the MIT License. See the `LICENSE` file for details.
