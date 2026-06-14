using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.IO;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

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
                var cacheFolder = await ApplicationData.Current.TemporaryFolder.CreateFolderAsync(
                    _cacheFolderName, CreationCollisionOption.OpenIfExists).AsTask().ConfigureAwait(false);

                string fileName = GetSafeFileName(url);

                var existing = await cacheFolder.TryGetItemAsync(fileName).AsTask().ConfigureAwait(false);
                var expiration = cacheDuration ?? TimeSpan.FromDays(7);

                bool shouldDownload = true;
                if (existing is StorageFile existingFile)
                {
                    if (DateTimeOffset.UtcNow - existingFile.DateCreated < expiration)
                    {
                        shouldDownload = false;
                    }
                    else
                    {
                        try { await existingFile.DeleteAsync(StorageDeleteOption.Default).AsTask().ConfigureAwait(false); } catch { }
                    }
                }

                if (shouldDownload)
                {
                    var bytes = await _httpClient.GetByteArrayAsync(url, cancellationToken).ConfigureAwait(false);

                    long maxBytes = maxCacheSizeBytes ?? DefaultMaxCacheSizeBytes;
                    await EnsureCacheSizeLimitAsync(cacheFolder, maxBytes, bytes.Length).ConfigureAwait(false);

                    var file = await cacheFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting).AsTask().ConfigureAwait(false);
                    await using (var stream = await file.OpenStreamForWriteAsync().ConfigureAwait(false))
                    {
                        await stream.WriteAsync(bytes.AsMemory(0, bytes.Length), cancellationToken).ConfigureAwait(false);
                    }
                }

                var fileToLoad = await cacheFolder.GetFileAsync(fileName).AsTask().ConfigureAwait(false);
                return await LoadBitmapImageAsync(fileToLoad).ConfigureAwait(false);
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
                var cacheFolder = await ApplicationData.Current.TemporaryFolder.CreateFolderAsync(
                    _cacheFolderName, CreationCollisionOption.OpenIfExists);
                await cacheFolder.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to clear image cache: {ex.Message}");
            }
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
                var files = await folder.GetFilesAsync().AsTask().ConfigureAwait(false);
                long total = 0;
                foreach (var f in files)
                {
                    var p = await f.GetBasicPropertiesAsync().AsTask().ConfigureAwait(false);
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
                        var p = await f.GetBasicPropertiesAsync().AsTask().ConfigureAwait(false);
                        long size = (long)p.Size;
                        await f.DeleteAsync().AsTask().ConfigureAwait(false);
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
