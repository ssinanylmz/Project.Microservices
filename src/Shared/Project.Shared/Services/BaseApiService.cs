using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Project.Shared.Dtos;
using Project.Shared.Enums;
using Project.Shared.Helper;
using Project.Shared.Models.Responses;
using Project.Shared.Services.IService;

namespace Project.Shared.Services
{
    public class BaseApiService : IBaseApiService
    {
        //public ResponseDto responseModel { get; set; }
        public IHttpClientFactory httpClient { get; set; }
        private readonly ILogger<BaseApiService> _logger;

        public BaseApiService(IHttpClientFactory httpClient, ILogger<BaseApiService> logger)
        {
            //this.responseModel = new ResponseDto();
            this.httpClient = httpClient;
            _logger = logger;
        }

        public async Task<T> SendAsync<T>(ApiRequest apiRequest)
        {
            try
            {
                var client = httpClient.CreateClient("ProjectAPI");
                client.Timeout = TimeSpan.FromSeconds(10);
                HttpRequestMessage message = new HttpRequestMessage();
                message.Headers.Add("Accept", "application/json");
                message.RequestUri = new Uri(apiRequest.Url);
                client.DefaultRequestHeaders.Clear();

                if (apiRequest.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data),
                        Encoding.UTF8, "application/json");
                }

                if (!string.IsNullOrEmpty(apiRequest.AccessToken))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiRequest.AccessToken);
                }

                if (!string.IsNullOrEmpty(apiRequest.ApiKey))
                    message.Headers.Add("X-API-KEY", apiRequest.ApiKey);

                if (!string.IsNullOrEmpty(apiRequest.XDigest))
                {
                    if (client.DefaultRequestHeaders.Any(s => s.Key == "X-Digest"))
                        client.DefaultRequestHeaders.Remove("X-Digest");
                    client.DefaultRequestHeaders.Add("X-Digest", apiRequest.XDigest);
                }

                HttpResponseMessage apiResponse = null;

                switch (apiRequest.ApiType)
                {
                    case SD.ApiType.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case SD.ApiType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    case SD.ApiType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }

                apiResponse = await client.SendAsync(message);

                var apiContent = await apiResponse.Content.ReadAsStringAsync();
                var apiResponseDto = JsonConvert.DeserializeObject<T>(apiContent);

                if (apiResponseDto is null)
                {
                    var dto = new Response<T>
                    {
                        Errors = new List<string> { Convert.ToString(apiResponse.StatusCode) },
                        IsSuccessful = false,
                        StatusCode = (int)apiResponse.StatusCode
                    };
                    var res = JsonConvert.SerializeObject(dto);
                    return JsonConvert.DeserializeObject<T>(res);
                }

                return apiResponseDto;

            }
            catch (Exception e)
            {
                _logger.LogError(e, "BaseApiService Request: {LogType} - {AlertType} - {@Request}", LogTypeEnum.BaseApiException.ToString(), AlertTypeEnum.General, apiRequest);

                var dto = new Response<T>
                {
                    Errors = new List<string> { Convert.ToString(e.Message) },
                    IsSuccessful = false,
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
                var res = JsonConvert.SerializeObject(dto);
                var apiResponseDto = JsonConvert.DeserializeObject<T>(res);

                return apiResponseDto;
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(true);
        }
    }
}
