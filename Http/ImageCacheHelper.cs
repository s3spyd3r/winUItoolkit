using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.IO;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using winUItoolkit.Helpers;

namespace winUItoolkit.Http
{
    public static class ImageCacheHelper
    {
        private static readonly HttpClient _httpClient = new() { Timeout = TimeSpan.FromSeconds(10) };
        private static readonly string _cacheFolderName = "ImageCache";
        // Default max cache size: 100 MB
        private const long DefaultMaxCacheSizeBytes = 100 * 1024 * 1024;

        /// <summary>
        /// Loads an image from cache or downloads it if not found.
        /// Uses StorageFolder/StorageFile APIs end-to-end and enforces a simple size-based eviction policy.
        /// </summary>
        /// <param name="url">Remote image URL.</param>
        /// <param name="cacheDuration">How long to keep the image cached (default 7 days).</param>
        /// <param name="maxCacheSizeBytes">Maximum cache size in bytes (defaults to 100MB).</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        public static async Task<BitmapImage?> GetCachedImageAsync(string url, TimeSpan? cacheDuration = null, long? maxCacheSizeBytes = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(url)) return null;

            try
            {
                StorageFolder cacheFolder = await GetCacheFolderAsync();

                string fileName = GetSafeFileName(url);
                string filePath = Path.Combine(cacheFolder.Path, fileName);

                var expiration = cacheDuration ?? TimeSpan.FromDays(7);

                bool shouldDownload = true;
                if (File.Exists(filePath))
                {
                    var created = File.GetCreationTimeUtc(filePath);
                    if (DateTimeOffset.UtcNow - created < expiration)
                    {
                        shouldDownload = false;
                    }
                    else
                    {
                        try { File.Delete(filePath); } catch { }
                    }
                }

                if (shouldDownload)
                {
                    var bytes = await _httpClient.GetByteArrayAsync(url, cancellationToken);

                    long maxBytes = maxCacheSizeBytes ?? DefaultMaxCacheSizeBytes;
                    await EnsureCacheSizeLimitAsync(cacheFolder, maxBytes, bytes.Length);

                    await File.WriteAllBytesAsync(filePath, bytes, cancellationToken);
                }

                var file = await cacheFolder.GetFileAsync(fileName).AsTask();
                return await LoadBitmapImageAsync(file);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ImageCacheHelper error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Clears the entire cache folder.
        /// </summary>
        public static async Task ClearCacheAsync()
        {
            try
            {
                var cacheFolder = await GetCacheFolderAsync();
                foreach (var file in await cacheFolder.GetFilesAsync())
                {
                    try { await file.DeleteAsync(StorageDeleteOption.PermanentDelete); } catch { }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to clear image cache: {ex.Message}");
            }
        }

        private static async Task<StorageFolder> GetCacheFolderAsync()
        {
            if (RuntimeHelper.IsPackaged())
            {
                return await ApplicationData.Current.TemporaryFolder.CreateFolderAsync(
                    _cacheFolderName, CreationCollisionOption.OpenIfExists).AsTask();
            }

            string path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "winUItoolkit", _cacheFolderName);
            Directory.CreateDirectory(path);
            return await StorageFolder.GetFolderFromPathAsync(path).AsTask();
        }

        private static string GetSafeFileName(string url)
        {
            // Hash the URL to avoid filesystem issues with long URLs or invalid characters.
            // SHA256 is used for collision resistance (SHA1 is deprecated); this is not a security-sensitive use.
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(url));
            var hash = Convert.ToHexString(hashBytes);

            // Try to preserve the original extension for content-type sniffing by decoders.
            string ext = string.Empty;
            if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
                ext = Path.GetExtension(uri.LocalPath);

            if (string.IsNullOrEmpty(ext)) ext = ".img";
            return $"{hash}{ext}";
        }

        private static async Task<BitmapImage> LoadBitmapImageAsync(StorageFile file)
        {
            var bitmap = new BitmapImage();
            using var stream = await file.OpenReadAsync();
            await bitmap.SetSourceAsync(stream);
            return bitmap;
        }

        private static async Task EnsureCacheSizeLimitAsync(StorageFolder folder, long maxBytes, int incomingFileSize)
        {
            try
            {
                var files = await folder.GetFilesAsync().AsTask();
                long total = 0;
                foreach (var f in files)
                {
                    var p = await f.GetBasicPropertiesAsync().AsTask();
                    total += (long)p.Size;
                }

                if (total + incomingFileSize <= maxBytes)
                    return;

                var ordered = new List<StorageFile>(files.Count);
                foreach (var f in files) ordered.Add(f);
                ordered.Sort((a, b) => a.DateCreated.CompareTo(b.DateCreated));

                foreach (var f in ordered)
                {
                    try
                    {
                        var p = await f.GetBasicPropertiesAsync().AsTask();
                        long size = (long)p.Size;
                        await f.DeleteAsync().AsTask();
                        total -= size;

                        if (total + incomingFileSize <= maxBytes)
                            break;
                    }
                    catch
                    {
                        // ignore individual delete failures
                    }
                }
            }
            catch
            {
                // ignore eviction errors - cache is best-effort
            }
        }
    }
}
