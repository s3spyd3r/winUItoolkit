# WinUIToolkit AI Assistant Instructions

## Project Overview
This is a WinUI 3 helper library targeting .NET 8.0+ and Windows App SDK. It provides reusable components, helpers, and animations for building modern Windows applications.

## Key Architecture Patterns

### 1. Component Organization
- **Helpers/**: Core utility classes following single responsibility principle
- **Animations/**: Custom WinUI animations with async/await support
- **IO/**: File system and data persistence utilities
- **Http/**: Networking and web service helpers
- **Tasks/**: System integration helpers (calendar, location, etc.)

### 2. Design Patterns
- Singleton pattern implementation using `SingletonBase<T>` for thread-safe services
- Async/await pattern used throughout for non-blocking operations
- Null-safety enabled with `<Nullable>enable</Nullable>` project-wide

### 3. Code Conventions
- Async methods consistently use the `Async` suffix
- Helper classes are static where appropriate (e.g., `JsonStorage`, `CustomAnimations`)
- XML documentation required for public APIs
- Nullable reference types enabled - use `?` for nullable types

## Common Usage Patterns

### Animations
```csharp
// Duration in seconds, returns Task
await CustomAnimations.ShowUIElementAnimationAsync(element, 0.3);
await CustomAnimations.HideUIElementAnimationAsync(element, 0.4);
```

### JSON Storage
```csharp
// Uses System.Text.Json with consistent settings
await JsonStorage.SerializeAsync(data);
await JsonStorage.DeserializeAsync<T>(json);
```

### Singleton Services
```csharp
public class MyService : SingletonBase<MyService>
{
    // Access via MyService.Instance
}
```

## Project Dependencies
- Windows App SDK 1.8+
- .NET 8.0
- Minimum Windows version: 10.0.17763.0 (1809)
- Target Windows version: 10.0.19041.0 (2004)

## Development Setup
1. Install .NET 8 SDK
2. Install Windows App SDK
3. Build using standard .NET CLI: `dotnet build`

## Key Files for Understanding
- `Helpers/SingletonBase.cs`: Base pattern for services
- `IO/JsonStorage.cs`: Data persistence patterns
- `Animations/CustomAnimations.cs`: UI animation patterns
- `winUItoolkit.csproj`: Project configuration and dependencies