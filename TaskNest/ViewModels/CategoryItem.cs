namespace TaskNest.ViewModels;

public class CategoryItem
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Count { get; set; }
    public string TaskCountText { get; set; } = string.Empty;
    public Color BadgeBackgroundColor { get; set; } = Colors.LightGray;
    public Color BadgeTextColor { get; set; } = Colors.Black;
}