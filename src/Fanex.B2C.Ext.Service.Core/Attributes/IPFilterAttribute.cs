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
    public sealed class IPFilterAttribute : ActionFilterAttribute
    {
        private readonly ILogger<IPFilterAttribute> _logger;
        private readonly ApplicationOptions _applicationOptions;

        public IPFilterAttribute(
            ILogger<IPFilterAttribute> logger,
            IOptions<ApplicationOptions> applicationOptionAccessor)
        {
            _logger = logger;
            _applicationOptions = applicationOptionAccessor.Value;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.FromInternal())
            {
                return;
            }

            var ipAddress = context.HttpContext.Connection.RemoteIpAddress;
            var whiteListIPList = _applicationOptions.WhiteList;

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

        private bool IsInRangeIP(string ipRangeString, IPAddress ipAddress)
        {
            if (!IPAddressRange.TryParse(ipRangeString, out IPAddressRange ipRange))
            {
                _logger.LogError($"The WhiteList IP: {ipRangeString} was invalid - client IP: ${ipAddress}");

                return false;
            }

            return ipRange.Contains(ipAddress);
        }
    }
}
