# TaskNest

A cross-platform task management application built with **.NET MAUI 8** and **Supabase**, developed as coursework for **6004CMD** at Coventry University.

## Background & Motivation

Managing tasks across university, work, and personal life is a constant challenge for students. Existing apps are often bloated with features, require paid subscriptions, or lack proper cloud syncing across devices. TaskNest was built to solve this — a clean, focused mobile app that lets users create, organise, and track tasks with minimal friction.

The project demonstrates how Cloud Computing enhances mobile applications: Supabase provides real-time authentication and data persistence without managing infrastructure, while Row Level Security ensures that each user's data remains private and isolated at the database level. This cloud-first architecture means a user can log in on any platform and see the same tasks immediately — showcasing genuine cross-platform data accessibility.

TaskNest was designed from the ground up with the MVVM pattern, a clean service layer, and proper separation of concerns to reflect professional software engineering practices.

## Features

| Feature | Description |
|---|---|
| Dashboard | Real-time summary of active tasks, completed count, due-today alerts, and focus items |
| Task CRUD | Create, view, edit, complete, and soft-delete tasks with colour coding |
| Categories | Organise tasks into categories with progress tracking and accent colours |
| Authentication | Secure email/password auth via Supabase GoTrue with JWT sessions |
| Profile | View account info, session status, and task statistics |
| Settings | Theme (light/dark), language, reminder frequency, and logout |
| Cloud Sync | All data synced to Supabase with Row Level Security per user |

## Architecture

```
TaskNest/
├── Models/           # Domain entities (TaskItem, CategoryItem, DTOs)
├── ViewModels/       # MVVM ViewModels (CommunityToolkit.Mvvm)
├── Views/            # XAML pages (Dashboard, Tasks, Categories, Profile, Settings)
├── Services/         # Business logic (Auth, Profile, Dashboard, Validation, Navigation)
├── Interfaces/       # Abstractions (IUnitOfWork, ITaskRepository, ICategoryRepository)
├── Repositories/     # Supabase PostgREST implementations
├── Controls/         # Custom XAML controls (TaskCard)
├── Converters/       # Value converters for XAML bindings
├── Data/             # SQLite local database (AppDatabase)
├── Platforms/        # Platform-specific code (Android, iOS, Mac, Windows)
└── Resources/        # Fonts, images, styles, splash screen
```

## Tech Stack

| Layer | Technology |
|---|---|
| UI Framework | .NET MAUI 8.0 |
| MVVM Toolkit | CommunityToolkit.Mvvm 8.4.0 |
| Backend | Supabase (GoTrue Auth + PostgREST) |
| Local Storage | SQLite (sqlite-net-pcl) |
| Secure Storage | .NET MAUI SecureStorage (iOS Keychain / Android Keystore) |
| Target Platforms | iOS, Android, macOS (Catalyst), Windows |

## Design Patterns

- **MVVM** — strict separation of Views, ViewModels, and Models
- **Repository + Unit of Work** — `IUnitOfWork` wraps `ITaskRepository` and `ICategoryRepository`
- **Dependency Injection** — all services and ViewModels registered via `MauiProgram.cs`
- **WeakReferenceMessenger** — cross-ViewModel communication (e.g. `TaskStatusChangedMessage`)
- **Soft Delete** — `is_deleted` flag preserves data integrity and supports undo
- **Input Validation** — `IInputValidationService` sanitises all user inputs before API calls

## Prerequisites

- .NET 8 SDK
- Visual Studio 2022 / VS Code with .NET MAUI extension
- Xcode (for iOS/macOS targets)
- A Supabase project with `tasks` and `categories` tables + RLS policies

## Build & Run

```bash
# macOS (Catalyst)
dotnet build -f net8.0-maccatalyst
dotnet run -f net8.0-maccatalyst

# Android
dotnet build -f net8.0-android

# iOS (requires Mac with Xcode)
dotnet build -f net8.0-ios
```

## Security

See [SECURITY.md](SECURITY.md) for the full security policy, threat model, and OWASP alignment.

## Author

Parsa Nanavazadeh — Coventry University, 6004CMD (Semester 6)
