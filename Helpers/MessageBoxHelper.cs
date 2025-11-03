using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace winUItoolkit.Helpers
{
    /// <summary>
    /// Helper for displaying message dialogs in WinUI 3 applications.
    /// </summary>
    public class MessageBoxHelper
    {
        private bool _messageShowing = false;

        /// <summary>
        /// Displays a simple message with an OK button.
        /// </summary>
        public async Task ShowAsync(string text, XamlRoot xamlRoot)
        {
            if (_messageShowing) return;

            _messageShowing = true;
            await RunOnUIThreadAsync(xamlRoot, async () =>
            {
                var dialog = new ContentDialog
                {
                    Content = new TextBlock
                    {
                        Text = text,
                        TextWrapping = TextWrapping.Wrap
                    },
                    CloseButtonText = "OK",
                    XamlRoot = xamlRoot
                };

                await dialog.ShowAsync();
                _messageShowing = false;
            });
        }

        /// <summary>
        /// Displays a message with a title.
        /// </summary>
        public async Task ShowAsync(string text, string caption, XamlRoot xamlRoot)
        {
            if (_messageShowing) return;

            _messageShowing = true;
            await RunOnUIThreadAsync(xamlRoot, async () =>
            {
                var dialog = new ContentDialog
                {
                    Title = caption,
                    Content = new TextBlock
                    {
                        Text = text,
                        TextWrapping = TextWrapping.Wrap
                    },
                    CloseButtonText = "OK",
                    XamlRoot = xamlRoot
                };

                await dialog.ShowAsync();
                _messageShowing = false;
            });
        }

        /// <summary>
        /// Displays a message dialog with multiple buttons and returns the selected button index.
        /// </summary>
        public async Task<int> ShowAsync(string text, string caption, IEnumerable<string> buttons, XamlRoot xamlRoot)
        {
            int selectedIndex = -1;
            _messageShowing = true;

            await RunOnUIThreadAsync(xamlRoot, async () =>
            {
                var dialog = new ContentDialog
                {
                    Title = caption,
                    Content = new TextBlock
                    {
                        Text = text,
                        TextWrapping = TextWrapping.Wrap
                    },
                    XamlRoot = xamlRoot
                };

                var buttonList = new List<string>(buttons);

                if (buttonList.Count == 1)
                {
                    dialog.CloseButtonText = buttonList[0];
                }
                else if (buttonList.Count == 2)
                {
                    dialog.PrimaryButtonText = buttonList[0];
                    dialog.CloseButtonText = buttonList[1];
                }
                else if (buttonList.Count >= 3)
                {
                    dialog.PrimaryButtonText = buttonList[0];
                    dialog.SecondaryButtonText = buttonList[1];
                    dialog.CloseButtonText = buttonList[2];
                }

                var result = await dialog.ShowAsync();

                selectedIndex = result switch
                {
                    ContentDialogResult.Primary => 0,
                    ContentDialogResult.Secondary => 1,
                    _ => buttonList.Count > 2 ? 2 : buttonList.Count - 1
                };

                _messageShowing = false;
            });

            return selectedIndex;
        }

        /// <summary>
        /// Displays a Yes/No confirmation dialog and returns true if the user confirms.
        /// </summary>
        public async Task<bool> ConfirmAsync(string message, XamlRoot xamlRoot, string title = "Confirm")
        {
            bool confirmed = false;

            await RunOnUIThreadAsync(xamlRoot, async () =>
            {
                var dialog = new ContentDialog
                {
                    Title = title,
                    Content = new TextBlock
                    {
                        Text = message,
                        TextWrapping = TextWrapping.Wrap
                    },
                    PrimaryButtonText = "Yes",
                    CloseButtonText = "No",
                    DefaultButton = ContentDialogButton.Close,
                    XamlRoot = xamlRoot
                };

                var result = await dialog.ShowAsync();
                confirmed = result == ContentDialogResult.Primary;
            });

            return confirmed;
        }

        private static async Task RunOnUIThreadAsync(XamlRoot xamlRoot, Func<Task> action)
        {
            // Try to obtain a DispatcherQueue from XamlRoot (available in newer WinUI versions).
            Microsoft.UI.Dispatching.DispatcherQueue? dispatcher = null;
            try
            {
                var prop = xamlRoot?.GetType().GetProperty("DispatcherQueue");
                if (prop != null)
                    dispatcher = prop.GetValue(xamlRoot) as Microsoft.UI.Dispatching.DispatcherQueue;
            }
            catch
            {
                dispatcher = null;
            }

            // Fallback to the current thread dispatcher queue if none obtained from XamlRoot
            if (dispatcher == null)
                dispatcher = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();

            if (dispatcher == null || dispatcher.HasThreadAccess)
            {
                await action();
                return;
            }

            var tcs = new TaskCompletionSource();
            dispatcher.TryEnqueue(async () =>
            {
                await action();
                tcs.SetResult();
            });
            await tcs.Task;
        }
    }
}