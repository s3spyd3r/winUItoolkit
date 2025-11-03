using System;
using System.Text.RegularExpressions;

namespace winUItoolkit.Helpers
{
    /// <summary>
    /// Common validation utilities for WinUI and .NET applications.
    /// </summary>
    public static class Validators
    {
        /// <summary>
        /// Validates an email address.
        /// </summary>
        public static bool IsValidEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            const string pattern = @"^[a-zA-Z0-9._%+\-]+@[a-zA-Z0-9.\-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Validates a phone number (simple international format).
        /// </summary>
        public static bool IsValidPhone(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return false;

            const string pattern = @"^\+?[0-9]{7,15}$";
            return Regex.IsMatch(phone, pattern);
        }

        /// <summary>
        /// Validates a URL (HTTP/HTTPS).
        /// </summary>
        public static bool IsValidUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url)) return false;

            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        /// <summary>
        /// Validates a password based on complexity rules.
        /// Default: minimum 8 chars, at least one uppercase, one lowercase, one digit, one symbol.
        /// </summary>
        public static bool IsValidPassword(string? password)
        {
            if (string.IsNullOrWhiteSpace(password)) return false;

            const string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&_\-])[A-Za-z\d@$!%*?&_\-]{8,}$";
            return Regex.IsMatch(password, pattern);
        }

        /// <summary>
        /// Validates that a string represents a numeric value (integer or decimal).
        /// </summary>
        public static bool IsNumeric(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;

            return double.TryParse(input, out _);
        }

        /// <summary>
        /// Validates a postal code (supports simple global format).
        /// </summary>
        public static bool IsValidPostalCode(string? postalCode)
        {
            if (string.IsNullOrWhiteSpace(postalCode)) return false;

            const string pattern = @"^[A-Za-z0-9\s\-]{3,10}$";
            return Regex.IsMatch(postalCode, pattern);
        }

        /// <summary>
        /// Validates if a string contains only letters (A-Z, a-z).
        /// </summary>
        public static bool IsAlpha(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;

            const string pattern = @"^[A-Za-z]+$";
            return Regex.IsMatch(input, pattern);
        }

        /// <summary>
        /// Validates if a string contains only alphanumeric characters.
        /// </summary>
        public static bool IsAlphaNumeric(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;

            const string pattern = @"^[A-Za-z0-9]+$";
            return Regex.IsMatch(input, pattern);
        }
    }
}