# winUItoolkit Examples

A runnable WinUI 3 desktop app that exercises every helper, animation, and feature in the `winUItoolkit` library. Use it as a smoke test, a playground, and a copy-paste reference for your own apps.

## Prerequisites

- Windows 10 (19041) or later.
- .NET 8 SDK.
- Windows App SDK 1.8+ runtime (the project is `WindowsAppSDKSelfContained`, so it can run without a system-wide install).

## Running

From the repo root:

```
dotnet run --project examples/WinUIToolkit.Examples
```

The window opens with a `NavigationView` on the left; click any item to see that helper in action.

## Layout

```
examples/WinUIToolkit.Examples/
  Program.cs                   # WinUI 3 unpackaged entry point
  App.xaml / App.xaml.cs       # Application + logging init
  MainWindow.xaml              # NavigationView shell
  Pages/                       # One page per helper category
  Sample/AppState.cs           # Concrete SingletonBase<T> for the demo
  app.manifest                 # PerMonitorV2 DPI + supportedOS list
```

## What each page covers

| Page | Helpers demonstrated |
| --- | --- |
| Home | (landing) |
| Custom Animations | `CustomAnimations` (Show/Hide/Slide/Scale/Shake/Flip) |
| Colors | `ColorConverter` (hex ⇄ Color, HSL, lighten/darken/desaturate) |
| Validators | `Validators` (email, phone, URL, password, postal) |
| Dialogs & Toast | `MessageBoxHelper`, `ToastHelper` |
| Clipboard | `ClipboardHelper` |
| Visual Tree | `VisualElementsHelper` (find descendants, dump) |
| Performance / Logging | `PerformanceHelper`, `LoggingHelper`, `AsyncUtils.Debounce` |
| JSON Storage | `JsonStorage.SerializeAsync` / `DeserializeAsync` |
| Storage & LocalSettings | `StorageHelper.SetIntoLocalSettingsAsync` / `GetFromLocalSettingsAsync` |
| Image Manipulation | `ImageManipulationHelper` (load, resize, crop, rotate, brightness, grayscale, invert) + `PicturePickerHelper.PickPictureAsync` |
| Image Cache | `ImageCacheHelper.GetCachedImageAsync` / `ClearCacheAsync` |
| HTTP Requests | `ServicesHelper.HttpRequestAsync` (typed + raw) |
| Retry Policy | `RetryPolicyHelper.ExecuteOrThrowAsync` |
| Network | `NetHelper.CheckNetworkConnection` |
| Launcher | `LauncherHelper.LaunchUriAsync` / `LaunchLocalPathAsync` |
| Location | `LocationHelper.GetPositionAsync` + `Distance` + unit conversion |
| Picture Picker | `PicturePickerHelper.PickPictureAsync` / `SavePictureAsync` |
| Calendar | `CalendarHelper.CreateAppointmentAsync` |
| Singleton | `SingletonBase<T>` (backed by `AppState`) |

## Notes

- The example project is configured as **unpackaged** (`WindowsPackageType=None`) and self-contained, so it runs without an MSIX manifest. Switch to packaged in `WinUIToolkit.Examples.csproj` if you want to test MSIX-specific behavior.
- Some helpers (location, picture picker, calendar) require a Windows permission grant on first use. The system prompt is shown automatically.
- Network pages (HTTP, Image Cache, Network) need an active internet connection.
- The example project targets `net8.0-windows10.0.19041.0`, matching the library.

## Build requirements

Building WinUI 3 desktop projects requires:

- .NET 8 SDK.
- Windows App SDK 1.8+ runtime.
- .NET Framework 4.7.2 (the `XamlCompiler.exe` tool runs on it). On Windows 11 this is provided by the in-box .NET Framework 4.8 runtime; on Windows 10 install the [.NET Framework 4.7.2 Developer Pack](https://dotnet.microsoft.com/download/dotnet-framework/net472).
- The Windows SDK reference metadata (`Windows.winmd`). On a stock developer machine with Visual Studio 2022 + the "Windows application development" workload installed, this is present. The metadata is not installed in the CI image used by some sandboxes.

The example project is currently configured for **x64 only** (`Platforms=x64`, `RuntimeIdentifier=win-x64`, `WindowsAppSDKSelfContained=true`). To target other architectures, add them to `WinUIToolkit.Examples.csproj`.
