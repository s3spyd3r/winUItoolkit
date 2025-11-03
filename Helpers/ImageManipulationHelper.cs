using System;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI;

namespace winUItoolkit.Helpers
{
    public static class ImageManipulationHelper
    {
        #region Download / Load

        public static async Task<byte[]?> GetImageByteArrayAsync(string? url)
        {
            if (string.IsNullOrWhiteSpace(url)) return null;

            try
            {
                using var client = new HttpClient();
                return await client.GetByteArrayAsync(url);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetImageByteArrayAsync error: {ex}");
                return null;
            }
        }

        public static async Task<BitmapImage?> LoadBitmapFromUrlAsync(string url)
        {
            var bytes = await GetImageByteArrayAsync(url);
            if (bytes == null)
                return null;

            using var stream = new InMemoryRandomAccessStream();
            await stream.WriteAsync(bytes.AsBuffer());
            stream.Seek(0);

            var bitmap = new BitmapImage();
            await bitmap.SetSourceAsync(stream);
            return bitmap;
        }

        #endregion

        #region Color Replacement

        public static async Task<BitmapImage?> ReplaceColorAsync(byte[] imagePixels, Color from, Color to, bool ignoreTransparency = true)
        {
            if (imagePixels == null) return null;

            var newPixels = ApplyColorTransform(imagePixels, from, to, ignoreTransparency);
            return await ConvertBytesToBitmapAsync(newPixels, 256, 256);
        }

        private static byte[] ApplyColorTransform(byte[] pixels, Color from, Color to, bool ignoreTransparency)
        {
            for (int i = 0; i < pixels.Length; i += 4)
            {
                byte b = pixels[i];
                byte g = pixels[i + 1];
                byte r = pixels[i + 2];
                byte a = pixels[i + 3];

                bool matches =
                    b == from.B &&
                    g == from.G &&
                    r == from.R &&
                    (ignoreTransparency || a == from.A);

                if (matches)
                {
                    pixels[i] = to.B;
                    pixels[i + 1] = to.G;
                    pixels[i + 2] = to.R;
                    if (!ignoreTransparency)
                        pixels[i + 3] = to.A;
                }
            }
            return pixels;
        }

        #endregion

        #region Transformations

        public static async Task<BitmapImage?> ResizeAsync(byte[] imageBytes, uint newWidth, uint newHeight)
        {
            try
            {
                using var stream = new InMemoryRandomAccessStream();
                await stream.WriteAsync(imageBytes.AsBuffer());
                stream.Seek(0);

                var decoder = await BitmapDecoder.CreateAsync(stream);
                var transform = new BitmapTransform { ScaledWidth = newWidth, ScaledHeight = newHeight };
                var provider = await decoder.GetPixelDataAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, transform, ExifOrientationMode.RespectExifOrientation, ColorManagementMode.ColorManageToSRgb);

                var pixels = provider.DetachPixelData();
                return await ConvertBytesToBitmapAsync(pixels, newWidth, newHeight);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ResizeAsync error: {ex}");
                return null;
            }
        }

        public static async Task<BitmapImage?> CropAsync(byte[] imageBytes, uint x, uint y, uint width, uint height)
        {
            try
            {
                using var stream = new InMemoryRandomAccessStream();
                await stream.WriteAsync(imageBytes.AsBuffer());
                stream.Seek(0);

                var decoder = await BitmapDecoder.CreateAsync(stream);
                var transform = new BitmapTransform
                {
                    Bounds = new BitmapBounds { X = x, Y = y, Width = width, Height = height }
                };

                var provider = await decoder.GetPixelDataAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, transform, ExifOrientationMode.RespectExifOrientation, ColorManagementMode.ColorManageToSRgb);
                var pixels = provider.DetachPixelData();

                return await ConvertBytesToBitmapAsync(pixels, width, height);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CropAsync error: {ex}");
                return null;
            }
        }

        public static async Task<BitmapImage?> RotateAsync(byte[] imageBytes, double angle)
        {
            try
            {
                using var stream = new InMemoryRandomAccessStream();
                await stream.WriteAsync(imageBytes.AsBuffer());
                stream.Seek(0);

                var decoder = await BitmapDecoder.CreateAsync(stream);
                BitmapRotation rotation = angle switch
                {
                    90 => BitmapRotation.Clockwise90Degrees,
                    180 => BitmapRotation.Clockwise180Degrees,
                    270 => BitmapRotation.Clockwise270Degrees,
                    _ => BitmapRotation.None
                };

                var transform = new BitmapTransform { Rotation = rotation };
                var provider = await decoder.GetPixelDataAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, transform, ExifOrientationMode.IgnoreExifOrientation, ColorManagementMode.ColorManageToSRgb);

                var pixels = provider.DetachPixelData();
                return await ConvertBytesToBitmapAsync(pixels, decoder.PixelWidth, decoder.PixelHeight);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"RotateAsync error: {ex}");
                return null;
            }
        }

        #endregion

        #region Filters

        public static async Task<BitmapImage?> AdjustBrightnessAsync(byte[] imageBytes, double factor)
        {
            return await ApplyPixelFilterAsync(imageBytes, (b, g, r, a) =>
            {
                byte Clamp(double value) => (byte)Math.Min(255, Math.Max(0, value));
                return (Clamp(r * factor), Clamp(g * factor), Clamp(b * factor), a);
            });
        }

        public static async Task<BitmapImage?> ToGrayscaleAsync(byte[] imageBytes)
        {
            return await ApplyPixelFilterAsync(imageBytes, (b, g, r, a) =>
            {
                var gray = (byte)(0.299 * r + 0.587 * g + 0.114 * b);
                return (gray, gray, gray, a);
            });
        }

        public static async Task<BitmapImage?> InvertAsync(byte[] imageBytes)
        {
            return await ApplyPixelFilterAsync(imageBytes, (b, g, r, a) => ((byte)(255 - r), (byte)(255 - g), (byte)(255 - b), a));
        }

        private static async Task<BitmapImage?> ApplyPixelFilterAsync(byte[] imageBytes, Func<byte, byte, byte, byte, (byte r, byte g, byte b, byte a)> transform)
        {
            try
            {
                using var stream = new InMemoryRandomAccessStream();
                await stream.WriteAsync(imageBytes.AsBuffer());
                stream.Seek(0);

                var decoder = await BitmapDecoder.CreateAsync(stream);
                var provider = await decoder.GetPixelDataAsync();
                var pixels = provider.DetachPixelData();

                for (int i = 0; i < pixels.Length; i += 4)
                {
                    var (r, g, b, a) = transform(pixels[i + 2], pixels[i + 1], pixels[i], pixels[i + 3]);
                    pixels[i] = b;
                    pixels[i + 1] = g;
                    pixels[i + 2] = r;
                    pixels[i + 3] = a;
                }

                return await ConvertBytesToBitmapAsync(pixels, decoder.PixelWidth, decoder.PixelHeight);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ApplyPixelFilterAsync error: {ex}");
                return null;
            }
        }

        #endregion

        #region Internal Helpers

        private static async Task<BitmapImage?> ConvertBytesToBitmapAsync(byte[] pixels, uint width, uint height)
        {
            using var memoryStream = new InMemoryRandomAccessStream();
            await EncodeToStreamAsync(memoryStream, pixels, width, height);
            var image = new BitmapImage();
            await image.SetSourceAsync(memoryStream);
            return image;
        }

        private static async Task EncodeToStreamAsync(IRandomAccessStream stream, byte[] pixels, uint width, uint height)
        {
            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, width, height, 96, 96, pixels);
            await encoder.FlushAsync();
        }

        #endregion
    }
}