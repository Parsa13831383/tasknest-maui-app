# Security Policy

## 1. Overview

TaskNest enforces defence-in-depth security across authentication, data access, input validation, session management, and secure storage. The strategy aligns with OWASP Mobile Top 10 recommendations and Supabase best practices.

## 2. Architecture

```
┌──────────────┐    HTTPS / TLS 1.2+    ┌─────────────────┐
│  .NET MAUI   │ ◄─────────────────────► │  Supabase Edge  │
│  Client App   │                         │  (GoTrue Auth   │
│               │                         │   + PostgREST)  │
└──────┬───────┘                         └────────┬────────┘
       │                                          │
  SecureStorage                            Row Level Security
  (platform keychain)                      (PostgreSQL RLS)
```

All network traffic uses HTTPS. No plaintext HTTP endpoints are used.

## 3. Authentication

| Concern | Implementation |
|---|---|
| Provider | Supabase GoTrue (email + password) |
| Token type | JWT (access + refresh) |
| Token storage | .NET MAUI `SecureStorage` — iOS Keychain / Android Keystore / Windows DPAPI |
| Fallback | `Preferences` wrapper only when SecureStorage unavailable (emulator) |
| Password rules | Minimum 6 characters enforced client-side; Supabase enforces server-side policy |
| Session lifecycle | Tokens loaded on app start; cleared on explicit sign-out via `SecureSessionService` |

### Session Flow
1. User signs in → Supabase returns JWT access token + refresh token.
2. Tokens stored in `SecureStorage` via `ISecureSessionService`.
3. On each API call, the access token is attached as `Authorization: Bearer`.
4. On sign-out, all stored tokens and preferences are cleared.

## 4. Threat Model

| Threat | Risk | Mitigation |
|---|---|---|
| Unauthorized data access | High | Row Level Security (RLS) on `tasks` and `categories` tables — queries filtered by `user_id = auth.uid()` |
| Token theft from device | Medium | SecureStorage uses platform keychain/keystore with hardware-backed encryption where available |
| Malicious input (XSS/injection) | Medium | `IInputValidationService` validates and sanitizes all user inputs before API calls |
| Credential stuffing | Medium | Delegated to Supabase server-side rate limiting |
| Data loss | Low | Soft-delete pattern (`is_deleted` flag) prevents permanent data loss; supports undo workflows |
| Man-in-the-middle | Low | All traffic over TLS; Supabase enforces HTTPS-only |

## 5. Data Protection

- **No passwords stored locally** — authentication is delegated to Supabase GoTrue.
- **JWT tokens** are stored in platform `SecureStorage` (iOS Keychain, Android EncryptedSharedPreferences).
- **No sensitive data in logs** — error handlers display user-friendly messages without exposing stack traces.
- **Soft delete** — cloud and local deletes set `is_deleted = true` rather than removing rows, reducing accidental data loss.

## 6. Input Validation

All user input is validated through `IInputValidationService` before reaching the network layer:

| Input | Validation |
|---|---|
| Email | Format check via regex; trimmed and lowercased |
| Password | Minimum length (6 chars); non-empty check |
| Task title | Required, max 200 characters, trimmed, HTML-escaped |
| Task description/reflection | Optional, max 2000 characters, trimmed |
| Category name | Required, max 100 characters, uniqueness check, trimmed |
| Category description | Optional, max 500 characters, trimmed |

## 7. Cloud Access Control (Row Level Security)

Supabase PostgreSQL RLS is enabled on all user-data tables:

```sql
-- Example policy on tasks table
CREATE POLICY "Users can only access their own tasks"
  ON tasks FOR ALL
  USING (user_id = auth.uid());
```

- **SELECT, INSERT, UPDATE, DELETE** all filtered by `user_id = auth.uid()`.
- Even if a client constructs a malicious query, the database rejects access to other users' rows.
- Categories follow the same RLS policy pattern.

## 8. Secure Coding Practices

- **No hardcoded secrets** — Supabase URL and anon key are configuration values, not embedded secrets. The anon key is a public key scoped by RLS.
- **Parameterised queries** — all Supabase PostgREST calls use typed DTOs, preventing SQL injection.
- **Async/await** — all I/O operations are non-blocking, preventing UI-thread deadlocks.
- **IsBusy guards** — all ViewModel commands check `IsBusy` before executing, preventing duplicate requests.
- **WeakReferenceMessenger** — cross-ViewModel messaging uses weak references to prevent memory leaks.

## 9. OWASP Mobile Top 10 Alignment

| OWASP M# | Category | Status |
|---|---|---|
| M1 | Improper Platform Usage | SecureStorage used correctly; no misuse of platform APIs |
| M2 | Insecure Data Storage | Tokens in keychain/keystore; no plaintext secrets |
| M3 | Insecure Communication | All traffic over HTTPS/TLS |
| M4 | Insecure Authentication | JWT-based auth; session cleanup on sign-out |
| M5 | Insufficient Cryptography | Delegated to platform keychain (hardware-backed) |
| M6 | Insecure Authorization | RLS enforces server-side authorization |
| M7 | Client Code Quality | Input validation; null checks; IsBusy guards |
| M8 | Code Tampering | N/A for coursework scope |
| M9 | Reverse Engineering | N/A for coursework scope |
| M10 | Extraneous Functionality | No debug endpoints or test backdoors in release |

## 10. Current Limitations

| Limitation | Planned Mitigation |
|---|---|
| No refresh token rotation | Implement token refresh before expiry |
| No biometric unlock | Add `BiometricAuthentication` plugin |
| No rate limiting on client | Supabase handles server-side; add client-side throttling |
| No audit logging | Add cloud function for change tracking |
| No multi-factor authentication | Enable Supabase MFA when available |
| No certificate pinning | Add SSL pinning for production release |