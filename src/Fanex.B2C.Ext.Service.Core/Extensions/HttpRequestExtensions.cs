using Fanex.B2C.Ext.Service.Core.Constant;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Fanex.B2C.Ext.Service.Core.Extensions
{
    public static class HttpRequestExtensions
    {
        private static readonly string HostName = Dns.GetHostName();

        public static bool FromInternal(this HttpRequest request)
            => HostName.Contains(request.Host.Host, StringComparison.OrdinalIgnoreCase);

        public static string GetClientIP(this HttpRequest request)
        {
            string clientIP = null;

            if (request.Headers.TryGetValue(HeaderKey.X_ACTUAL_IP, out var actualIP))
            {
                clientIP = actualIP[0];
            }

            if (string.IsNullOrWhiteSpace(clientIP) && request.Headers.TryGetValue(HeaderKey.X_FORWARDED_FOR, out var forwardedIP))
            {
                clientIP = forwardedIP[0];
            }

            return clientIP?.Trim() ?? request.HttpContext.Connection.RemoteIpAddress.ToString();
        }
    }
}
