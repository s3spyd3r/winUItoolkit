using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.System;

namespace winUItoolkit.PhoneTasks
{
    /*
     * await LauncherHelper.LaunchUriAsync("https://www.google.com");
     * await LauncherHelper.LaunchUriAsync("mailto:test@test.com");
     * await LauncherHelper.LaunchUriAsync("ms-settings:privacy");
     * LauncherHelper.LaunchLocalPath("C:\\Users\\Public\\Documents");
     */

    public static class LauncherHelper
    {
        public static async Task LaunchUriAsync(string uri)
        {
            if (string.IsNullOrWhiteSpace(uri)) return;

            try
            {
                var targetUri = new Uri(uri, UriKind.Absolute);
                bool success = await Launcher.LaunchUriAsync(targetUri);

                if (!success)
                    Debug.WriteLine($"[LauncherHelper] Could not launch URI: {uri}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[LauncherHelper] Exception launching URI {uri}: {ex}");
            }
        }

        public static void LaunchLocalPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;

            try
            {
                if (File.Exists(path) || Directory.Exists(path))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = path,
                        UseShellExecute = true
                    });
                }
                else
                {
                    Debug.WriteLine($"[LauncherHelper] Path not found: {path}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[LauncherHelper] Exception opening path {path}: {ex}");
            }
        }
    }
}