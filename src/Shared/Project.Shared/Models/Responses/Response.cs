using System.Net;
using System.Text.Json.Serialization;

namespace Project.Shared.Models.Responses
{
    public class Response<T>
    {
        [JsonPropertyName("data")]
        public T? Data { get; set; }

        [JsonIgnore]
        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; }

        [JsonIgnore]
        [JsonPropertyName("isSuccessful")]
        public bool IsSuccessful { get; set; } = true;

        [JsonPropertyName("messageList")]
        public List<string>? MessageList { get; set; }

        [JsonPropertyName("errors")]
        public List<string>? Errors { get; set; }
        /// <summary>
        /// 0:Redirect,  1:Modal(Devam Et Butonlu - bir sonraki servis), 2:Modal(Tamam Butonlu - reload), 3:Modal(Header servis çağrılır)
        /// 4:Redirect-Mesaj Gösterilmez, 5: Image göster (Error)
        /// </summary>
        [JsonPropertyName("displayType")]
        public int DisplayType { get; set; } = 0;

        [JsonPropertyName("redirectURL")]
        public string RedirectURL { get; set; } = ""; //Yönlendirme yaparken

        // Static Factory Method
        public static Response<T> Success(T data, int statusCode, string message = "")
        {
            return new Response<T> { Data = data, StatusCode = statusCode, IsSuccessful = true, MessageList = string.IsNullOrEmpty(message) ? new List<string>() : new List<string>() { message } };
        }

        public static Response<T> Success(T data, int statusCode, List<string> messagelist)
        {
            return new Response<T> { Data = data, StatusCode = statusCode, IsSuccessful = true, MessageList = messagelist };
        }

        public static Response<T> Success(int statusCode, List<string>? messagelist = null)
        {
            return new Response<T> { Data = default, StatusCode = statusCode, IsSuccessful = true, MessageList = messagelist ?? new List<string>() };
        }

        public static Response<T> Fail(List<string> errors, int statusCode)
        {
            return new Response<T>
            {
                Errors = errors,
                StatusCode = statusCode,
                IsSuccessful = false
            };
        }

        public static Response<T> Fail(string error, int statusCode)
        {
            return new Response<T> { Errors = new List<string>() { error }, StatusCode = statusCode, IsSuccessful = false };
        }

        /// <summary>
        /// Sadece Redirect ve Modal durumlarında kullanılacaktır.
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="displayType"></param>
        /// <param name="RedirectURL"></param>
        /// <returns></returns>
        public static Response<T> Fail(List<string> errors, int displayType, string RedirectURL)
        {
            return new Response<T>
            {
                Errors = errors,
                StatusCode = (int)HttpStatusCode.NotAcceptable,
                IsSuccessful = false,
                DisplayType = displayType,
                RedirectURL = RedirectURL
            };
        }

        /// <summary>
        /// HttpStatusCode.NotAcceptable'te data dönülmek istenildiğinde kullanılır
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="displayType"></param>
        /// <param name="RedirectURL"></param>
        /// <returns></returns>
        public static Response<T> Fail(T data, List<string> errors, int displayType)
        {
            return new Response<T>
            {
                Data = data,
                Errors = errors,
                StatusCode = (int)HttpStatusCode.NotAcceptable,
                IsSuccessful = false,
                DisplayType = displayType
            };
        }

        /// <summary>
        /// Login işlemleri için yapıldı
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="statusCode"></param>
        /// <param name="displayType"></param>
        /// <returns></returns>
        public static Response<T> Fail(List<string> errors, int statusCode, int displayType)
        {
            return new Response<T>
            {
                Errors = errors,
                StatusCode = statusCode,
                IsSuccessful = false,
                DisplayType = displayType
            };
        }

    }
}
