using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Project.Firebase.Repository.Config;

namespace Project.Firebase.Repository.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFirebaseServices(this IServiceCollection services, IConfiguration configuration)
        {
            FirebaseConfig firebaseConfig = configuration.GetSection("FirebaseConfig").Get<FirebaseConfig>() ?? new FirebaseConfig();
            services.AddSingleton(firebaseConfig);

            services.AddHttpClient("FirebaseClient", httpClient =>
            {
                httpClient.BaseAddress = new System.Uri(firebaseConfig.BasePath ?? "");
            });

            services.AddScoped(typeof(FirebaseRepository<>));

            return services;
        }
    }
}
