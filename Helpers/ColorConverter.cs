using System;
using System.Globalization;
using Microsoft.UI;
using Windows.UI;

namespace winUItoolkit.Helpers
{
    /*
     * var color = ColorConverter.ConvertFromString("#1E90FF");
     * string hex = ColorConverter.ConvertToString(color);
     * var lighter = ColorConverter.Lighten(color, 0.2);
     * var darker = ColorConverter.Darken(color, 0.2);
     * var desaturated = ColorConverter.AdjustSaturation(color, -0.3);
     * var (h, s, l) = ColorConverter.ToHsl(color);
     * var fromHsl = ColorConverter.FromHsl(h, s, l);
     */

    public static class ColorConverter
    {
        #region HEX ⇄ COLOR

        /// <summary>
        /// Converts a hex color string (e.g. "#RRGGBB" or "#AARRGGBB") into a WinUI Color.
        /// </summary>
        public static Color ConvertFromString(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex)) return Colors.Black;

            hex = hex.Trim().TrimStart('#');

            // Handle shorthand (#RGB, #ARGB)
            if (hex.Length == 3)
                hex = $"{hex[0]}{hex[0]}{hex[1]}{hex[1]}{hex[2]}{hex[2]}";
            else if (hex.Length == 4)
                hex = $"{hex[0]}{hex[0]}{hex[1]}{hex[1]}{hex[2]}{hex[2]}{hex[3]}{hex[3]}";

            // Add alpha if missing
            if (hex.Length == 6)
                hex = "FF" + hex;

            if (hex.Length != 8)
                return Colors.Black;

            try
            {
                byte a = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
                byte r = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
                byte g = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
                byte b = byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber);

                return Color.FromArgb(a, r, g, b);
            }
            catch
            {
                return Colors.Black;
            }
        }

        /// <summary>
        /// Converts a WinUI Color to a hex string "#AARRGGBB".
        /// </summary>
        public static string ConvertToString(Color color)
        {
            return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        #endregion

        #region COLOR ⇄ HSL

        /// <summary>
        /// Converts a Color to HSL (hue, saturation, lightness).
        /// Hue: 0–360°, Saturation/Lightness: 0–1.
        /// </summary>
        public static (double Hue, double Saturation, double Lightness) ToHsl(Color color)
        {
            double r = color.R / 255.0;
            double g = color.G / 255.0;
            double b = color.B / 255.0;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));
            double h = 0, s, l = (max + min) / 2;

            if (Math.Abs(max - min) < 0.0001)
            {
                s = 0;
                h = 0;
            }
            else
            {
                double d = max - min;
                s = l > 0.5 ? d / (2 - max - min) : d / (max + min);

                if (Math.Abs(max - r) < 0.0001)
                    h = (g - b) / d + (g < b ? 6 : 0);
                else if (Math.Abs(max - g) < 0.0001)
                    h = (b - r) / d + 2;
                else
                    h = (r - g) / d + 4;

                h *= 60;
            }

            return (h, s, l);
        }

        /// <summary>
        /// Converts HSL to a Color.
        /// Hue: 0–360°, Saturation/Lightness: 0–1.
        /// </summary>
        public static Color FromHsl(double hue, double saturation, double lightness, byte alpha = 255)
        {
            hue = Math.Clamp(hue, 0, 360);
            saturation = Math.Clamp(saturation, 0, 1);
            lightness = Math.Clamp(lightness, 0, 1);

            double c = (1 - Math.Abs(2 * lightness - 1)) * saturation;
            double x = c * (1 - Math.Abs((hue / 60) % 2 - 1));
            double m = lightness - c / 2;

            double r = 0, g = 0, b = 0;

            if (hue < 60) (r, g, b) = (c, x, 0);
            else if (hue < 120) (r, g, b) = (x, c, 0);
            else if (hue < 180) (r, g, b) = (0, c, x);
            else if (hue < 240) (r, g, b) = (0, x, c);
            else if (hue < 300) (r, g, b) = (x, 0, c);
            else (r, g, b) = (c, 0, x);

            return Color.FromArgb(alpha,
                (byte)((r + m) * 255),
                (byte)((g + m) * 255),
                (byte)((b + m) * 255));
        }

        #endregion

        #region COLOR ADJUSTMENTS

        /// <summary>
        /// Lightens a color by the given factor (0–1).
        /// </summary>
        public static Color Lighten(Color color, double amount)
        {
            var (h, s, l) = ToHsl(color);
            l = Math.Min(1, l + amount);
            return FromHsl(h, s, l, color.A);
        }

        /// <summary>
        /// Darkens a color by the given factor (0–1).
        /// </summary>
        public static Color Darken(Color color, double amount)
        {
            var (h, s, l) = ToHsl(color);
            l = Math.Max(0, l - amount);
            return FromHsl(h, s, l, color.A);
        }

        /// <summary>
        /// Adjusts color saturation by the given factor (-1 to +1).
        /// </summary>
        public static Color AdjustSaturation(Color color, double amount)
        {
            var (h, s, l) = ToHsl(color);
            s = Math.Clamp(s + amount, 0, 1);
            return FromHsl(h, s, l, color.A);
        }

        #endregion
    }
}