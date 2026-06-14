using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace winUItoolkit.Helpers
{
    /// <summary>
    /// Helper for displaying message dialogs in WinUI 3 applications.
    /// </summary>
    public static class MessageBoxHelper
    {
        private static readonly SemaphoreSlim _gate = new(1, 1);

        /// <summary>
        /// Displays a simple message with an OK button.
        /// </summary>
        public static Task ShowAsync(string text, XamlRoot xamlRoot)
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

            return RunOnUIThreadAsync(xamlRoot, () => dialog.ShowAsync().AsTask());
        }

        /// <summary>
        /// Displays a message with a title.
        /// </summary>
        public static Task ShowAsync(string text, string caption, XamlRoot xamlRoot)
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

            return RunOnUIThreadAsync(xamlRoot, () => dialog.ShowAsync().AsTask());
        }

        /// <summary>
        /// Displays a message dialog with multiple buttons and returns the selected button index.
        /// </summary>
        public static Task<int> ShowAsync(string text, string caption, IEnumerable<string> buttons, XamlRoot xamlRoot)
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

            return RunOnUIThreadAsync(xamlRoot, () => MapButtonResultAsync(buttonList, dialog));
        }

        private static async Task<int> MapButtonResultAsync(List<string> buttonList, ContentDialog dialog)
        {
            var result = await dialog.ShowAsync().AsTask();
            return result switch
            {
                ContentDialogResult.Primary => 0,
                ContentDialogResult.Secondary => 1,
                _ => buttonList.Count > 2 ? 2 : Math.Max(0, buttonList.Count - 1)
            };
        }

        /// <summary>
        /// Displays a Yes/No confirmation dialog and returns true if the user confirms.
        /// </summary>
        public static async Task<bool> ConfirmAsync(string message, XamlRoot xamlRoot, string title = "Confirm")
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

            var result = await RunOnUIThreadAsync(xamlRoot, () => dialog.ShowAsync().AsTask());
            return result == ContentDialogResult.Primary;
        }

        private static async Task RunOnUIThreadAsync(XamlRoot xamlRoot, Func<Task> action)
        {
            await _gate.WaitAsync();
            try
            {
                var dispatcher = DispatcherQueue.GetForCurrentThread();
                if (dispatcher == null || dispatcher.HasThreadAccess)
                {
                    await action();
                    return;
                }

                var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
                var queued = dispatcher.TryEnqueue(async () =>
                {
                    try
                    {
                        await action();
                        tcs.TrySetResult();
                    }
                    catch (Exception ex)
                    {
                        tcs.TrySetException(ex);
                    }
                });

                if (!queued)
                {
                    throw new InvalidOperationException("Failed to enqueue action on dispatcher queue.");
                }

                await tcs.Task;
            }
            finally
            {
                _gate.Release();
            }
        }

        private static async Task<T> RunOnUIThreadAsync<T>(XamlRoot xamlRoot, Func<Task<T>> action)
        {
            await _gate.WaitAsync();
            try
            {
                var dispatcher = DispatcherQueue.GetForCurrentThread();
                if (dispatcher == null || dispatcher.HasThreadAccess)
                {
                    return await action();
                }

                var tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
                var queued = dispatcher.TryEnqueue(async () =>
                {
                    try
                    {
                        tcs.TrySetResult(await action());
                    }
                    catch (Exception ex)
                    {
                        tcs.TrySetException(ex);
                    }
                });

                if (!queued)
                {
                    throw new InvalidOperationException("Failed to enqueue action on dispatcher queue.");
                }

                return await tcs.Task;
            }
            finally
            {
                _gate.Release();
            }
        }
    }
}
