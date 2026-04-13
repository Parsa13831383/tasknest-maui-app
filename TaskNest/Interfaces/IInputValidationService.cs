namespace TaskNest.Interfaces;

public interface IInputValidationService
{
    bool TryValidateEmail(string? email, out string normalizedEmail, out string errorMessage);
    bool TryValidatePassword(string? password, out string normalizedPassword, out string errorMessage);
    bool TryValidateRequiredText(string? value, string fieldName, out string normalizedValue, out string errorMessage, int maxLength = 200);
    bool TryValidateTaskTitle(string? title, out string normalizedTitle, out string errorMessage);
    bool TryValidateCategoryName(string? name, out string normalizedName, out string errorMessage);
    string NormalizeOptionalText(string? value, int maxLength = 2000);
}
