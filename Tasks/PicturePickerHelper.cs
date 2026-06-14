using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;
using winUItoolkit.Helpers;

namespace winUItoolkit.Tasks
{
    public class PictureData
    {
        public byte[]? PictureBytes { get; set; }
        public string? Location { get; set; }
    }

    public interface IPicturePickerService
    {
        Task<PictureData?> PickPictureAsync(Window? window);
        Task<string?> SavePictureAsync(byte[] bytes, string fileName);
    }

    public class PicturePickerHelper : IPicturePickerService
    {
        public async Task<PictureData?> PickPictureAsync(Window? window)
        {
            var picker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                FileTypeFilter = { ".png", ".jpg", ".jpeg", ".bmp" }
            };
            if (window != null)
                InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(window));

            StorageFile? file = await picker.PickSingleFileAsync();
            if (file == null) return null;

            byte[] bytes;
            using (var stream = await file.OpenStreamForReadAsync())
            {
                bytes = new byte[stream.Length];
                await stream.ReadExactlyAsync(bytes);
            }

            return new PictureData
            {
                PictureBytes = bytes,
                Location = file.Path
            };
        }

        public async Task<string?> SavePictureAsync(byte[] bytes, string fileName)
        {
            if (bytes == null || bytes.Length == 0) return null;

            string safeName = string.IsNullOrWhiteSpace(fileName) ? Guid.NewGuid().ToString("N") + ".jpg" : fileName;

            if (RuntimeHelper.IsPackaged())
            {
                try
                {
                    var localFolder = ApplicationData.Current.LocalFolder;
                    var toolkitFolder = await localFolder.CreateFolderAsync("winUItoolkit", CreationCollisionOption.OpenIfExists);
                    var file = await toolkitFolder.CreateFileAsync(safeName, CreationCollisionOption.GenerateUniqueName);

                    using (var stream = await file.OpenStreamForWriteAsync())
                    {
                        await stream.WriteAsync(bytes, 0, bytes.Length);
                    }

                    return file.Path;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[PicturePickerHelper] SavePictureAsync packaged error: {ex}");
                }
            }

            // Unpackaged fallback: LocalApplicationData\winUItoolkit\<safeName>
            try
            {
                var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "winUItoolkit");
                Directory.CreateDirectory(folder);
                var unique = Path.Combine(folder, $"{Path.GetFileNameWithoutExtension(safeName)}_{Guid.NewGuid():N}{Path.GetExtension(safeName)}");
                await File.WriteAllBytesAsync(unique, bytes);
                return unique;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PicturePickerHelper] SavePictureAsync unpackaged error: {ex}");
                return null;
            }
        }
    }
}
