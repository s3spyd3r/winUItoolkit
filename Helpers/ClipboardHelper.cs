using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace winUItoolkit.Helpers
{
    public static class ClipboardHelper
    {
        public static Task SetTextAsync(string text)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(text ?? string.Empty);
            Clipboard.SetContent(dataPackage);
            return Task.CompletedTask;
        }

        public static async Task<string?> GetTextAsync()
        {
            try
            {
                var data = Clipboard.GetContent();
                if (data.Contains(StandardDataFormats.Text))
                    return await data.GetTextAsync().AsTask().ConfigureAwait(false);
            }
            catch { }
            return null;
        }
    }
}
