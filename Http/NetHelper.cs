using System.Net.NetworkInformation;
using Windows.Networking.Connectivity;

namespace winUItoolkit.Http
{
    public static class NetHelper
    {
        /// <summary>
        /// Checks whether the device currently has an active internet connection.
        /// </summary>
        /// <remarks>
        /// <c>ConstrainedInternetAccess</c> indicates a captive portal (e.g. coffee-shop Wi-Fi). Such networks
        /// return <c>true</c> here, but many HTTP requests will still fail until the user signs in.
        /// </remarks>
        public static bool CheckNetworkConnection()
        {
            if (!NetworkInterface.GetIsNetworkAvailable()) return false;

            ConnectionProfile? profile = NetworkInformation.GetInternetConnectionProfile();
            if (profile is null) return false;

            NetworkConnectivityLevel connectivity = profile.GetNetworkConnectivityLevel();

            return connectivity == NetworkConnectivityLevel.InternetAccess ||
                   connectivity == NetworkConnectivityLevel.ConstrainedInternetAccess;
        }
    }
}
