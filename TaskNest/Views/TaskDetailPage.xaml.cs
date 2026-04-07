using Microsoft.Maui.Controls;
using TaskNest.ViewModels;

namespace TaskNest.Views;

[QueryProperty(nameof(TaskTitle), "title")]
[QueryProperty(nameof(Description), "description")]
[QueryProperty(nameof(DueDate), "dueDate")]
[QueryProperty(nameof(Category), "category")]
[QueryProperty(nameof(PriorityText), "priorityText")]
public partial class TaskDetailPage : ContentPage
{
    private string taskTitle = string.Empty;
    private string description = string.Empty;
    private string dueDate = string.Empty;
    private string category = string.Empty;
    private string priorityText = string.Empty;

    public TaskDetailPage()
    {
        InitializeComponent();
        BindingContext = new TaskDetailViewModel();
        ApplyValues();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ApplyValues();
    }

    public string TaskTitle
    {
        get => taskTitle;
        set
        {
            taskTitle = value ?? string.Empty;
            ApplyValues();
        }
    }

    public string Description
    {
        get => description;
        set
        {
            description = value ?? string.Empty;
            ApplyValues();
        }
    }

    public string DueDate
    {
        get => dueDate;
        set
        {
            dueDate = value ?? string.Empty;
            ApplyValues();
        }
    }

    public string Category
    {
        get => category;
        set
        {
            category = value ?? string.Empty;
            ApplyValues();
        }
    }

    public string PriorityText
    {
        get => priorityText;
        set
        {
            priorityText = value ?? string.Empty;
            ApplyValues();
        }
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        ApplyValues();
    }

    private void ApplyValues()
    {
        if (BindingContext is not TaskDetailViewModel viewModel)
            return;

        viewModel.TaskTitle = taskTitle;
        viewModel.Description = description;
        viewModel.DueDate = dueDate;
        viewModel.Category = category;
        viewModel.PriorityText = priorityText;
    }
}
