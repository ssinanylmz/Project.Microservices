using Firebase.Auth;
using Project.Auth.Application.Abstracs;

namespace Project.Auth.Application.Concretes
{
    public class FirebaseAuthService : IAuthService
    {
        private readonly FirebaseAuthClient _firebaseAuthClient;

        public FirebaseAuthService(FirebaseAuthClient firebaseAuthClient)
        {
            _firebaseAuthClient = firebaseAuthClient;
        }

        public async Task<string?> CreateUserWithEmailAndPasswordAsync(string email, string password, string displayName = null!)
        {
            var userCredentials = await _firebaseAuthClient.CreateUserWithEmailAndPasswordAsync(email, password, displayName);
            return userCredentials is null ? null : await userCredentials.User.GetIdTokenAsync();
        }

        public async Task<string?> SignInWithEmailAndPasswordAsync(string email, string password)
        {
            var userCredentials = await _firebaseAuthClient.SignInWithEmailAndPasswordAsync(email, password);
            return userCredentials is null ? null : await userCredentials.User.GetIdTokenAsync();
        }

        public async Task<string?> SignInAnonymouslyAsync()
        {
            var userCredentials = await _firebaseAuthClient.SignInAnonymouslyAsync();
            return userCredentials is null ? null : await userCredentials.User.GetIdTokenAsync();
        }
    }
}
