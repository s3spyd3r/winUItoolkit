using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace winUItoolkit.Helpers
{
    public static class ThemeHelper
    {
        public static void SetApplicationTheme(ElementTheme theme)
        {
            if (Application.Current is Application app)
            {
                app.RequestedTheme = theme == ElementTheme.Default ? ApplicationTheme.Light : (theme == ElementTheme.Dark ? ApplicationTheme.Dark : ApplicationTheme.Light);
            }
        }

        public static void ApplyAccentColor(Color color)
        {
            var brush = new SolidColorBrush(color);
            Application.Current.Resources["SystemAccentColor"] = color;
            Application.Current.Resources["SystemControlForegroundAccentBrush"] = brush;
        }
    }
}