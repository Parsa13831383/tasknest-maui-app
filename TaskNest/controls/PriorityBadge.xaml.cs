using Microsoft.Maui.Graphics;

namespace TaskNest.Controls;

public partial class PriorityBadge : ContentView
{
    public PriorityBadge()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(PriorityBadge), "Low");

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly BindableProperty BadgeColorProperty =
        BindableProperty.Create(nameof(BadgeColor), typeof(Color), typeof(PriorityBadge), Colors.Gray);

    public Color BadgeColor
    {
        get => (Color)GetValue(BadgeColorProperty);
        set => SetValue(BadgeColorProperty, value);
    }
}