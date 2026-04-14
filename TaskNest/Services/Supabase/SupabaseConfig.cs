namespace TaskNest.Services.Supabase;

public static class SupabaseConfig
{
    public const string SupabaseUrl = "https://dxvfopagvqrevflxlwgi.supabase.co";
    public const string SupabaseAnonKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImR4dmZvcGFndnFyZXZmbHhsd2dpIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NzU5MjMyMTQsImV4cCI6MjA5MTQ5OTIxNH0.ampkfKXiXnWqSiOMdQr4mEleCWlrKXejBiHw1QA-SEw";
    public const string AppName = "TaskNest";

    // Optional: set this to an allowed redirect URL in Supabase Auth URL Configuration.
    // Leave empty to use your Supabase project's default Site URL.
    public const string EmailConfirmationRedirectUrl = "";

    // Optional: set this to an allowed reset redirect URL in Supabase Auth URL Configuration.
    // Leave empty to use your Supabase project's default Site URL.
    public const string PasswordResetRedirectUrl = "tasknest://reset-password";
}
