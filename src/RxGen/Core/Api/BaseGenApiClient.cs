using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RxGen.Core.Exceptions;

namespace RxGen.Core.Api
{
    public abstract class BaseGenApiClient
    {
        private readonly HttpClient _client;

        public BaseGenApiClient(string apiUrl, HttpMessageHandler messageHandler = null)
        {
            _client = CreateClient(apiUrl, messageHandler);
            _client.Timeout = TimeSpan.FromHours(1);
        }

        private static HttpClient CreateClient(string baseUrl, HttpMessageHandler messageHandler = null)
        {
            var client = messageHandler == null
                ? new HttpClient()
                : new HttpClient(messageHandler);
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        protected virtual async Task<T> GetAsync<T>(string url)
        {
            try
            {
                var response = await _client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var stringResponse = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<T>(stringResponse);
                return result;
            }
            catch (Exception ex)
            {
                throw new RxGenApiException($"REAL data api exception from {url}", ex);
            }
        }

        public HttpClient GetClient() => _client;

        public void Dispose() => _client?.Dispose();
    }
}