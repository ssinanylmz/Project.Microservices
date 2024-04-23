using static Project.Shared.Helper.SD;

namespace Project.Shared.Dtos
{
    public class ApiRequest
    {
        public ApiType ApiType { get; set; } = ApiType.GET;
        public string Url { get; set; }
        public object Data { get; set; }
        public string AccessToken { get; set; }
        public string ApiKey { get; set; }
        public string XDigest { get; set; }

    }
}
