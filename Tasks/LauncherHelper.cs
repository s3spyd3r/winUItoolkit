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
                return await Launcher.LaunchUriAsync(new Uri(uri));
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
                if (Directory.Exists(path))
                {
                    return await Launcher.LaunchFolderPathAsync(path);
                }

                if (File.Exists(path))
                {
                    var file = await StorageFile.GetFileFromPathAsync(path);
                    return await Launcher.LaunchFileAsync(file);
                }

                Debug.WriteLine($"[LauncherHelper] Path not found: {path}");
                return false;
            }
            catch (Exception ex)
            {
                // StorageFile.GetFileFromPathAsync can fail in unpackaged scenarios without broadFileSystemAccess.
                // Fall back to the shell to at least open the file with its default association.
                if (File.Exists(path))
                {
                    try
                    {
                        var psi = new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = path,
                            UseShellExecute = true
                        };
                        System.Diagnostics.Process.Start(psi);
                        return true;
                    }
                    catch (Exception fallbackEx)
                    {
                        Debug.WriteLine($"[LauncherHelper] Shell fallback failed for {path}: {fallbackEx}");
                    }
                }

                Debug.WriteLine($"[LauncherHelper] Exception opening path {path}: {ex}");
                return false;
            }
        }
    }
}
