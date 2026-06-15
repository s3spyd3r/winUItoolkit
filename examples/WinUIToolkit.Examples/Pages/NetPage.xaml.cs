using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using winUItoolkit.Http;

namespace WinUIToolkit.Examples.Pages
{
    public sealed partial class NetPage : UserControl
    {
        public NetPage()
        {
            InitializeComponent();
        }

        private void Check_Click(object sender, RoutedEventArgs e)
        {
            Result.Text = NetHelper.CheckNetworkConnection() ? "Connected" : "No connection";
        }
    }
}
