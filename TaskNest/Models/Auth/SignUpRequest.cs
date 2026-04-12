using System.Text.Json.Serialization;

namespace TaskNest.Models.Auth;

public class SignUpRequest
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public SignUpMetadata? Data { get; set; }

    [JsonPropertyName("email_redirect_to")]
    public string? EmailRedirectTo { get; set; }
}

public class SignUpMetadata
{
    [JsonPropertyName("full_name")]
    public string? FullName { get; set; }

    [JsonPropertyName("app_name")]
    public string AppName { get; set; } = string.Empty;

    [JsonPropertyName("support_email")]
    public string SupportEmail { get; set; } = "support@tasknest.app";
}
