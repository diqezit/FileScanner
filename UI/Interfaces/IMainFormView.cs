// UI/Interfaces/IMainFormView.cs
namespace FileScanner.UI.Interfaces;

public interface IMainFormView
{
    string ProjectPath { get; set; }
    string OutputPath { get; set; }
    string ProjectTreeText { set; }
    string StatisticsText { set; }

    bool IsSplitEnabled { get; }
    int ChunkSizeInChars { get; }

    event EventHandler LoadForm;
    event EventHandler StartScanClick;
    event EventHandler CancelScanClick;
    event EventHandler BrowseProjectClick;
    event EventHandler BrowseOutputClick;

    void SetScanningState(bool isScanning);
    void ShowInvalidPathInPreview();
    void SetWaitCursor(bool isWaiting);
    void UpdateStatus(string message);
    void UpdateFileCountLabel(string text);
}