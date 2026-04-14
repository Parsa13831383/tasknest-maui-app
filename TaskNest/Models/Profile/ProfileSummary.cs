namespace TaskNest.Models.Profile;

public sealed class ProfileSummary
{
    public string FullName { get; init; } = "TaskNest User";
    public string Email { get; init; } = "Not available";
    public string Role { get; init; } = "Guest";
    public bool IsAuthenticated { get; init; }
    public string SessionStatus { get; init; } = "Signed out";
    public int CompletedTaskCount { get; init; }
    public int ActiveTaskCount { get; init; }
    public int CategoryCount { get; init; }
    public string Initials { get; init; } = "TU";
}