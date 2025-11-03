using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace winUItoolkit.Http
{
    public static class ServicesHelper
    {
        private static readonly HttpClient HttpClient = new();
        private const int MaxRetries = 3;
        private static readonly TimeSpan RetryBaseDelay = TimeSpan.FromMilliseconds(250);

        /// <summary>
        /// The base address for your API, e.g. "https://api.example.com/v1/"
        /// </summary>
        public static string ServerAddress { get; set; } = string.Empty;

        #region JSON REQUESTS

        /// <summary>
        /// Sends an HTTP request and returns raw JSON string.
        /// </summary>
        public static async Task<string?> HttpRequestAsync(RestRequestTypes requestType, string endpointWithParams, object? postData = null)
        {
            try
            {
                var uri = new Uri($"{ServerAddress}{endpointWithParams}");
                using var requestMessage = new HttpRequestMessage(new HttpMethod(requestType.ToString().ToUpperInvariant()), uri);

                if (postData is not null &&
                    (requestType == RestRequestTypes.Post ||
                     requestType == RestRequestTypes.Put ||
                     requestType == RestRequestTypes.Patch))
                {
                    string json = JsonSerializer.Serialize(postData);
                    requestMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");
                }

                using HttpResponseMessage? response = await SendWithRetriesAsync(requestMessage).ConfigureAwait(false);
                if (response == null)
                    return null;

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ServicesHelper] HttpRequestAsync Error: {ex}");
                return null;
            }
        }

        private static async Task<HttpResponseMessage?> SendWithRetriesAsync(HttpRequestMessage request)
        {
            for (int attempt = 1; attempt <= MaxRetries; attempt++)
            {
                try
                {
                    var response = await HttpClient.SendAsync(request).ConfigureAwait(false);
                    // Retry on server errors (5xx)
                    if ((int)response.StatusCode >= 500 && attempt < MaxRetries)
                    {
                        await Task.Delay(ComputeBackoff(attempt)).ConfigureAwait(false);
                        continue;
                    }

                    return response;
                }
                catch (HttpRequestException) when (attempt < MaxRetries)
                {
                    await Task.Delay(ComputeBackoff(attempt)).ConfigureAwait(false);
                    continue;
                }
                catch (TaskCanceledException) when (attempt < MaxRetries)
                {
                    await Task.Delay(ComputeBackoff(attempt)).ConfigureAwait(false);
                    continue;
                }
            }

            return null;
        }

        private static TimeSpan ComputeBackoff(int attempt)
        {
            return TimeSpan.FromMilliseconds(RetryBaseDelay.TotalMilliseconds * Math.Pow(2, attempt - 1));
        }

        /// <summary>
        /// Sends an HTTP request and deserializes JSON into a model.
        /// </summary>
        public static async Task<T?> HttpRequestAsync<T>(RestRequestTypes requestType, string endpointWithParams, object? postData = null)
        {
            string? json = await HttpRequestAsync(requestType, endpointWithParams, postData);

            if (string.IsNullOrWhiteSpace(json)) return default;

            try
            {
                return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ServicesHelper] JSON Deserialize Error: {ex}");
                return default;
            }
        }

        #endregion

        #region FILE UPLOAD

        /// <summary>
        /// Uploads a file to a remote endpoint using multipart/form-data.
        /// </summary>
        public static async Task<bool> UploadFileAsync(string endpoint, string filePath, string formFieldName = "file", string? contentType = null)
        {
            try
            {
                var uri = new Uri($"{ServerAddress}{endpoint}");

                using var form = new MultipartFormDataContent();
                using var fileStream = File.OpenRead(filePath);

                var streamContent = new StreamContent(fileStream);
                if (!string.IsNullOrEmpty(contentType))
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);

                form.Add(streamContent, formFieldName, Path.GetFileName(filePath));

                using HttpResponseMessage response = await HttpClient.PostAsync(uri, form);
                response.EnsureSuccessStatusCode();

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ServicesHelper] UploadFileAsync Error: {ex}");
                return false;
            }
        }

        #endregion

        #region FILE DOWNLOAD

        /// <summary>
        /// Downloads a file from an endpoint and saves it to the specified path.
        /// </summary>
        public static async Task<bool> DownloadFileAsync(string endpoint, string destinationPath)
        {
            try
            {
                var uri = new Uri($"{ServerAddress}{endpoint}");

                using HttpResponseMessage response = await HttpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                await using var inputStream = await response.Content.ReadAsStreamAsync();
                await using var outputStream = File.Create(destinationPath);
                await inputStream.CopyToAsync(outputStream);

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ServicesHelper] DownloadFileAsync Error: {ex}");
                return false;
            }
        }

        #endregion
    }

    public enum RestRequestTypes
    {
        Get,
        Post,
        Put,
        Delete,
        Patch
    }
}