using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUIToolkit.Examples.Sample;

namespace WinUIToolkit.Examples.Pages
{
    public sealed partial class SingletonPage : UserControl
    {
        public SingletonPage()
        {
            InitializeComponent();
            Refresh();
        }

        private void Record_Click(object sender, RoutedEventArgs e)
        {
            AppState.Instance.Record(Input.Text);
            Refresh();
        }

        private void Refresh()
        {
            Status.Text = $"Singleton count: {AppState.Instance.InteractionCount}\nLast: {AppState.Instance.LastMessage}";
        }
    }
}
