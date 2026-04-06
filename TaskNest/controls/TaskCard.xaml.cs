namespace TaskNest.Controls;

public partial class TaskCard : ContentView
{
    public TaskCard()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(TaskCard), string.Empty);

    public static readonly BindableProperty SubtitleProperty =
        BindableProperty.Create(nameof(Subtitle), typeof(string), typeof(TaskCard), string.Empty);

    public static readonly BindableProperty PriorityTextProperty =
        BindableProperty.Create(nameof(PriorityText), typeof(string), typeof(TaskCard), string.Empty);

    public static readonly BindableProperty PriorityColorProperty =
        BindableProperty.Create(nameof(PriorityColor), typeof(Color), typeof(TaskCard), Colors.Red);

    public static readonly BindableProperty DueDateProperty =
        BindableProperty.Create(nameof(DueDate), typeof(string), typeof(TaskCard), string.Empty);

    public static readonly BindableProperty CategoryProperty =
        BindableProperty.Create(nameof(Category), typeof(string), typeof(TaskCard), string.Empty);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Subtitle
    {
        get => (string)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public string PriorityText
    {
        get => (string)GetValue(PriorityTextProperty);
        set => SetValue(PriorityTextProperty, value);
    }

    public Color PriorityColor
    {
        get => (Color)GetValue(PriorityColorProperty);
        set => SetValue(PriorityColorProperty, value);
    }

    public string DueDate
    {
        get => (string)GetValue(DueDateProperty);
        set => SetValue(DueDateProperty, value);
    }

    public string Category
    {
        get => (string)GetValue(CategoryProperty);
        set => SetValue(CategoryProperty, value);
    }

    private async void OnViewDetailsClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("taskdetail");
    }

    private async void OnEditTaskClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("taskedit");
    }
}