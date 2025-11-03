using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;

namespace winUItoolkit.IO
{
    public static class StorageHelper
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        /// <summary>
        /// Reads a JSON file from a package URI (e.g. "ms-appx:///Assets/config.json") and deserializes it.
        /// </summary>
        public static async Task<T?> GetContentFileDeserializedAsync<T>(string uri)
        {
            try
            {
                try
                {
                    StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(uri)).AsTask().ConfigureAwait(false);
                    using var stream = await file.OpenStreamForReadAsync().ConfigureAwait(false);
                    return await JsonSerializer.DeserializeAsync<T>(stream, JsonOptions).ConfigureAwait(false);
                }
                catch (Exception)
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
                        await using var fs = File.OpenRead(candidate);
                        return await JsonSerializer.DeserializeAsync<T>(fs, JsonOptions).ConfigureAwait(false);
                    }

                    throw; // rethrow original exception if fallback not applicable
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[StorageHelper] Error reading file from URI: {ex}");
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
                return await JsonSerializer.DeserializeAsync<T>(stream, JsonOptions);
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
                await JsonSerializer.SerializeAsync(stream, content, JsonOptions);
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
        /// Retrieves a simple value from local settings.
        /// </summary>
        public static Task<T?> GetFromLocalSettingsAsync<T>(string key)
        {
            try
            {
                var settings = ApplicationData.Current.LocalSettings;
                if (settings.Values.TryGetValue(key, out object? value) && value is T typed)
                    return Task.FromResult<T?>(typed);

                return Task.FromResult<T?>(default);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[StorageHelper] Error reading LocalSettings key '{key}': {ex}");
                return Task.FromResult<T?>(default);
            }
        }

        /// <summary>
        /// Saves a simple value into local settings.
        /// </summary>
        public static void SetIntoLocalSettings<T>(string key, T value)
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
        }
    }
}