using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using winUItoolkit.Tasks;

namespace WinUIToolkit.Examples.Pages
{
    public sealed partial class LocationPage : UserControl
    {
        public LocationPage()
        {
            InitializeComponent();
        }

        private async void GetPosition_Click(object sender, RoutedEventArgs e)
        {
            var pos = await LocationHelper.GetPositionAsync();
            if (pos == null)
            {
                Position.Text = "Could not retrieve position (permission denied or disabled).";
                return;
            }
            Position.Text = $"Lat: {pos.Coordinate.Latitude:0.0000}\nLon: {pos.Coordinate.Longitude:0.0000}\nAccuracy: {pos.Coordinate.Accuracy:0} m";
        }

        private void Distance_Click(object sender, RoutedEventArgs e)
        {
            var km = LocationHelper.Distance(Lat1.Value, Lon1.Value, Lat2.Value, Lon2.Value, DistanceType.Kilometers);
            var mi = LocationHelper.KilometersToMiles(km);
            Distance.Text = $"{km:0.00} km ({mi:0.00} miles)";
        }
    }
}
