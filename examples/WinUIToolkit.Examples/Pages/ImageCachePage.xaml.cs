using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using winUItoolkit.Http;

namespace WinUIToolkit.Examples.Pages
{
    public sealed partial class ImageCachePage : UserControl
    {
        public ImageCachePage()
        {
            InitializeComponent();
        }

        private async void Load_Click(object sender, RoutedEventArgs e)
        {
            var sw = Stopwatch.StartNew();
            var bmp = await ImageCacheHelper.GetCachedImageAsync(UrlBox.Text);
            sw.Stop();
            Output.Source = bmp;
            Status.Text = bmp == null ? "Failed to load image." : $"Loaded in {sw.ElapsedMilliseconds} ms.";
        }

        private async void Clear_Click(object sender, RoutedEventArgs e)
        {
            await ImageCacheHelper.ClearCacheAsync();
            Status.Text = "Cache cleared.";
        }
    }
}
