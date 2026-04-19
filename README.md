# TaskNest - .NET MAUI Task Management App

TaskNest is a cross-platform mobile task management application built with .NET MAUI using the MVVM architectural pattern. It supports local task management, cloud-backed authentication, and structured app design with clear separation of concerns.

---

## Features

### Core Functionality
- User registration and login
- Task creation, editing, deletion, and completion tracking
- Category management
- Dashboard overview
- Task detail and task edit flows
- Settings and profile pages

### Architecture
- Built using the MVVM pattern
- Clear separation of:
  - Models
  - ViewModels
  - Services
  - Interfaces
  - Views
- Reusable controls and converters
- Structured navigation through Shell

### Data & Cloud Integration
- Local data storage with SQLite
- Cloud authentication and database integration with Supabase
- User-level ownership and secure data handling
- Sync-ready task and category structure

### UI
- Multi-page mobile interface
- Consistent visual styling
- Reusable TaskCard component
- Dashboard and task summaries
- Empty states and structured layout design

---

## Tech Stack

**Frontend / App**
- .NET MAUI
- C#
- XAML

**Architecture**
- MVVM
- Services & Interfaces pattern
- Repository / structured data access approach

**Data**
- SQLite
- Supabase

---

## Project Structure

```text
tasknest-maui-app/
│
├── TaskNest/
│   ├── controls/
│   ├── converters/
│   ├── Data/
│   ├── Interfaces/
│   ├── Models/
│   ├── Platforms/
│   ├── Properties/
│   ├── Repositories/
│   ├── Resources/
│   ├── Services/
│   ├── ViewModels/
│   ├── Views/
│   ├── App.xaml
│   ├── AppShell.xaml
│   └── MauiProgram.cs
│
├── TaskNest.sln
└── README.md
