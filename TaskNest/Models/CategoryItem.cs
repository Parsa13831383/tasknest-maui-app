using SQLite;
using Microsoft.Maui.Graphics;
using TaskNest.Models.Enums;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TaskNest.Models;

public class CategoryItem : INotifyPropertyChanged
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [MaxLength(100), Unique]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    // Sync-ready fields
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public SyncStatus SyncStatus { get; set; } = SyncStatus.PendingCreate;

    //UI ONLY (ignored in DB)
    [Ignore]
    public string TaskCountText { get; set; } = string.Empty;

    [Ignore]
    public Color BadgeBackgroundColor { get; set; } = Colors.LightGray;

    [Ignore]
    public Color BadgeTextColor { get; set; } = Colors.Black;

    [Ignore]
    public int Count { get; set; }

    [Ignore]
    public int CompletedCount { get; set; }

    [Ignore]
    public double ProgressValue { get; set; }

    [Ignore]
    public string ProgressText { get; set; } = "0/0 completed";

    [Ignore]
    public string IconGlyph { get; set; } = "◉";

    [Ignore]
    public Color AccentColor { get; set; } = Colors.SteelBlue;

    [Ignore]
    public bool IsEmptyState { get; set; }

    [Ignore]
    public bool ShowQuickActions
    {
        get => _showQuickActions;
        set
        {
            if (_showQuickActions != value)
            {
                _showQuickActions = value;
                OnPropertyChanged();
            }
        }
    }

    private bool _showQuickActions;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}