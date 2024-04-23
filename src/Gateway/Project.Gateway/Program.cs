using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Project.Gateway.DelegateHandlers;

var builder = WebApplication.CreateBuilder(args);

Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
                    .AddJsonFile("appsettings.json", true, true)
                    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
                    .AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json")
                    .AddEnvironmentVariables();

#region Response Compression

builder.Services.AddResponseCompression(options =>
{
    options.Providers.Add<GzipCompressionProvider>();
    options.EnableForHttps = true; // If you want compression for HTTPS requests too
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Fastest;
});

#endregion

builder.Services.AddAuthentication()
    .AddJwtBearer("Firebase", options =>
    {
        // MetadataAddress'� manuel olarak sa�layarak otomatik yap�land�rman�n yerini alabilirsiniz.
        options.MetadataAddress = "https://securetoken.google.com/talkie-lingo-test/.well-known/openid-configuration";
        options.Authority = "https://securetoken.google.com/talkie-lingo-test";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "https://securetoken.google.com/talkie-lingo-test",
            ValidateAudience = true,
            ValidAudience = "talkie-lingo-test",
            ValidateLifetime = true
        };
    });

#region Swagger

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Project API Gateway", Version = "v1" });
});

#endregion

#region Ocelot

builder.Services.AddOcelot();

builder.Services.AddSwaggerForOcelot(builder.Configuration);

#endregion

//builder.Host.UseSerilog((context, config) => config
//     .ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllers();

#region Cors

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("Default",
        b =>
        {
            b.AllowAnyHeader();
            b.AllowAnyMethod();

            if (builder.Environment.EnvironmentName == "Test")
                b.WithOrigins(builder.Configuration["CorsPolicyURL"] ?? "", "http://localhost:3000", "http://127.0.0.1:3000");
            else
                b.WithOrigins(builder.Configuration["CorsPolicyURL"] ?? "");
        });
});

#endregion

builder.Services.AddHealthChecks();

var app = builder.Build();

var logger = app.Logger;

app.UseResponseCompression();

if (int.Parse(builder.Configuration["RequestResponseLog"] ?? "0") == 1)
    app.UseMiddleware<RequestResponseLoggingMiddleware>();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("Default");

app.UseAuthentication();

app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllers();
    _ = endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });
});

app.UsePathBase("/gateway");

if (builder.Environment.EnvironmentName != "Production")
    await app.UseSwaggerForOcelotUI().UseOcelot();
else
    await app.UseOcelot();

app.Run();
