using Microsoft.Maui.Graphics;

namespace TaskNest.Controls;

public partial class PriorityBadge : ContentView
{
    public static readonly BindableProperty PriorityTextProperty =
        BindableProperty.Create(
            nameof(PriorityText),
            typeof(string),
            typeof(PriorityBadge),
            default(string));

    public string PriorityText
    {
        get => (string)GetValue(PriorityTextProperty);
        set => SetValue(PriorityTextProperty, value);
    }

    public static readonly BindableProperty PriorityColorProperty =
        BindableProperty.Create(
            nameof(PriorityColor),
            typeof(Color),
            typeof(PriorityBadge),
            Colors.Gray);

    public Color PriorityColor
    {
        get => (Color)GetValue(PriorityColorProperty);
        set => SetValue(PriorityColorProperty, value);
    }

    public PriorityBadge()
    {
        InitializeComponent();
    }
}