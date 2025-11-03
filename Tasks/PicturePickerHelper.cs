using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

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
            // Set owner window if provided
            if (window != null)
                InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(window));

            StorageFile? file = await picker.PickSingleFileAsync();
            if (file == null) return null;

            byte[] bytes;
            using (var stream = await file.OpenStreamForReadAsync())
            {
                bytes = new byte[stream.Length];
                await stream.ReadAsync(bytes, 0, bytes.Length);
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

            // Use ApplicationData for local storage (more idiomatic than Environment.SpecialFolder)
            var localFolder = ApplicationData.Current.LocalFolder;
            var toolkitFolder = await localFolder.CreateFolderAsync("winUItoolkit", CreationCollisionOption.OpenIfExists);

            string safeName = string.IsNullOrWhiteSpace(fileName) ? Guid.NewGuid().ToString("N") + ".jpg" : fileName;
            var file = await toolkitFolder.CreateFileAsync(safeName, CreationCollisionOption.GenerateUniqueName); // Built-in unique naming

            using (var stream = await file.OpenStreamForWriteAsync())
            {
                await stream.WriteAsync(bytes, 0, bytes.Length);
            }

            return file.Path;
        }
    }
}