using Fanex.B2C.Ext.Service.Core.Constant;
using Fanex.B2C.Ext.Service.Core.Models.Criteria;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Fanex.B2C.Ext.Service.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class LicenseeFilterAttribute() : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var isValidLicencee = context.ActionArguments
                .SingleOrDefault(x => x.Key.Contains("criteria")).Value is ILicenseeCode criteria
                && Licensee.LicenseeCodes.Contains(criteria.LicenseeCode.ToUpperInvariant());

            if (!isValidLicencee)
            {
                context.Result = new BadRequestObjectResult("The Licensee is invalid or unsupport!");
            }
        }
    }
}
