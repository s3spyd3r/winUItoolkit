using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using winUItoolkit.IO;

namespace WinUIToolkit.Examples.Pages
{
    public sealed partial class StoragePage : UserControl
    {
        public StoragePage()
        {
            InitializeComponent();
        }

        private async void SaveLocal_Click(object sender, RoutedEventArgs e)
        {
            await StorageHelper.SetIntoLocalSettingsAsync(SettingsKey.Text, SettingsValue.Text);
            LocalResult.Text = $"Saved '{SettingsKey.Text}'.";
        }

        private async void ReadLocal_Click(object sender, RoutedEventArgs e)
        {
            var v = await StorageHelper.GetFromLocalSettingsAsync<string>(SettingsKey.Text);
            LocalResult.Text = v is null ? "(no value)" : v;
        }
    }
}
