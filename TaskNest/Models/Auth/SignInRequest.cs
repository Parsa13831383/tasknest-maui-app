using System.Text.Json.Serialization;

namespace TaskNest.Models.Auth;

public class SignInRequest
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;
}