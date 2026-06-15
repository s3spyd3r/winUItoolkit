using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using winUItoolkit.Helpers;

namespace WinUIToolkit.Examples.Pages
{
    public sealed partial class VisualElementsPage : UserControl
    {
        public VisualElementsPage()
        {
            InitializeComponent();
        }

        private void Find_Click(object sender, RoutedEventArgs e)
        {
            var buttons = VisualElementsHelper.FindVisualChildren<Button>(TreeRoot);
            Result.Text = string.Join("\n", buttons.Select(b => $"{b.GetType().Name}: '{b.Content}'"));
        }

        private void Dump_Click(object sender, RoutedEventArgs e)
        {
            Result.Text = VisualElementsHelper.DumpVisualTree(TreeRoot);
        }
    }
}
