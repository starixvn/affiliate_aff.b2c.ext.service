using Fanex.B2C.Ext.Service.Core.Constant;
using Fanex.B2C.Ext.Service.Core.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Net;
using System.Net.Sockets;

namespace Fanex.B2C.Ext.Service.Core.Middleware
{
    public class WhoAmIHealthCheck(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env) : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            IDictionary<string, object> result = new Dictionary<string, object>
            {
                { "Env", env.EnvironmentName }
            };
            await AddServerIPInfo(result);
            AddExtraServerInfo(result);

            return HealthCheckResult.Healthy("Server Info", new ReadOnlyDictionary<string, object>(result));
        }

        private static async Task AddServerIPInfo(IDictionary<string, object> result)
        {
            var hostName = Dns.GetHostName();
            result.Add("Host Name", hostName);

            var serverIps = await Dns.GetHostAddressesAsync(hostName).ConfigureAwait(false);
            for (var i = 0; i < serverIps.Length; i++)
            {
                if (serverIps[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    result.Add($"IP{i}", serverIps[i].ToString());
                }
            }
        }

        private void AddExtraServerInfo(IDictionary<string, object> result)
        {
            var context = httpContextAccessor.HttpContext;
            result.Add("Local Ip", context.Connection.LocalIpAddress.ToString());
            result.Add("Local Port", context.Connection.LocalPort);
            result.Add("Client Ip", context.Connection.RemoteIpAddress.ToString());
            result.Add("Actual Client Ip", context.Request.GetClientIP());
            result.Add("Server Time", DateTimeOffset.Now.UtcDateTime.ToString(CultureInfo.InvariantCulture));
            result.Add("Last Upadted", File.GetCreationTime(typeof(WhoAmIHealthCheck).Assembly.Location).ToString(CultureInfo.InvariantCulture));
            result.Add("Host", context.Request.Host);
            result.Add("Schema", context.Request.Scheme);
            result.Add("X-Headers", "---------------------------------");

            foreach (var item in context.Request.Headers.Where(i => i.Key.Contains(HeaderKey.X, StringComparison.OrdinalIgnoreCase)))
            {
                result.Add($" + {item.Key}", item.Value.ToString());
            }
        }
    }
}