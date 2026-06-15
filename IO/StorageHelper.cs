using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;
using winUItoolkit.Helpers;

namespace winUItoolkit.IO
{
    public static class StorageHelper
    {
        /// <summary>
        /// Reads a JSON file from a package URI (e.g. "ms-appx:///Assets/config.json") and deserializes it.
        /// </summary>
        public static async Task<T?> GetContentFileDeserializedAsync<T>(string uri)
        {
            try
            {
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(uri)).AsTask().ConfigureAwait(false);
                using var stream = await file.OpenStreamForReadAsync().ConfigureAwait(false);
                return await JsonSerializer.DeserializeAsync<T>(stream, JsonStorage.FileOptions).ConfigureAwait(false);
            }
            catch (Exception primaryEx)
            {
                // Fallback for unpackaged desktop scenarios where ms-appx:/// URIs may not be available.
                // Try to resolve the path relative to the executable directory.
                const string msAppxPrefix = "ms-appx:///";
                string relativePath = uri.StartsWith(msAppxPrefix, StringComparison.OrdinalIgnoreCase)
                    ? uri.Substring(msAppxPrefix.Length)
                    : uri;

                string candidate = Path.Combine(AppContext.BaseDirectory, relativePath.Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(candidate))
                {
                    try
                    {
                        await using var fs = File.OpenRead(candidate);
                        return await JsonSerializer.DeserializeAsync<T>(fs, JsonStorage.FileOptions).ConfigureAwait(false);
                    }
                    catch (Exception fallbackEx)
                    {
                        Debug.WriteLine($"[StorageHelper] Fallback read failed for '{uri}': {fallbackEx}");
                        return default;
                    }
                }

                Debug.WriteLine($"[StorageHelper] Error reading file from URI '{uri}': {primaryEx}");
                return default;
            }
        }

        /// <summary>
        /// Reads and deserializes a JSON file from the given StorageFolder.
        /// </summary>
        public static async Task<T?> GetContentFileDeserializedAsync<T>(StorageFolder folder, string fileName)
        {
            try
            {
                StorageFile file = await folder.GetFileAsync(fileName);
                using var stream = await file.OpenStreamForReadAsync();
                return await JsonSerializer.DeserializeAsync<T>(stream, JsonStorage.FileOptions);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[StorageHelper] Error reading file '{fileName}': {ex}");
                return default;
            }
        }

        /// <summary>
        /// Serializes an object as JSON and writes it into the specified file.
        /// </summary>
        public static async Task<bool> SetContentFileSerializedAsync<T>(StorageFolder folder, string fileName, T content)
        {
            try
            {
                StorageFile file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                using var stream = await file.OpenStreamForWriteAsync();
                await JsonSerializer.SerializeAsync(stream, content, JsonStorage.FileOptions);
                await stream.FlushAsync();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[StorageHelper] Error writing file '{fileName}': {ex}");
                return false;
            }
        }

        /// <summary>
        /// Retrieves a simple value from local settings. Uses <see cref="ApplicationData.LocalSettings"/>
        /// in packaged (MSIX) apps and a JSON file under <c>%LocalAppData%\winUItoolkit\settings.json</c>
        /// in unpackaged scenarios.
        /// </summary>
        public static async Task<T?> GetFromLocalSettingsAsync<T>(string key)
        {
            if (RuntimeHelper.IsPackaged())
            {
                try
                {
                    var settings = ApplicationData.Current.LocalSettings;
                    if (settings.Values.TryGetValue(key, out object? value) && value is T typed)
                        return typed;
                    return default;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[StorageHelper] Error reading LocalSettings key '{key}': {ex}");
                    return default;
                }
            }

            var map = await ReadUnpackagedSettingsAsync().ConfigureAwait(false);
            if (map.TryGetValue(key, out var raw))
            {
                try
                {
                    return JsonSerializer.Deserialize<T>(raw, JsonStorage.FileOptions);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[StorageHelper] Error deserializing unpackaged setting '{key}': {ex}");
                    return default;
                }
            }
            return default;
        }

        /// <summary>
        /// Saves a simple value into local settings. Uses <see cref="ApplicationData.LocalSettings"/>
        /// in packaged (MSIX) apps and a JSON file under <c>%LocalAppData%\winUItoolkit\settings.json</c>
        /// in unpackaged scenarios.
        /// </summary>
        public static async Task SetIntoLocalSettingsAsync<T>(string key, T value)
        {
            if (RuntimeHelper.IsPackaged())
            {
                try
                {
                    var settings = ApplicationData.Current.LocalSettings;
                    settings.Values[key] = value;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[StorageHelper] Error writing LocalSettings key '{key}': {ex}");
                }
                return;
            }

            var map = await ReadUnpackagedSettingsAsync().ConfigureAwait(false);
            map[key] = JsonSerializer.Serialize(value, JsonStorage.FileOptions);
            await WriteUnpackagedSettingsAsync(map).ConfigureAwait(false);
        }

        private static string UnpackagedSettingsPath
            => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "winUItoolkit", "settings.json");

        private static async Task<Dictionary<string, string>> ReadUnpackagedSettingsAsync()
        {
            try
            {
                if (!File.Exists(UnpackagedSettingsPath)) return new();
                await using var fs = File.OpenRead(UnpackagedSettingsPath);
                return await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(fs, JsonStorage.FileOptions).ConfigureAwait(false)
                    ?? new();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[StorageHelper] Error reading unpackaged settings file: {ex}");
                return new();
            }
        }

        private static async Task WriteUnpackagedSettingsAsync(Dictionary<string, string> map)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(UnpackagedSettingsPath)!);
                await using var fs = File.Create(UnpackagedSettingsPath);
                await JsonSerializer.SerializeAsync(fs, map, JsonStorage.FileOptions).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[StorageHelper] Error writing unpackaged settings file: {ex}");
            }
        }
    }
}
