using Microsoft.Maui.Graphics;

namespace TaskNest.Controls;

public partial class TaskCard : ContentView
{
    public TaskCard()
    {
        InitializeComponent();
    }

    // Title
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(TaskCard), "");

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    // Subtitle
    public static readonly BindableProperty SubtitleProperty =
        BindableProperty.Create(nameof(Subtitle), typeof(string), typeof(TaskCard), "");

    public string Subtitle
    {
        get => (string)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    // Priority text + color
    public static readonly BindableProperty PriorityTextProperty =
        BindableProperty.Create(nameof(PriorityText), typeof(string), typeof(TaskCard), "Low");

    public string PriorityText
    {
        get => (string)GetValue(PriorityTextProperty);
        set => SetValue(PriorityTextProperty, value);
    }

    public static readonly BindableProperty PriorityColorProperty =
        BindableProperty.Create(nameof(PriorityColor), typeof(Color), typeof(TaskCard), Colors.Gray);

    public Color PriorityColor
    {
        get => (Color)GetValue(PriorityColorProperty);
        set => SetValue(PriorityColorProperty, value);
    }

    // Optional icon
    public static readonly BindableProperty IconProperty =
        BindableProperty.Create(nameof(Icon), typeof(ImageSource), typeof(TaskCard), default(ImageSource));

    public ImageSource? Icon
    {
        get => (ImageSource?)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly BindableProperty HasIconProperty =
        BindableProperty.Create(nameof(HasIcon), typeof(bool), typeof(TaskCard), false);

    public bool HasIcon
    {
        get => (bool)GetValue(HasIconProperty);
        set => SetValue(HasIconProperty, value);
    }
}