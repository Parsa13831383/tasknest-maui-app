namespace TaskNest.Models;

public class CategoryItem
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string TaskCountText { get; set; } = string.Empty;
    public Color BadgeBackgroundColor { get; set; } = Colors.LightGray;
    public Color BadgeTextColor { get; set; } = Colors.Black;
    public int Count { get; set; }
}