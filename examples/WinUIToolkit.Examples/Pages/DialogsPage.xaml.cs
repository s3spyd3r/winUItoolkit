using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using winUItoolkit.Helpers;

namespace WinUIToolkit.Examples.Pages
{
    public sealed partial class DialogsPage : UserControl
    {
        public DialogsPage()
        {
            InitializeComponent();
        }

        private async void SimpleDialog_Click(object sender, RoutedEventArgs e)
        {
            await MessageBoxHelper.ShowAsync("This is a simple dialog.", XamlRoot!);
        }

        private async void TitledDialog_Click(object sender, RoutedEventArgs e)
        {
            await MessageBoxHelper.ShowAsync("Body text for the dialog.", "Title goes here", XamlRoot!);
        }

        private async void MultiButton_Click(object sender, RoutedEventArgs e)
        {
            var idx = await MessageBoxHelper.ShowAsync(
                "Choose an option.",
                "Multi-button",
                new[] { "Save", "Discard", "Cancel" },
                XamlRoot!);
            DialogResult.Text = $"You selected index {idx}.";
        }

        private async void Confirm_Click(object sender, RoutedEventArgs e)
        {
            var ok = await MessageBoxHelper.ConfirmAsync("Proceed with the action?", XamlRoot!, "Confirm");
            ConfirmResult.Text = ok ? "Confirmed." : "Cancelled.";
        }

        private async void Toast_Click(object sender, RoutedEventArgs e)
        {
            await ToastHelper.ShowToastAsync(ToastText.Text, XamlRoot!);
        }
    }
}
