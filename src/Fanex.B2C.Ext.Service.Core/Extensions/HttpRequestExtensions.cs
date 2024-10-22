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

        public static string GetForwardedIP(this HttpRequest request)
        {
            string forwardedIP = null;

            if (request.Headers.TryGetValue(HeaderKey.X_FORWARDED_FOR, out var value))
            {
                forwardedIP = value[0];
            }

            return forwardedIP ?? request.HttpContext.Connection.RemoteIpAddress.ToString();
        }
    }
}
