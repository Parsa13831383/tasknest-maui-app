using System.Collections.Generic;

namespace TaskNest.Views;

public partial class TaskEditPage : ContentPage
{
    public TaskEditPage()
    {
        InitializeComponent();

        CategoryPicker.ItemsSource = new List<string>
        {
            "Work",
            "Personal",
            "Study",
            "Health",
            "Finance",
            "Design"
        };

        PriorityPicker.ItemsSource = new List<string>
        {
            "Low",
            "Medium",
            "High",
            "Urgent"
        };

        CategoryPicker.SelectedIndex = 0;
        PriorityPicker.SelectedIndex = 2;
        DueDatePicker.Date = DateTime.Today.AddDays(2);
        CompletedSwitch.IsToggled = false;
    }
}
