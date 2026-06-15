using System;
using System.Globalization;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using winUItoolkit.Helpers;
using winUItoolkit.Tasks;

namespace WinUIToolkit.Examples.Pages
{
    public sealed partial class ImagePage : UserControl
    {
        private byte[]? _bytes;

        public ImagePage()
        {
            InitializeComponent();
        }

        private async void Pick_Click(object sender, RoutedEventArgs e)
        {
            var picker = new PicturePickerHelper();
            var window = App.MainWindow;
            var data = await picker.PickPictureAsync(window);
            if (data?.PictureBytes == null) return;
            _bytes = data.PictureBytes;
            SourceImage.Source = await BytesToBitmapAsync(_bytes);
        }

        private static async Task<BitmapImage> BytesToBitmapAsync(byte[] bytes)
        {
            var bmp = new BitmapImage();
            using var stream = new InMemoryRandomAccessStream();
            await stream.WriteAsync(bytes.AsBuffer());
            stream.Seek(0);
            await bmp.SetSourceAsync(stream);
            return bmp;
        }

        private async void Grayscale_Click(object sender, RoutedEventArgs e)
        {
            if (_bytes == null) return;
            var bmp = await ImageManipulationHelper.ToGrayscaleAsync(_bytes);
            OutputImage.Source = bmp;
        }

        private async void Invert_Click(object sender, RoutedEventArgs e)
        {
            if (_bytes == null) return;
            var bmp = await ImageManipulationHelper.InvertAsync(_bytes);
            OutputImage.Source = bmp;
        }

        private async void Brighten_Click(object sender, RoutedEventArgs e)
        {
            if (_bytes == null) return;
            var bmp = await ImageManipulationHelper.AdjustBrightnessAsync(_bytes, 1.5);
            OutputImage.Source = bmp;
        }

        private async void Resize_Click(object sender, RoutedEventArgs e)
        {
            if (_bytes == null) return;
            var bmp = await ImageManipulationHelper.ResizeAsync(_bytes, (uint)ResizeW.Value, (uint)ResizeH.Value);
            OutputImage.Source = bmp;
        }

        private async void Rotate_Click(object sender, RoutedEventArgs e)
        {
            if (_bytes == null) return;
            var angle = double.Parse(((Button)sender).Tag!.ToString()!, CultureInfo.InvariantCulture);
            var bmp = await ImageManipulationHelper.RotateAsync(_bytes, angle);
            OutputImage.Source = bmp;
        }
    }
}
