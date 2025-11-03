using System.Net.NetworkInformation;
using Windows.Networking.Connectivity;

namespace winUItoolkit.Http
{
    public static class NetHelper
    {
        /// <summary>
        /// Checks whether the device currently has an active internet connection.
        /// </summary>
        public static bool CheckNetworkConnection()
        {
            // Quick system-level check
            if (!NetworkInterface.GetIsNetworkAvailable()) return false;

            // Retrieve the current connection profile (can be null)
            ConnectionProfile? profile = NetworkInformation.GetInternetConnectionProfile();

            if (profile is null)
                return false;

            // Determine if we actually have internet access
            NetworkConnectivityLevel connectivity = profile.GetNetworkConnectivityLevel();

            return connectivity == NetworkConnectivityLevel.InternetAccess ||
                   connectivity == NetworkConnectivityLevel.ConstrainedInternetAccess;
        }
    }
}