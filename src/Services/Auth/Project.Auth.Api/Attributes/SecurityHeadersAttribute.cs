using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Project.Auth.Api.Attributes
{
    /// <summary>
    /// Represents an attribute that is used to apply security-related HTTP headers to a response.
    /// The <see cref="SecurityHeadersAttribute"/> is applied globally to add common security headers
    /// across all responses or on specific actions or controllers to cater to particular requirements.
    /// </summary>
    public sealed class SecurityHeadersAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Adds security-related headers to the HTTP response before it is sent to the client.
        /// This method looks for security headers configuration and applies them to the response.
        /// It is executed before the action result is executed, thus ensuring that all responses
        /// carry the specified headers.
        /// </summary>
        /// <param name="context">The <see cref="ResultExecutingContext"/> that provides the context in which the result is executed.</param>
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            var result = context.Result;
            if (result is ViewResult)
            {
                // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Content-Type-Options
                if (!context.HttpContext.Response.Headers.ContainsKey("X-Content-Type-Options"))
                {
                    context.HttpContext.Response.Headers.Append("X-Content-Type-Options", "nosniff");
                }

                // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Frame-Options
                if (!context.HttpContext.Response.Headers.ContainsKey("X-Frame-Options"))
                {
                    context.HttpContext.Response.Headers.Append("X-Frame-Options", "SAMEORIGIN");
                }

                // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy
                var csp = "default-src 'self'; object-src 'none'; frame-ancestors 'none'; sandbox allow-forms allow-same-origin allow-scripts; base-uri 'self';";

                // once for standards compliant browsers
                if (!context.HttpContext.Response.Headers.ContainsKey("Content-Security-Policy"))
                {
                    context.HttpContext.Response.Headers.Append("Content-Security-Policy", csp);
                }
                // and once again for IE
                if (!context.HttpContext.Response.Headers.ContainsKey("X-Content-Security-Policy"))
                {
                    context.HttpContext.Response.Headers.Append("X-Content-Security-Policy", csp);
                }

                // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Referrer-Policy
                var referrer_policy = "no-referrer";
                if (!context.HttpContext.Response.Headers.ContainsKey("Referrer-Policy"))
                {
                    context.HttpContext.Response.Headers.Append("Referrer-Policy", referrer_policy);
                }
            }
        }
    }
}
