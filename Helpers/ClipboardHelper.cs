using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace winUItoolkit.Helpers
{
    public static class ClipboardHelper
    {
        /// <summary>
        /// Sets the clipboard text. Must be called on the UI thread; <see cref="Clipboard.SetContent"/>
        /// throws on background threads in WinUI 3 desktop.
        /// </summary>
        public static void SetText(string text)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(text ?? string.Empty);
            Clipboard.SetContent(dataPackage);
        }

        public static async Task<string?> GetTextAsync()
        {
            try
            {
                var data = Clipboard.GetContent();
                if (data.Contains(StandardDataFormats.Text))
                    return await data.GetTextAsync().AsTask().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ClipboardHelper] GetTextAsync error: {ex}");
            }
            return null;
        }
    }
}
