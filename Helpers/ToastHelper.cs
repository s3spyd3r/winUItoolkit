using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Threading.Tasks;

namespace Toolkit.Helpers
{
    /// <summary>
    /// Displays a temporary toast-style message in a WinUI 3 app using a provided XamlRoot.
    /// </summary>
    public static class ToastHelper
    {
        /// <summary>
        /// Shows a toast notification with text and optional duration.
        /// </summary>
        /// <param name="message">The text to display in the toast.</param>
        /// <param name="xamlRoot">The XamlRoot from the caller's visual tree (e.g. Page.XamlRoot).</param>
        /// <param name="duration">How long to display the toast (default: 3 seconds).</param>
        public static async Task ShowToastAsync(string message, XamlRoot xamlRoot, int duration = 3000)
        {
            if (xamlRoot == null)
                throw new ArgumentNullException(nameof(xamlRoot), "You must pass a valid XamlRoot.");

            var rootGrid = new Grid
            {
                Background = new SolidColorBrush(Microsoft.UI.Colors.Black) { Opacity = 0.8 },
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(16),
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(20),
                RenderTransform = new TranslateTransform { Y = 100 },
                Opacity = 0
            };

            var textBlock = new TextBlock
            {
                Text = message,
                Foreground = new SolidColorBrush(Microsoft.UI.Colors.White),
                TextWrapping = TextWrapping.WrapWholeWords,
                TextAlignment = TextAlignment.Center,
                FontSize = 14
            };

            rootGrid.Children.Add(textBlock);

            // Create popup for toast
            var popup = new Popup
            {
                Child = rootGrid,
                IsOpen = true,
                XamlRoot = xamlRoot
            };

            // Animate fade in + slide up
            var fadeIn = new DoubleAnimation { To = 1, Duration = new Duration(TimeSpan.FromMilliseconds(300)) };
            var slideUp = new DoubleAnimation { To = 0, Duration = new Duration(TimeSpan.FromMilliseconds(300)) };

            var fadeStoryboard = new Storyboard();
            Storyboard.SetTarget(fadeIn, rootGrid);
            Storyboard.SetTargetProperty(fadeIn, "Opacity");

            Storyboard.SetTarget(slideUp, rootGrid.RenderTransform);
            Storyboard.SetTargetProperty(slideUp, "Y");

            fadeStoryboard.Children.Add(fadeIn);
            fadeStoryboard.Children.Add(slideUp);
            fadeStoryboard.Begin();

            // Wait for duration, then fade out
            await Task.Delay(duration);

            var fadeOut = new DoubleAnimation { To = 0, Duration = new Duration(TimeSpan.FromMilliseconds(400)) };
            var slideDown = new DoubleAnimation { To = 50, Duration = new Duration(TimeSpan.FromMilliseconds(400)) };

            var outStoryboard = new Storyboard();
            Storyboard.SetTarget(fadeOut, rootGrid);
            Storyboard.SetTargetProperty(fadeOut, "Opacity");

            Storyboard.SetTarget(slideDown, rootGrid.RenderTransform);
            Storyboard.SetTargetProperty(slideDown, "Y");

            outStoryboard.Children.Add(fadeOut);
            outStoryboard.Children.Add(slideDown);
            outStoryboard.Begin();

            // Give time for fade out, then close popup
            await Task.Delay(500);
            popup.IsOpen = false;
        }
    }
}