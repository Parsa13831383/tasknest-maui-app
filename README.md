# 6004CMD-Parsa-Nanavazadeh
TaskNest – Cloud Task Manager

## Professional Confirmation Email Setup (Supabase)

TaskNest now sends richer signup metadata (`full_name`, `app_name`) and supports an optional confirmation redirect URL from `TaskNest/Services/Supabase/SupabaseConfig.cs`.

To make confirmation emails look professional and improve delivery reliability:

1. In Supabase Dashboard, open `Authentication -> URL Configuration`.
2. Set `Site URL` to your production domain.
3. Add your confirmation redirect URL to `Redirect URLs`, then set the same URL in `EmailConfirmationRedirectUrl` in `SupabaseConfig`.
4. Open `Authentication -> Email Templates -> Confirm signup`.
5. Set a clear subject, for example: `Confirm your TaskNest account`.
6. Paste the HTML from `TaskNest/Resources/Raw/Supabase/confirmation-email-template.html`.
7. Open `Project Settings -> Authentication -> SMTP Settings` and configure a custom SMTP provider (recommended for production).
8. On your sending domain, configure SPF, DKIM, and DMARC records to reduce spam filtering.

Notes:
- If a user tries to register with an already registered but unconfirmed email, TaskNest now attempts to resend the confirmation email automatically.
- Keep support contact details current in the template metadata or fallback text.

### Troubleshooting: Email still looks default/simple

If your email still looks like "Confirm your signup" with a plain link, Supabase is still using the default template.

1. Open `Authentication -> Email Templates -> Confirm signup`.
2. Paste the full HTML from `TaskNest/Resources/Raw/Supabase/confirmation-email-template.html`.
3. Click `Save` in the template editor.
4. Send a brand-new signup from an unused email (not cached by an old email event).
5. Confirm your project URL matches the one in `TaskNest/Services/Supabase/SupabaseConfig.cs`.

Tip: the default Supabase template usually includes "powered by Supabase" and "Opt out" copy. If you still see those exact lines, the custom template has not been applied yet.

Important compatibility note:
- If a template references unsupported variables (especially nested custom fields like `.Data.*` in some setups), Supabase can fall back to its default template.
- The checked-in template now uses only safe core placeholders: `{{ .ConfirmationURL }}` and `{{ .Email }}`.
