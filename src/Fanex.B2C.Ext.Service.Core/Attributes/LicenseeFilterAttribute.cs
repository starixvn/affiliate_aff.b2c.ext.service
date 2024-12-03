using Fanex.B2C.Ext.Service.Core.Models.Criteria;
using Fanex.B2C.Ext.Service.Core.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace Fanex.B2C.Ext.Service.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class LicenseeFilterAttribute(IOptions<List<User>> userListAccessor) : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var username = context.HttpContext.User.Identity.Name;
            var userInfo = userListAccessor.Value.Find(x => x.UserName.Equals(username));

            if (userInfo.SupportedLicensees is null)
            {
                context.Result = new UnauthorizedObjectResult("You are not authorized on this function!");
            }
            else
            {
                var isValidLicensee = userInfo != null && context.ActionArguments
                        .FirstOrDefault(x => x.Key.Contains("criteria")).Value is ILicenseeCode criteria
                        && userInfo.SupportedLicensees.Exists(licen => string.Equals(criteria.LicenseeCode, licen, StringComparison.OrdinalIgnoreCase));

                if (!isValidLicensee)
                {
                    context.Result = new BadRequestObjectResult("The Licensee is invalid or unsupport!");
                }
            }
        }
    }
}
