using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace winUItoolkit.Helpers
{
    /// <summary>
    /// Utility class for traversing, searching, and debugging the WinUI 3 visual tree.
    /// </summary>
    public static class VisualElementsHelper
    {
        /// <summary>
        /// Recursively searches the visual tree to find a ScrollViewer inside the given DependencyObject.
        /// </summary>
        public static ScrollViewer? GetScrollViewer(DependencyObject depObj)
        {
            if (depObj is ScrollViewer viewer) return viewer;

            int count = VisualTreeHelper.GetChildrenCount(depObj);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                ScrollViewer? result = GetScrollViewer(child);
                if (result != null)
                    return result;
            }

            return null;
        }

        /// <summary>
        /// Recursively finds all visual children of a given type in the visual tree.
        /// </summary>
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject? depObj) where T : DependencyObject
        {
            if (depObj == null)
                yield break;

            int count = VisualTreeHelper.GetChildrenCount(depObj);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                if (child is T t)
                    yield return t;

                foreach (T childOfChild in FindVisualChildren<T>(child))
                    yield return childOfChild;
            }
        }

        /// <summary>
        /// Finds the first parent of a given type in the visual tree.
        /// </summary>
        public static T? GetParentByType<T>(DependencyObject element) where T : FrameworkElement
        {
            DependencyObject? parent = VisualTreeHelper.GetParent(element);
            while (parent != null)
            {
                if (parent is T t)
                    return t;

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }

        /// <summary>
        /// Finds the top-level parent (e.g. the root visual) of the given element.
        /// </summary>
        public static DependencyObject? GetRoot(DependencyObject element)
        {
            DependencyObject current = element;
            DependencyObject? parent = VisualTreeHelper.GetParent(current);
            while (parent != null)
            {
                current = parent;
                parent = VisualTreeHelper.GetParent(current);
            }

            return current;
        }

        /// <summary>
        /// Finds the first ancestor of a given type in the logical tree (useful for templates or content presenters).
        /// </summary>
        public static T? GetAncestorOfType<T>(DependencyObject element) where T : DependencyObject
        {
            DependencyObject? parent = element;
            while (parent != null)
            {
                if (parent is T typedParent)
                    return typedParent;

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }

        /// <summary>
        /// Attempts to find a named child element within the visual tree.
        /// </summary>
        public static T? FindChildByName<T>(DependencyObject parent, string name) where T : FrameworkElement
        {
            if (parent == null) return null;

            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                if (child is T frameworkElement && frameworkElement.Name == name)
                    return frameworkElement;

                var result = FindChildByName<T>(child, name);
                if (result != null)
                    return result;
            }

            return null;
        }

        /// <summary>
        /// Dumps the full visual tree hierarchy to a string for debugging.
        /// </summary>
        public static string DumpVisualTree(DependencyObject root, bool includeNames = true)
        {
            var sb = new StringBuilder();
            DumpVisualTreeInternal(root, sb, 0, includeNames);
            return sb.ToString();
        }

        /// <summary>
        /// Logs the visual tree to Debug output (for easy inspection in Visual Studio).
        /// </summary>
        public static void LogVisualTree(DependencyObject root, bool includeNames = true)
        {
            var dump = DumpVisualTree(root, includeNames);
            System.Diagnostics.Debug.WriteLine("=== VISUAL TREE DUMP ===");
            System.Diagnostics.Debug.WriteLine(dump);
        }

        private static void DumpVisualTreeInternal(DependencyObject element, StringBuilder sb, int depth, bool includeNames)
        {
            string indent = new string(' ', depth * 2);
            string typeName = element.GetType().Name;

            if (includeNames && element is FrameworkElement fe && !string.IsNullOrEmpty(fe.Name))
                sb.AppendLine($"{indent}- {typeName} (Name: {fe.Name})");
            else
                sb.AppendLine($"{indent}- {typeName}");

            int count = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(element, i);
                DumpVisualTreeInternal(child, sb, depth + 1, includeNames);
            }
        }
    }
}