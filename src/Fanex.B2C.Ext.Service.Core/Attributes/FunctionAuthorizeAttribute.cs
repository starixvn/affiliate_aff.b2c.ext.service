using Fanex.B2C.Ext.Service.Core.Models.Criteria;
using Fanex.B2C.Ext.Service.Core.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace Fanex.B2C.Ext.Service.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class FunctionAuthorizeAttribute(IOptions<List<User>> userListAccessor, string functionName) : ActionFilterAttribute
    {
        public string FunctionName { get; } = functionName;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var username = context.HttpContext.User.Identity.Name;
            var userInfo = userListAccessor.Value.Find(x => x.UserName.Equals(username));

            if (userInfo is null)
            {
                context.Result = new UnauthorizedObjectResult("User not found!");
                return;
            }

            // Function authorization check
            if (userInfo.SupportedFunctions is null)
            {
                context.Result = new UnauthorizedObjectResult("You are not authorized to access this function!");
                return;
            }

            var hasRequiredFunction = userInfo.SupportedFunctions.Exists(func => 
                string.Equals(func, FunctionName, StringComparison.OrdinalIgnoreCase));

            if (!hasRequiredFunction)
            {
                context.Result = new UnauthorizedObjectResult("Access denied. You are not authorized to access this function!");
                return;
            }

            // Licensee validation check (only if criteria with licensee code is present)
            var criteriaArgument = context.ActionArguments.FirstOrDefault(x => x.Key.Contains("criteria"));
            if (criteriaArgument.Value is ILicenseeCode criteria)
            {
                if (userInfo.SupportedLicensees is null)
                {
                    context.Result = new UnauthorizedObjectResult("You are not authorized on this licensee!");
                    return;
                }

                var isValidLicensee = userInfo.SupportedLicensees.Exists(licen => 
                    string.Equals(criteria.LicenseeCode, licen, StringComparison.OrdinalIgnoreCase));

                if (!isValidLicensee)
                {
                    context.Result = new UnauthorizedObjectResult("The licensee is either invalid or not authorized to use this function!");
                    return;
                }
            }
        }
    }
} 