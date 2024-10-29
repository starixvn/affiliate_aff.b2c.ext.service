using Fanex.B2C.Ext.Service.Core.Extensions;
using Fanex.B2C.Ext.Service.Core.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetTools;
using System.Net;

namespace Fanex.B2C.Ext.Service.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public sealed class IPFilterAttribute(
        ILogger<IPFilterAttribute> logger,
        IOptions<ApplicationOptions> applicationOptionAccessor) : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.FromInternal())
            {
                return;
            }

            var ipAddress = context.HttpContext.Request.GetClientIP();
            var whiteListIPList = applicationOptionAccessor.Value.WhiteList;

            var isInWhiteListIPList = !whiteListIPList.Any() || whiteListIPList.Any(_ => IsInRangeIP(_, ipAddress));
            if (!isInWhiteListIPList)
            {
                var result = new ContentResult
                {
                    Content = $"The IP Address: {ipAddress} was blocked"
                };

                context.Result = result;
            }
        }

        private bool IsInRangeIP(string ipRangeString, string ipAddress)
        {
            if (!IPAddressRange.TryParse(ipRangeString, out IPAddressRange ipRange))
            {
                logger.LogError($"The WhiteList IP: {ipRangeString} was invalid - client IP: ${ipAddress}");

                return false;
            }

            return ipRange.Contains(IPAddress.Parse(ipAddress));
        }
    }
}
