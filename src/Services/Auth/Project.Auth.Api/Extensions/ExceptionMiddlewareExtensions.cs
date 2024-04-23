using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using System.Net;
using FluentValidation;
using Project.Shared.Enums;
using Serilog;
using Project.Shared.Models.Responses;
using Project.Shared.Dtos;

namespace Project.Auth.Api.Extensions
{    /// <summary>
     /// Hata işleme ara katmanı (middleware) konfigürasyonunu genişleten uzantı metodları sağlar.
     /// </summary>
    public static class ExceptionMiddlewareExtensions
    {
        /// <summary>
        /// Uygulamaya özel hata işleme ara katmanını (middleware) ekler ve yapılandırır.
        /// </summary>
        /// <param name="app">IApplicationBuilder nesnesi, ara katmanları (middlewares) yapılandırmak için kullanılır.</param>
        /// <param name="IsDevelopment">Uygulamanın geliştirme modunda olup olmadığını belirtir.</param>
        /// <remarks>
        /// Bu metod, gelen HTTP isteklerinde oluşan hataları yakalar ve özelleştirilmiş hata yanıtları döner.
        /// Development modundayken hata mesajını gösterir, aksi halde genel bir hata mesajı döner.
        /// </remarks>
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, bool IsDevelopment)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    Response<NoContent> error;

                    var ex = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;
                    context.Response.ContentType = "application/json";

                    if (ex?.GetType() == typeof(NullReferenceException))
                        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    else if (ex?.GetType() == typeof(InvalidOperationException))
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    else if (ex?.GetType() == typeof(ValidationException))
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        error = Response<NoContent>.Fail(((ValidationException)ex)
                            .Errors.Select(x => x.ErrorMessage).ToList(), context.Response.StatusCode);
                        //Log.Error("Validation Errors: {@Errors}", ex);
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(error));
                        return;
                    }
                    else
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                    if (IsDevelopment)
                        error = Response<NoContent>.Fail(ex?.Message ?? "General Error", context.Response.StatusCode);
                    else
                        error = Response<NoContent>.Fail("General Error", context.Response.StatusCode);

                    var stackTrace = ex?.StackTrace?.Replace(System.Environment.NewLine, "");

                    Log.Error("Exception Errors {LogType}: {@Errors}", LogTypeEnum.ExceptionMiddleware.ToString(), ex?.Message + stackTrace);

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(error));
                });
            });
        }
    }
}
