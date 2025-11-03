using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.System;

namespace winUItoolkit.PhoneTasks
{
    public static class LocationHelper
    {
        /// <summary>
        /// Retrieves the user's current geographic position.
        /// If location is disabled, prompts the user to enable it in system settings.
        /// </summary>
        /// <param name="accuracyInMeters">Desired accuracy (default 50m)</param>
        /// <param name="autoPromptUser">If true, opens Windows Settings when location is disabled</param>
        public static async Task<Geoposition?> GetPositionAsync(
            int accuracyInMeters = 50,
            bool autoPromptUser = true)
        {
            try
            {
                var geolocator = new Geolocator
                {
                    DesiredAccuracyInMeters = (uint)Math.Max(accuracyInMeters, 1)
                };

                // Check location access
                if (geolocator.LocationStatus == PositionStatus.Disabled)
                {
                    Debug.WriteLine("[GPSHelper] Location access is disabled.");

                    if (autoPromptUser)
                    {
                        // Open system location settings
                        await PromptEnableLocationAsync();
                    }

                    return null;
                }

                // Try to get current position
                Geoposition position = await geolocator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(5),
                    timeout: TimeSpan.FromSeconds(10)
                );

                return position;
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("[GPSHelper] Location permission denied by user.");
                if (autoPromptUser)
                {
                    await PromptEnableLocationAsync();
                }
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[GPSHelper] Error retrieving position: {ex}");
                return null;
            }
        }

        /// <summary>
        /// Opens the Windows Settings page for Location.
        /// </summary>
        public static async Task PromptEnableLocationAsync()
        {
            try
            {
                // Opens the "Privacy -> Location" settings page
                await Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-location"));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[GPSHelper] Failed to open location settings: {ex}");
            }
        }
    }
}
