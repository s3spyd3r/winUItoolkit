# winUItoolkit

winUItoolkit is a compact collection of helpers, utilities, converters and UI primitives for WinUI 3 (.NET 8) applications. The goal is to provide small, well-scoped building blocks you can drop into a WinUI 3 app to speed development of common tasks (animations, storage, networking, navigation, permissions, etc.).

This repository targets desktop WinUI 3 apps (Windows App SDK) but includes compatibility fallbacks where practical so helpers work in both packaged (MSIX) and unpackaged scenarios.

Contents
- `Animations/` — custom storyboard-based animations (`CustomAnimations.cs`).
- `Helpers/` — many small helpers (navigation, logging, async utilities, image manipulation, color conversion, validation, clipboard, UI tree, toast, message dialogs, and more).
- `Http/` — networking helpers, an image cache, and a services wrapper (`ServicesHelper`, `ImageCacheHelper`, `NetHelper`).
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
using winUItoolkit.Tasks;
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
ServicesHelper.ServerAddress = "https://api.example.com/v1/";
var result = await ServicesHelper.HttpRequestAsync(RestRequestTypes.Get, "endpoint/items");
var model = await ServicesHelper.HttpRequestAsync<MyModel>(RestRequestTypes.Get, "endpoint/items/1");
```

What this toolkit provides (summary of helpers and features)

Animations
- `Animations/CustomAnimations.cs`: storyboards and helpers for common UI animations (fade, slide, scale, shake, flip, and a helper to await storyboard completion).

IO and Storage
- `IO/JsonStorage.cs`: small wrapper around `System.Text.Json` with shared `Options`/`FileOptions`. Async `Serialize`/`Deserialize` helpers.
- `IO/StorageHelper.cs`: helpers to read package/asset files (with an unpackaged fallback that resolves the URI relative to the executable), plus `LocalSettings` get/set helpers that use `ApplicationData.LocalSettings` when packaged and a JSON file at `%LocalAppData%\winUItoolkit\settings.json` when unpackaged.

HTTP and Networking
- `Http/ServicesHelper.cs`: JSON request/response helpers, file upload/download, retry/backoff, and typed deserialization.
- `Http/ImageCacheHelper.cs`: downloads images, caches them in `ApplicationData.TemporaryFolder` for packaged apps and `%LocalAppData%\winUItoolkit\ImageCache` for unpackaged, and enforces a size-based eviction policy.
- `Http/NetHelper.cs`: quick connectivity check using `NetworkInformation`.

Helpers (Helpers/)
Methods are intentionally small and synchronous/async friendly.

- `AsyncUtils` — `FireAndForget` and `Debounce` helpers.
- `ClipboardHelper` — text get (async). `SetText` is synchronous because `Clipboard.SetContent` must be called on the UI thread.
- `ColorConverter` — hex ⇄ Color, HSL ⇄ Color, lighten/darken/adjust saturation.
- `Converters` — common XAML converters (`BooleanToVisibilityConverter`, `InverseBooleanConverter`).
- `ImageManipulationHelper` — download bytes, build `BitmapImage`, resize, crop, rotate, brightness, grayscale, invert, color replace.
- `LoggingHelper` — simple logger that writes to `Debug` and optionally to a log file under LocalAppData.
- `MessageBoxHelper` — `ContentDialog` wrappers (`ShowAsync` with text/title, multi-button, and `ConfirmAsync`). Serializes concurrent invocations on a single semaphore.
- `NavigationHelper` — `Frame.Navigate` wrapper and `OpenWindow` helper.
- `PerformanceHelper` — `Measure`/`MeasureAsync` timing helpers.
- `RetryPolicyHelper` — generic retry wrapper, plus a public `ComputeBackoff` used by `ServicesHelper`.
- `RuntimeHelper` — detects if the app is running with a package identity (MSIX).
- `SingletonBase<T>` — generic thread-safe singleton base.
- `ToastHelper` — popup-style transient toast attached to a `XamlRoot`.
- `UriHelper` — HTTP URI validation and query parsing.
- `Validators` — common input validators (email, phone, URL, password, numeric, postal, alpha, alphanumeric). `IsValidUrl` delegates to `UriHelper`.
- `VisualElementsHelper` — visual/logical tree traversal and dump helpers.

Tasks folder (platform helpers)
- `Tasks/LauncherHelper.cs` — launch URIs and local files (WinRT Launcher with a `Process.Start` shell fallback for unpackaged scenarios).
- `Tasks/LocationHelper.cs` — geolocation helpers using `Geolocator`, distance calculations, and unit conversions. `PromptEnableLocationAsync` uses `Launcher.LaunchUriAsync` for `ms-settings:privacy-location`.
- `Tasks/PicturePickerHelper.cs` — file picker and camera capture helpers (WinRT pickers, interop for WinUI 3 desktop ownership). Save fallback uses `LocalApplicationData` in unpackaged scenarios.
- `Tasks/CalendarHelper.cs` — appointment creation helpers (uses WinRT `AppointmentManager`).

Design & conventions
- Nullable reference types are enabled in the project.
- Async methods use the `Async` suffix where applicable.
- Helpers prefer returning `Task<T?>` or `bool` for failure instead of throwing on routine errors — callers can inspect returned values and use `LoggingHelper` for diagnostics.

Packaging notes (important)
- Many helpers call WinRT APIs (pickers, StorageFile, Launcher, Geolocator, AppointmentManager). These are fully supported in packaged (MSIX) apps. In unpackaged desktop scenarios (WinUI 3 without package identity) behavior can differ:
	- `ms-appx:///` package URIs may not resolve; `StorageHelper` includes a fallback to read assets from the executable output folder.
	- File pickers require a HWND owner for proper modality in WinUI 3 desktop; `PicturePickerHelper` demonstrates using `WinRT.Interop.WindowNative.GetWindowHandle(window)`.
	- `Process.Start` with `UseShellExecute = true` is used in some helpers where desktop shell behavior is preferable (see `LauncherHelper.LaunchLocalPathAsync`).

Troubleshooting
- If you hit runtime errors related to package identity or capability access, verify:
	1. The app has the required capability in `Package.appxmanifest` (for packaged apps).
	2. For unpackaged development, test the behavior on the target machine; some features (appointments, certain launchers) behave differently without package identity.
