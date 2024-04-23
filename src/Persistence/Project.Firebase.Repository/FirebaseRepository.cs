using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using Project.Firebase.Repository.Config;

namespace Project.Firebase.Repository
{
    public class FirebaseRepository<T> where T : class, new()
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly FirebaseConfig _firebaseConfig;
        private readonly ILogger<FirebaseRepository<T>> _logger;

        public FirebaseRepository(
            IHttpClientFactory httpClientFactory,
            FirebaseConfig firebaseConfig,
            ILogger<FirebaseRepository<T>> logger)
        {
            _httpClientFactory = httpClientFactory;
            _firebaseConfig = firebaseConfig;
            _logger = logger;
        }

        private HttpClient CreateClient()
        {
            return _httpClientFactory.CreateClient("FirebaseClient");
        }

        private async Task<HttpResponseMessage> SendRequestAsync(Func<HttpClient, Task<HttpResponseMessage>> action)
        {
            using var httpClient = CreateClient();
            try
            {
                var response = await action(httpClient);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Firebase request error: {0} - {1}", response.StatusCode, errorContent);
                    throw new FirebaseException($"Request to Firebase failed with status code {response.StatusCode} and content {errorContent}");
                }
                return response;
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Exception occurred when sending request to Firebase.");
                throw new FirebaseException("An error occurred while sending request to Firebase.", e);
            }
        }

        public async Task<T?> GetAsync(string key)
        {
            var response = await SendRequestAsync(httpClient => httpClient.GetAsync($"{_firebaseConfig.BasePath}{key}.json"));
            string responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseBody);
        }

        public async Task<string> CreateAsync(T entity)
        {
            var content = new StringContent(JsonConvert.SerializeObject(entity), Encoding.UTF8, "application/json");
            var response = await SendRequestAsync(httpClient => httpClient.PostAsync($"{_firebaseConfig.BasePath}.json", content));
            string responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseBody);
            return result?["name"] ?? "";
        }

        public async Task UpdateAsync(string key, T entity)
        {
            var content = new StringContent(JsonConvert.SerializeObject(entity), Encoding.UTF8, "application/json");
            var response = await SendRequestAsync(httpClient => httpClient.PutAsync($"{_firebaseConfig.BasePath}{key}.json", content));
        }

        public async Task DeleteAsync(string key)
        {
            var response = await SendRequestAsync(httpClient => httpClient.DeleteAsync($"{_firebaseConfig.BasePath}{key}.json"));
        }
    }

    public class FirebaseException : Exception
    {
        public FirebaseException(string message) : base(message) { }

        public FirebaseException(string message, Exception inner) : base(message, inner) { }
    }
}