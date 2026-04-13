# Security Policy

## Overview
TaskNest applies a basic security policy for authentication, local token handling, input validation, and cloud data access control.

## Token Storage
Authentication session data is stored using .NET MAUI SecureStorage, which uses platform-secure storage mechanisms instead of plain local preferences or hardcoded values.

## Threat Model

Potential risks considered:
- Unauthorized access to user data
- Token exposure on device
- Invalid or malicious input

Mitigations:
- Row Level Security (RLS) ensures user isolation
- SecureStorage prevents plaintext token storage
- Input validation prevents malformed requests

## Data Protection Strategy
- Sensitive tokens are stored using platform secure storage mechanisms where available
- No passwords are stored locally
- All cloud requests require JWT authentication

## Input Validation
The application validates user input before sending it to cloud services:
- email format validation
- minimum password length
- task title length and required checks
- category name length and required checks

## Cloud Access Control
Supabase Row Level Security (RLS) is enabled for the `tasks` and `categories` tables.
Policies restrict users so they can only read and modify rows where `user_id` matches their authenticated account.

## Soft Delete
Cloud and local deletes use a soft delete strategy through the `is_deleted` flag, reducing accidental data loss and supporting sync workflows.

## Current Limitations
This version does not yet include:
- refresh token rotation handling
- advanced password rules
- rate limiting
- full audit logging
- multi-factor authentication