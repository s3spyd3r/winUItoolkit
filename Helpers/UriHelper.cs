using System;
using System.Collections.Generic;

namespace winUItoolkit.Helpers
{
    public static class UriHelper
    {
        public static bool IsValidHttpUri(string? uri)
        {
            if (string.IsNullOrWhiteSpace(uri)) return false;
            return Uri.TryCreate(uri, UriKind.Absolute, out var result) && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
        }

        public static IDictionary<string, string> ParseQuery(string uri)
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (!Uri.TryCreate(uri, UriKind.Absolute, out var u)) return dict;
            var q = u.Query;
            if (string.IsNullOrEmpty(q)) return dict;
            if (q.StartsWith("?")) q = q.Substring(1);
            foreach (var part in q.Split('&', StringSplitOptions.RemoveEmptyEntries))
            {
                var kv = part.Split('=', 2);
                var key = Uri.UnescapeDataString(kv[0]);
                var val = kv.Length > 1 ? Uri.UnescapeDataString(kv[1]) : string.Empty;
                dict[key] = val;
            }
            return dict;
        }
    }
}
