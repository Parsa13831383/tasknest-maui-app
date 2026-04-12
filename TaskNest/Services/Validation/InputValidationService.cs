using System.Net.Mail;
using TaskNest.Interfaces;

namespace TaskNest.Services.Validation;

public class InputValidationService : IInputValidationService
{
    public bool TryValidateEmail(string? email, out string normalizedEmail, out string errorMessage)
    {
        normalizedEmail = (email ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(normalizedEmail))
        {
            errorMessage = "Email is required.";
            return false;
        }

        try
        {
            var parsed = new MailAddress(normalizedEmail);
            if (!string.Equals(parsed.Address, normalizedEmail, StringComparison.OrdinalIgnoreCase))
            {
                errorMessage = "Please enter a valid email address.";
                return false;
            }
        }
        catch
        {
            errorMessage = "Please enter a valid email address.";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }

    public bool TryValidateRequiredText(string? value, string fieldName, out string normalizedValue, out string errorMessage, int maxLength = 200)
    {
        normalizedValue = (value ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(normalizedValue))
        {
            errorMessage = $"{fieldName} is required.";
            return false;
        }

        if (normalizedValue.Length > maxLength)
        {
            errorMessage = $"{fieldName} must be {maxLength} characters or fewer.";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }

    public bool TryValidateTaskTitle(string? title, out string normalizedTitle, out string errorMessage)
    {
        return TryValidateRequiredText(title, "Task title", out normalizedTitle, out errorMessage, maxLength: 120);
    }

    public bool TryValidateCategoryName(string? name, out string normalizedName, out string errorMessage)
    {
        return TryValidateRequiredText(name, "Category name", out normalizedName, out errorMessage, maxLength: 80);
    }

    public string NormalizeOptionalText(string? value, int maxLength = 2000)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var normalized = value.Trim();
        return normalized.Length <= maxLength
            ? normalized
            : normalized[..maxLength];
    }
}
