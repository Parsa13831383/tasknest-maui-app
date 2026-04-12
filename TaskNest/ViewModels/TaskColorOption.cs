using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Graphics;

namespace TaskNest.ViewModels;

public partial class TaskColorOption : ObservableObject
{
    public string Hex { get; }

    public Color Color { get; }

    [ObservableProperty]
    private bool isSelected;

    public TaskColorOption(string hex)
    {
        Hex = hex;
        Color = Color.FromArgb(hex);
    }
}