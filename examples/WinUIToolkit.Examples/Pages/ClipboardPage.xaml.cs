using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using winUItoolkit.Helpers;

namespace WinUIToolkit.Examples.Pages
{
    public sealed partial class ClipboardPage : UserControl
    {
        public ClipboardPage()
        {
            InitializeComponent();
        }

        private void Set_Click(object sender, RoutedEventArgs e)
        {
            ClipboardHelper.SetText(SetText.Text);
        }

        private async void Get_Click(object sender, RoutedEventArgs e)
        {
            var text = await ClipboardHelper.GetTextAsync();
            ReadResult.Text = text ?? "(empty or not text)";
        }
    }
}
