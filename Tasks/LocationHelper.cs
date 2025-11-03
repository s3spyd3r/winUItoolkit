using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace winUItoolkit.Tasks
{
    /// <summary>
    /// Defines the unit of distance measurement.
    /// </summary>
    public enum DistanceType
    {
        Miles,
        Kilometers
    }

    public static class LocationHelper
    {
        private const double EARTH_RADIUS_KM = 6371.0;
        private const double EARTH_RADIUS_MILES = 3958.8;

        /// <summary>
        /// Retrieves the user's current geographic position.
        /// If location is disabled, prompts the user to enable it in system settings.
        /// </summary>
        /// <param name="accuracyInMeters">Desired accuracy (default 50m)</param>
        /// <param name="autoPromptUser">If true, opens Windows Settings when location is disabled</param>
        public static async Task<Geoposition?> GetPositionAsync(int accuracyInMeters = 50, bool autoPromptUser = true)
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
                Geoposition position = await geolocator.GetGeopositionAsync(maximumAge: TimeSpan.FromMinutes(5), timeout: TimeSpan.FromSeconds(10));

                return position;
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("[GPSHelper] Location permission denied by user.");
                if (autoPromptUser)
                    await PromptEnableLocationAsync();

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
        public static Task PromptEnableLocationAsync()
        {
            try
            {
                // Opens the "Privacy -> Location" settings page using shell on desktop
                Process.Start(new ProcessStartInfo
                {
                    FileName = "ms-settings:privacy-location",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[GPSHelper] Failed to open location settings: {ex}");
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Calculates the distance between two coordinates.
        /// </summary>
        /// <param name="lat1">Latitude of the first location.</param>
        /// <param name="lon1">Longitude of the first location.</param>
        /// <param name="lat2">Latitude of the second location.</param>
        /// <param name="lon2">Longitude of the second location.</param>
        /// <param name="type">The distance unit (Miles or Kilometers).</param>
        /// <returns>The distance between the two coordinates.</returns>
        public static double Distance(double lat1, double lon1, double lat2, double lon2, DistanceType type = DistanceType.Kilometers)
        {
            double radius = type == DistanceType.Miles ? EARTH_RADIUS_MILES : EARTH_RADIUS_KM;

            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);

            double a = Math.Pow(Math.Sin(dLat / 2), 2) +
                       Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                       Math.Pow(Math.Sin(dLon / 2), 2);

            double c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));

            return radius * c;
        }

        /// <summary>
        /// Calculates the distance between two Geopoints.
        /// </summary>
        /// <param name="point1">First Geopoint.</param>
        /// <param name="point2">Second Geopoint.</param>
        /// <param name="type">The distance unit (Miles or Kilometers).</param>
        /// <returns>The distance between the two points.</returns>
        public static double Distance(Geopoint point1, Geopoint point2, DistanceType type = DistanceType.Kilometers)
        {
            return Distance(
                point1.Position.Latitude,
                point1.Position.Longitude,
                point2.Position.Latitude,
                point2.Position.Longitude,
                type);
        }

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        private static double ToRadians(double angle) => angle * Math.PI / 180.0;

        /// <summary>
        /// Converts miles to kilometers.
        /// </summary>
        public static double MilesToKilometers(double miles) => miles * 1.60934;

        /// <summary>
        /// Converts kilometers to miles.
        /// </summary>
        public static double KilometersToMiles(double kilometers) => kilometers / 1.60934;
    }
}
