using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace winUItoolkit.Helpers
{
    public static class ThemeHelper
    {
        public static void SetApplicationTheme(ElementTheme theme)
        {
            if (Application.Current is not Application app) return;

            app.RequestedTheme = theme switch
            {
                ElementTheme.Dark => ApplicationTheme.Dark,
                _ => ApplicationTheme.Light
            };
        }

        public static void ApplyAccentColor(Color color)
        {
            var brush = new SolidColorBrush(color);
            Application.Current.Resources["SystemAccentColor"] = color;
            Application.Current.Resources["SystemControlForegroundAccentBrush"] = brush;
        }
    }
}
