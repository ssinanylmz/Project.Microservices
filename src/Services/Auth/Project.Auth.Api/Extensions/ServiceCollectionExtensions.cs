using Firebase.Auth;
using Firebase.Auth.Providers;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Project.Auth.Application.Abstracs;
using Project.Auth.Application.Concretes;
using Project.Helpers.Options;
using Project.Shared.Helper;

namespace Project.Auth.Api.Extensions
{    /// <summary>
     /// IServiceCollection'ı genişletmek için kullanılan uzantı metodları içerir.
     /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds health check services to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
        /// <param name="configuration">The application's configuration properties, used to configure health checks.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks();

            return services;
        }

        /// <summary>
        /// Registers application-specific services into the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <remarks>
        /// This method should be used to add all custom services, repositories, and handlers to the IoC container.
        /// </remarks>
        /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, FirebaseAuthService>();
            return services;
        }
        /// <summary>
        /// Firebase kimlik doğrulaması için gerekli servisleri ekler.
        /// </summary>
        /// <param name="services">IServiceCollection servis konteyneri.</param>
        /// <param name="configuration">Uygulamanın yapılandırma ayarlarını içerir.</param>
        /// <returns>Genişletilmiş IServiceCollection nesnesi.</returns>
        public static IServiceCollection AddFirebaseAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // Firebase yapılandırmasını yükle
            var firebaseOptions = configuration.GetSection("FirebaseOptions").Get<FirebaseOptions>();

            services.AddScoped<FirebaseAuthClient>(provider =>
            {
                // FirebaseAuthClient oluşturma mantığınız burada olacak
                var firebaseAuthClient = new FirebaseAuthClient(new FirebaseAuthConfig
                {
                    ApiKey = firebaseOptions?.ApiKey,
                    AuthDomain = firebaseOptions?.AuthDomain,
                    Providers = new FirebaseAuthProvider[]
                                {
                                    new EmailProvider()
                                }
                });
                return firebaseAuthClient;
            });

            return services;
        }
        /// <summary>
        /// Swagger'ı ve sürüm yönetimini yapılandırmak için gerekli servisleri ekler.
        /// </summary>
        /// <param name="services">IServiceCollection servis konteyneri.</param>
        /// <param name="configuration">Uygulamanın yapılandırma ayarlarını içerir.</param>
        /// <returns>Genişletilmiş IServiceCollection nesnesi.</returns>
        public static IServiceCollection AddSwaggerCore(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddSwaggerVersioning(configuration);
        }

        /// <summary>
        /// Swagger versiyon yönetimini ve güvenlik tanımlarını ekler.
        /// </summary>
        /// <param name="services">IServiceCollection servis konteyneri.</param>
        /// <param name="configuration">Uygulamanın yapılandırma ayarlarını içerir.</param>
        /// <returns>Genişletilmiş IServiceCollection nesnesi.</returns>
        private static IServiceCollection AddSwaggerVersioning(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer",
                    new OpenApiSecurityScheme
                    {
                        Description = @"JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer"
                    });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                c.IncludeXmlComments(xmlPath, true);
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Project.Auth.Api.xml"), true);

                c.SchemaFilter<EnumTypesSchemaFilter>(Path.Combine(AppContext.BaseDirectory, "Project.Auth.Api.xml"));
            });

            services.ConfigureOptions<ConfigureSwaggerOptions>();

            return services;
        }
    }
}
