using System.Text.Json;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using winUItoolkit.Http;

namespace WinUIToolkit.Examples.Pages
{
    public sealed partial class HttpPage : UserControl
    {
        public HttpPage()
        {
            InitializeComponent();
        }

        private async void Get_Click(object sender, RoutedEventArgs e)
        {
            ServicesHelper.ServerAddress = Server.Text;
            var raw = await ServicesHelper.HttpRequestAsync(RestRequestTypes.Get, Endpoint.Text);
            if (string.IsNullOrEmpty(raw))
            {
                Response.Text = "(empty response or error)";
                return;
            }

            try
            {
                using var doc = JsonDocument.Parse(raw);
                Response.Text = JsonSerializer.Serialize(doc.RootElement, new JsonSerializerOptions { WriteIndented = true });
            }
            catch
            {
                Response.Text = raw;
            }
        }
    }
}
