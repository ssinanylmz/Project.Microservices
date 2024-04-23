using Project.Shared.Dtos;

namespace Project.Shared.Services.IService
{
    public interface IBaseApiService : IDisposable
    {
        //ResponseDto responseModel { get; set; }
        Task<T> SendAsync<T>(ApiRequest apiRequest);
    }
}
