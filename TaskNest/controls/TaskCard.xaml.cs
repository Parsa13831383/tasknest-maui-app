namespace TaskNest.Controls;

public partial class TaskCard : ContentView
{
    public TaskCard()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty TaskIdProperty =
        BindableProperty.Create(nameof(TaskId), typeof(string), typeof(TaskCard), string.Empty);

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(TaskCard), string.Empty);

    public static readonly BindableProperty SubtitleProperty =
        BindableProperty.Create(nameof(Subtitle), typeof(string), typeof(TaskCard), string.Empty);

    public static readonly BindableProperty TaskColorProperty =
        BindableProperty.Create(nameof(TaskColor), typeof(Color), typeof(TaskCard), Colors.Gray);

    public static readonly BindableProperty DueDateProperty =
        BindableProperty.Create(nameof(DueDate), typeof(string), typeof(TaskCard), string.Empty);

    public static readonly BindableProperty CategoryProperty =
        BindableProperty.Create(nameof(Category), typeof(string), typeof(TaskCard), string.Empty);

    public static readonly BindableProperty ViewTaskCommandProperty =
        BindableProperty.Create(nameof(ViewTaskCommand), typeof(System.Windows.Input.ICommand), typeof(TaskCard));

    public static readonly BindableProperty EditTaskCommandProperty =
        BindableProperty.Create(nameof(EditTaskCommand), typeof(System.Windows.Input.ICommand), typeof(TaskCard));

    public static readonly BindableProperty DeleteTaskCommandProperty =
        BindableProperty.Create(nameof(DeleteTaskCommand), typeof(System.Windows.Input.ICommand), typeof(TaskCard));

    public static readonly BindableProperty CompleteTaskCommandProperty =
        BindableProperty.Create(nameof(CompleteTaskCommand), typeof(System.Windows.Input.ICommand), typeof(TaskCard));

    public string TaskId
    {
        get => (string)GetValue(TaskIdProperty);
        set => SetValue(TaskIdProperty, value);
    }

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

    public Color TaskColor
    {
        get => (Color)GetValue(TaskColorProperty);
        set => SetValue(TaskColorProperty, value);
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

    public System.Windows.Input.ICommand? ViewTaskCommand
    {
        get => (System.Windows.Input.ICommand?)GetValue(ViewTaskCommandProperty);
        set => SetValue(ViewTaskCommandProperty, value);
    }

    public System.Windows.Input.ICommand? EditTaskCommand
    {
        get => (System.Windows.Input.ICommand?)GetValue(EditTaskCommandProperty);
        set => SetValue(EditTaskCommandProperty, value);
    }

    public System.Windows.Input.ICommand? DeleteTaskCommand
    {
        get => (System.Windows.Input.ICommand?)GetValue(DeleteTaskCommandProperty);
        set => SetValue(DeleteTaskCommandProperty, value);
    }

    public System.Windows.Input.ICommand? CompleteTaskCommand
    {
        get => (System.Windows.Input.ICommand?)GetValue(CompleteTaskCommandProperty);
        set => SetValue(CompleteTaskCommandProperty, value);
    }

    private async void OnViewDetailsClicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(TaskId))
        {
            await Shell.Current.GoToAsync($"taskdetail?id={TaskId}");
        }
    }

    private async void OnEditTaskClicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(TaskId))
        {
            await Shell.Current.GoToAsync($"taskedit?id={TaskId}");
        }
    }
}