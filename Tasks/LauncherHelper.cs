using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.System;
using Windows.Storage;

namespace winUItoolkit.Tasks
{
    public static class LauncherHelper
    {
        public static async Task<bool> LaunchUriAsync(string uri)
        {
            if (string.IsNullOrWhiteSpace(uri)) return false;

            try
            {
                var success = await Launcher.LaunchUriAsync(new Uri(uri));
                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[LauncherHelper] Exception launching URI {uri}: {ex}");
                return false;
            }
        }

        public static async Task<bool> LaunchLocalPathAsync(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return false;

            try
            {
                bool success;
                if (Directory.Exists(path))
                {
                    // Launch folder (requires Windows App SDK 1.2+ for LaunchFolderPathAsync)
                    success = await Launcher.LaunchFolderPathAsync(path);
                }
                else if (File.Exists(path))
                {
                    var file = await StorageFile.GetFileFromPathAsync(path);
                    success = await Launcher.LaunchFileAsync(file);
                }
                else
                {
                    Debug.WriteLine($"[LauncherHelper] Path not found: {path}");
                    return false;
                }

                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[LauncherHelper] Exception opening path {path}: {ex}");
                return false;
            }
        }
    }
}