using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Project.Helpers.Options
{
    public class ConfigureSwaggerOptions : IConfigureNamedOptions<SwaggerGenOptions>
    {
        private readonly IConfiguration _configuration;

        public ConfigureSwaggerOptions(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(SwaggerGenOptions options)
        {
            var apiVersions = GetApiVersionsFromConfiguration();

            foreach (var description in apiVersions)
            {
                options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
            }
        }

        public void Configure(string? name, SwaggerGenOptions options)
        {
            Configure(options);
        }

        private IEnumerable<ApiVersionDescription> GetApiVersionsFromConfiguration()
        {
            var versions = _configuration.GetSection("ApiVersions").Get<List<ApiVersionConfiguration>>() ?? new List<ApiVersionConfiguration>();
            return versions.Select(v => new ApiVersionDescription(v.GroupName, new Version(v.Version), v.IsDeprecated));
        }

        private OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new OpenApiInfo()
            {
                Title = "Project API " + description.GroupName,
                Version = description.ApiVersion.ToString()
            };

            if (description.IsDeprecated)
            {
                info.Description += " This API version has been deprecated.";
            }

            return info;
        }
    }

    public class ApiVersionDescription
    {
        public string GroupName { get; }
        public Version ApiVersion { get; }
        public bool IsDeprecated { get; }

        public ApiVersionDescription(string groupName, Version apiVersion, bool isDeprecated)
        {
            GroupName = groupName;
            ApiVersion = apiVersion;
            IsDeprecated = isDeprecated;
        }
    }

    public class ApiVersionConfiguration
    {
        public required string GroupName { get; set; }
        public required string Version { get; set; }
        public bool IsDeprecated { get; set; }
    }
}