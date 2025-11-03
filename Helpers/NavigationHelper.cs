using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace winUItoolkit.Helpers
{
    public static class NavigationHelper
    {
        // Navigate within a Frame
        public static bool Navigate(Frame frame, Type pageType, object? parameter = null)
        {
            if (frame == null || pageType == null) return false;
            return frame.Navigate(pageType, parameter);
        }

        // Open a new Window and set Content to the provided UIElement
        public static Window OpenWindow(UIElement content, string? title = null)
        {
            var window = new Window();
            if (!string.IsNullOrWhiteSpace(title) && window.Content is FrameworkElement fe)
            {
                fe.Name = title;
            }
            window.Content = content;
            window.Activate();
            return window;
        }
    }
}
