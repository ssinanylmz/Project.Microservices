using System.Text;
using Project.Helpers.Utils;
using Project.Shared.Enums;

namespace Project.Gateway.DelegateHandlers
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
        private readonly RequestDelegate _next;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!IsPreflightRequest(context))
            {
                context.Request.EnableBuffering();

                var builder = new StringBuilder();

                var request = await FormatRequest(context.Request);

                builder.Append("Request: ").AppendLine(request);

                var retClientIp = IPAddress.GetClientIPAddressDetail(context);
                _logger.LogWarning("Ocelot Request: LogType: {LogType} - {Request} - IPAddress: {IPAddress} - UserAgent: {UserAgent}",
                    LogTypeEnum.Ocelot.ToString(), builder.ToString(), retClientIp.IP, context.Request.Headers["User-Agent"].ToString());
            }
            await _next(context);
        }

        private bool IsPreflightRequest(HttpContext context)
       => string.Equals(
           context.Request.Method,
           "options",
           StringComparison.InvariantCultureIgnoreCase
       );

        private async Task<string> FormatRequest(HttpRequest request)
        {
            // Leave the body open so the next middleware can read it.
            using var reader = new StreamReader(
                request.Body,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            // Do some processing with body…

            var formattedRequest = $"{request.Scheme} {request.Host}{request.Path} {request.QueryString} {body}";

            // Reset the request body stream position so the next middleware can read it
            request.Body.Position = 0;

            return formattedRequest;
        }
    }
}
