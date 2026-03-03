using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TaskNest.Models;

public partial class TaskItem : ObservableObject
{
    public TaskItem()
    {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; init; }

    [ObservableProperty]
    private string title = string.Empty;

    [ObservableProperty]
    private bool isCompleted;
}
