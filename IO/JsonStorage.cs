using System.Text.Json;
using System.Threading.Tasks;

namespace winUItoolkit.IO
{
    public static class JsonStorage
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            WriteIndented = false,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        public static Task<string> SerializeAsync<T>(T value)
        {
            if (value is null)
                return Task.FromResult(string.Empty);

            string json = JsonSerializer.Serialize(value, Options);
            return Task.FromResult(json);
        }

        public static Task<T?> DeserializeAsync<T>(string json)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(json))
                    return Task.FromResult<T?>(default);

                var result = JsonSerializer.Deserialize<T>(json, Options);
                return Task.FromResult(result);
            }
            catch
            {
                return Task.FromResult<T?>(default);
            }
        }
    }
}
