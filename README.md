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

## Supabase Access Control Checklist (RLS)

TaskNest sends the user bearer token on all cloud requests and writes `user_id` when creating tasks/categories. Data isolation must be enforced by RLS policies in Supabase.

Run and verify the following in Supabase SQL Editor:

```sql
alter table public.tasks enable row level security;
alter table public.categories enable row level security;

drop policy if exists "tasks_select_own" on public.tasks;
create policy "tasks_select_own"
on public.tasks for select
using (auth.uid() = user_id);

drop policy if exists "tasks_insert_own" on public.tasks;
create policy "tasks_insert_own"
on public.tasks for insert
with check (auth.uid() = user_id);

drop policy if exists "tasks_update_own" on public.tasks;
create policy "tasks_update_own"
on public.tasks for update
using (auth.uid() = user_id)
with check (auth.uid() = user_id);

drop policy if exists "tasks_delete_own" on public.tasks;
create policy "tasks_delete_own"
on public.tasks for delete
using (auth.uid() = user_id);

drop policy if exists "categories_select_own" on public.categories;
create policy "categories_select_own"
on public.categories for select
using (auth.uid() = user_id);

drop policy if exists "categories_insert_own" on public.categories;
create policy "categories_insert_own"
on public.categories for insert
with check (auth.uid() = user_id);

drop policy if exists "categories_update_own" on public.categories;
create policy "categories_update_own"
on public.categories for update
using (auth.uid() = user_id)
with check (auth.uid() = user_id);

drop policy if exists "categories_delete_own" on public.categories;
create policy "categories_delete_own"
on public.categories for delete
using (auth.uid() = user_id);
```

Quick verification:
1. Log in as user A and create tasks/categories.
2. Log in as user B and confirm user A data is not visible.
3. Try updating a user A row as user B; it should be blocked by RLS.
