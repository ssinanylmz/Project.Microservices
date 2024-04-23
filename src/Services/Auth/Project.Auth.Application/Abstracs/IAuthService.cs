namespace Project.Auth.Application.Abstracs
{
    public interface IAuthService
    {
        Task<string?> CreateUserWithEmailAndPasswordAsync(string email, string password, string displayName = null!);
        Task<string?> SignInWithEmailAndPasswordAsync(string email, string password);
        Task<string?> SignInAnonymouslyAsync();
    }
}
