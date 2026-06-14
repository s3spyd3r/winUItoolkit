using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace winUItoolkit.IO
{
    public static class JsonStorage
    {
        /// <summary>
        /// Default options used for in-memory and transient serialization. Compact, case-insensitive, ignores nulls.
        /// </summary>
        public static JsonSerializerOptions Options { get; } = new()
        {
            WriteIndented = false,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        /// <summary>
        /// Options used for file-backed JSON (e.g. user settings on disk). Indented for human inspection.
        /// </summary>
        public static JsonSerializerOptions FileOptions { get; } = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public static async Task<string> SerializeAsync<T>(T value)
        {
            if (value is null) return string.Empty;
            await using var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, value, Options).ConfigureAwait(false);
            stream.Position = 0;
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync().ConfigureAwait(false);
        }

        public static async Task<T?> DeserializeAsync<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return default;

            try
            {
                using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
                return await JsonSerializer.DeserializeAsync<T>(stream, Options).ConfigureAwait(false);
            }
            catch
            {
                return default;
            }
        }
    }
}
