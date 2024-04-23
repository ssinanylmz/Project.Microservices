using Microsoft.AspNetCore.Http;

namespace Project.Helpers.Utils
{
    public static class IPAddress
    {
        public static IPClient GetClientIPAddressDetail(HttpContext context)
        {
            // IP adresini doğrudan bağlantı özelliğinden al
            var ip = context.Connection.RemoteIpAddress?.ToString();

            // X-Forwarded-For başlığını kontrol et
            if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
            {
                ip = forwardedFor.FirstOrDefault(); // Güvenlik amaçlı ilk adresi al
            }

            var client = new IPClient
            {
                IP = ip,
                LocalIP = context.Connection.LocalIpAddress?.ToString() // Yerel IP adresi bağlantıdan alınır
            };

            return client;
        }
    }
}
