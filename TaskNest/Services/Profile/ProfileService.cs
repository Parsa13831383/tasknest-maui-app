using TaskNest.Interfaces;
using TaskNest.Models.Profile;

namespace TaskNest.Services.Profile;

public sealed class ProfileService : IProfileService
{
    private readonly ISupabaseAuthService authService;
    private readonly IUnitOfWork unitOfWork;

    public ProfileService(ISupabaseAuthService authService, IUnitOfWork unitOfWork)
    {
        this.authService = authService;
        this.unitOfWork = unitOfWork;
    }

    public async Task<ProfileSummary> GetProfileSummaryAsync()
    {
        var isAuthenticated = authService.IsAuthenticated;
        if (!isAuthenticated)
        {
            isAuthenticated = await authService.RestoreSessionAsync();
        }

        var authenticatedUser = isAuthenticated
            ? await authService.GetCurrentUserAsync()
            : null;

        var email = NormalizeEmail(authenticatedUser?.Email ?? authService.UserEmail);
        var fullName = ResolveFullName(authenticatedUser?.FullName, email);

        var completedTaskCount = 0;
        var activeTaskCount = 0;
        var categoryCount = 0;

        if (isAuthenticated)
        {
            var tasks = await unitOfWork.Tasks.GetAllAsync();
            var categories = await unitOfWork.Categories.GetAllAsync();

            completedTaskCount = tasks.Count(t => !t.IsDeleted && t.IsCompleted);
            activeTaskCount = tasks.Count(t => !t.IsDeleted && !t.IsCompleted);
            categoryCount = categories.Count(c => !c.IsDeleted);
        }

        return new ProfileSummary
        {
            FullName = fullName,
            Email = email,
            Role = ResolveRole(isAuthenticated, email),
            IsAuthenticated = isAuthenticated,
            SessionStatus = isAuthenticated ? "Active" : "Signed out",
            CompletedTaskCount = completedTaskCount,
            ActiveTaskCount = activeTaskCount,
            CategoryCount = categoryCount,
            Initials = BuildInitials(fullName, email)
        };
    }

    private static string NormalizeEmail(string? email)
    {
        return string.IsNullOrWhiteSpace(email)
            ? "Not available"
            : email.Trim();
    }

    private static string ResolveFullName(string? fullName, string email)
    {
        if (!string.IsNullOrWhiteSpace(fullName) && !IsPlaceholderName(fullName))
        {
            return fullName.Trim();
        }

        if (!string.IsNullOrWhiteSpace(email)
            && !string.Equals(email, "Not available", StringComparison.OrdinalIgnoreCase)
            && email.Contains('@'))
        {
            return email.Split('@')[0];
        }

        return "TaskNest User";
    }

    private static bool IsPlaceholderName(string value)
    {
        var normalized = value.Trim();

        return normalized.Equals("TaskNest User", StringComparison.OrdinalIgnoreCase)
            || normalized.Equals("User", StringComparison.OrdinalIgnoreCase)
            || normalized.Equals("Guest", StringComparison.OrdinalIgnoreCase);
    }

    private static string ResolveRole(bool isAuthenticated, string email)
    {
        if (!isAuthenticated)
        {
            return "Guest";
        }

        if (email.EndsWith(".edu", StringComparison.OrdinalIgnoreCase)
            || email.Contains("student", StringComparison.OrdinalIgnoreCase))
        {
            return "Student Developer";
        }

        return "Authenticated User";
    }

    private static string BuildInitials(string fullName, string email)
    {
        var tokens = fullName
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(token => token.Length > 0)
            .ToArray();

        if (tokens.Length >= 2)
        {
            return string.Concat(char.ToUpperInvariant(tokens[0][0]), char.ToUpperInvariant(tokens[1][0]));
        }

        if (tokens.Length == 1)
        {
            return char.ToUpperInvariant(tokens[0][0]).ToString();
        }

        if (!string.IsNullOrWhiteSpace(email)
            && !string.Equals(email, "Not available", StringComparison.OrdinalIgnoreCase))
        {
            return char.ToUpperInvariant(email[0]).ToString();
        }

        return "TU";
    }
}