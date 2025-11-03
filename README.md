# winUItoolkit

winUItoolkit is a compact collection of helpers, utilities, converters and UI primitives for WinUI 3 (.NET 8) applications. The goal is to provide small, well-scoped building blocks you can drop into a WinUI 3 app to speed development of common tasks (animations, storage, networking, navigation, permissions, etc.).

This repository targets desktop WinUI 3 apps (Windows App SDK) but includes compatibility fallbacks where practical so helpers work in both packaged (MSIX) and unpackaged scenarios.

Contents
- `Animations/` — custom storyboard-based animations (`CustomAnimations.cs`).
- `Helpers/` — many small helpers (navigation, theme, logging, async utilities, permissions, accessibility, clipboard, and more).
- `Http/` — networking helpers, an image cache, and a services wrapper (`ServicesHelper`, `ImageCacheHelper`).
- `IO/` — JSON serialization and storage helpers (`JsonStorage`, `StorageHelper`).
- `Tasks/` — platform/task helpers (launcher, location, picture picker, calendar helpers).

Prerequisites
- Windows 10 (19041) or later.
- .NET 8 SDK installed.
- Windows App SDK 1.8+ installed if you want to build and run WinUI 3 apps that consume WinUI APIs.

How to use this toolkit in your WinUI 3 app

1. Add a project reference to the `winUItoolkit` project in your WinUI 3 solution, or add the compiled DLL as a reference.
2. Import the namespaces you need, for example:

```csharp
using winUItoolkit.Helpers;
using winUItoolkit.Animations;
using winUItoolkit.Http;
using winUItoolkit.IO;
```

3. Call helpers directly from UI code. Examples:

- Animations:
```csharp
await CustomAnimations.ShowUIElementAnimationAsync(myTextBlock, 0.3);
```

- JSON storage (in-memory helpers):
```csharp
string json = await JsonStorage.SerializeAsync(myObject);
var obj = await JsonStorage.DeserializeAsync<MyType>(json);
```

- Simple HTTP request:
```csharp
var result = await ServicesHelper.HttpRequestAsync(RestRequestTypes.Get, "endpoint/items");
var model = await ServicesHelper.HttpRequestAsync<MyModel>(RestRequestTypes.Get, "endpoint/items/1");
```

What this toolkit provides (summary of helpers and features)

Animations
- `Animations/CustomAnimations.cs`: storyboards and helpers for common UI animations (fade, slide, scale, shake, flip, and a helper to await storyboard completion).

IO and Storage
- `IO/JsonStorage.cs`: small wrapper around `System.Text.Json` for consistent serializer settings.
- `IO/StorageHelper.cs`: helpers to read package/asset files and a fallback for unpackaged apps.

HTTP and Networking
- `Http/ServicesHelper.cs`: lightweight JSON request/response helpers plus file upload/download and a simple retry/backoff.
- `Http/ImageCacheHelper.cs`: downloads images, caches them in the app's temporary folder using `StorageFile` APIs and enforces a size-based eviction policy.
- `Http/NetHelper.cs`: quick connectivity check using `NetworkInformation`.

Helpers (Helpers/)
The following helper types were added to cover common tasks. Methods are intentionally small and synchronous/async friendly.

- `NavigationHelper` — Frame navigation helpers and a small `OpenWindow` helper.
- `ThemeHelper` — Change application theme and apply accent colors at runtime.
- `PermissionsHelper` — Request location permission (wraps `Geolocator.RequestAccessAsync`) and placeholders for camera/microphone.
- `LoggingHelper` — Simple logger that writes to `Debug` and optionally to a log file under LocalAppData.
- `AsyncUtils` — `FireAndForget` and `Debounce` helpers for common async patterns.
- `StoragePathsHelper` — LocalAppData and temporary folder helpers for desktop scenarios.
- `ImageMediaHelper` — convenience wrapper around image resize functionality.
- `UriHelper` — safe HTTP URI validation and query parsing.
- `Converters` — common XAML converters (BooleanToVisibility, InverseBoolean).
- `RetryPolicyHelper` — generic retry wrapper to use across network or IO operations.
- `AccessibilityHelper` — helpers for setting `AutomationProperties` (Name/HelpText).
- `PerformanceHelper` — simple timing helpers for synchronous and asynchronous code.
- `TestHelpers` — small helpers to create/delete temporary test files.
- `RuntimeHelper` — detects if the app is running with a package identity (MSIX).
- `ClipboardHelper` — wrappers for the system clipboard (text); uses WinRT DataTransfer APIs.
- `SystemIntegrationHelper` — open system settings pages or URLs using the shell on desktop.
- `LocalizationHelper` — small wrapper for `ResourceLoader` to load localized strings.

Tasks folder (platform helpers)
- `Tasks/LauncherHelper.cs` — launch URIs and local files (WinRT Launcher in earlier versions; review for packaged/unpackaged behavior).
- `Tasks/LocationHelper.cs` — geolocation helpers using `Geolocator`.
- `Tasks/PicturePickerHelper.cs` — file picker & camera capture helpers (WinRT pickers and interop for WinUI 3 desktop ownership).
- `Tasks/CalendarHelper.cs` — appointment creation helpers (uses WinRT Appointments APIs when available).

Design & conventions
- Nullable reference types are enabled in the project.
- Async methods use the `Async` suffix where applicable.
- Helpers prefer returning `Task<T?>` or `bool` for failure instead of throwing on routine errors — callers can inspect returned values and use `LoggingHelper` for diagnostics.

Packaging notes (important)
- Many helpers call WinRT APIs (pickers, StorageFile, Launcher, Geolocator, AppointmentManager). These are fully supported in packaged (MSIX) apps. In unpackaged desktop scenarios (WinUI 3 without package identity) behavior can differ:
	- `ms-appx:///` package URIs may not resolve; `StorageHelper` includes a fallback to read assets from the executable output folder.
	- File pickers require a HWND owner for proper modality in WinUI 3 desktop; `PicturePickerHelper` demonstrates using `WinRT.Interop.WindowNative.GetWindowHandle(window)`.
	- `Process.Start` with `UseShellExecute = true` is used in some helpers where desktop shell behavior is preferable.

Troubleshooting
- If you hit runtime errors related to package identity or capability access, verify:
	1. The app has the required capability in `Package.appxmanifest` (for packaged apps).
	2. For unpackaged development, test the behavior on the target machine; some features (appointments, certain launchers) behave differently without package identity.
