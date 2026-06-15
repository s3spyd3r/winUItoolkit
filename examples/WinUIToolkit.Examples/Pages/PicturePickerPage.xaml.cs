using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using winUItoolkit.Tasks;

namespace WinUIToolkit.Examples.Pages
{
    public sealed partial class PicturePickerPage : UserControl
    {
        private byte[]? _bytes;
        private readonly PicturePickerHelper _picker = new();

        public PicturePickerPage()
        {
            InitializeComponent();
        }

        private async void Pick_Click(object sender, RoutedEventArgs e)
        {
            var data = await _picker.PickPictureAsync(App.MainWindow);
            if (data?.PictureBytes == null) { Info.Text = "Cancelled."; return; }
            _bytes = data.PictureBytes;
            Info.Text = $"Picked {data.Location}\n{_bytes.Length} bytes";
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            if (_bytes == null) { SaveResult.Text = "Pick a picture first."; return; }
            var path = await _picker.SavePictureAsync(_bytes, FileName.Text);
            SaveResult.Text = path ?? "Save failed.";
        }
    }
}
