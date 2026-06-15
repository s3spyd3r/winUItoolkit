using System.Globalization;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using winUItoolkit.Helpers;

namespace WinUIToolkit.Examples.Pages
{
    public sealed partial class ColorConverterPage : UserControl
    {
        public ColorConverterPage()
        {
            InitializeComponent();
        }

        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            var color = ColorConverter.ConvertFromString(HexInput.Text);
            Swatch.Background = new SolidColorBrush(color);
            var (h, s, l) = ColorConverter.ToHsl(color);
            ConvertedText.Text = $"ARGB = #{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}  |  HSL = ({h:0.##}, {s:0.##}, {l:0.##})";
        }

        private void Lighten_Click(object sender, RoutedEventArgs e)
        {
            var amount = double.Parse(((Button)sender).Tag!.ToString()!, CultureInfo.InvariantCulture);
            var color = ColorConverter.Lighten(ColorConverter.ConvertFromString(HexInput.Text), amount);
            Swatch.Background = new SolidColorBrush(color);
            ConvertedText.Text = $"Lightened {amount}: {ColorConverter.ConvertToString(color)}";
        }

        private void Darken_Click(object sender, RoutedEventArgs e)
        {
            var amount = double.Parse(((Button)sender).Tag!.ToString()!, CultureInfo.InvariantCulture);
            var color = ColorConverter.Darken(ColorConverter.ConvertFromString(HexInput.Text), amount);
            Swatch.Background = new SolidColorBrush(color);
            ConvertedText.Text = $"Darkened {amount}: {ColorConverter.ConvertToString(color)}";
        }

        private void Desat_Click(object sender, RoutedEventArgs e)
        {
            var color = ColorConverter.AdjustSaturation(ColorConverter.ConvertFromString(HexInput.Text), -0.3);
            Swatch.Background = new SolidColorBrush(color);
            ConvertedText.Text = $"Desaturated: {ColorConverter.ConvertToString(color)}";
        }
    }
}
