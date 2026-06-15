using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage.Pickers;
using winUItoolkit.Tasks;

namespace WinUIToolkit.Examples.Pages
{
    public sealed partial class LauncherPage : UserControl
    {
        public LauncherPage()
        {
            InitializeComponent();
        }

        private async void Uri_Click(object sender, RoutedEventArgs e)
        {
            Status.Text = await LauncherHelper.LaunchUriAsync(UriBox.Text) ? "Launched" : "Failed";
        }

        private async void Path_Click(object sender, RoutedEventArgs e)
        {
            Status.Text = await LauncherHelper.LaunchLocalPathAsync(PathBox.Text) ? "Launched" : "Failed";
        }

        private async void PickFile_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add("*");
            WinRT.Interop.InitializeWithWindow.Initialize(picker, WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow));
            var f = await picker.PickSingleFileAsync().AsTask();
            if (f != null) PathBox.Text = f.Path;
        }

        private async void PickFolder_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FolderPicker();
            picker.FileTypeFilter.Add("*");
            WinRT.Interop.InitializeWithWindow.Initialize(picker, WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow));
            var f = await picker.PickSingleFolderAsync().AsTask();
            if (f != null) PathBox.Text = f.Path;
        }
    }
}
