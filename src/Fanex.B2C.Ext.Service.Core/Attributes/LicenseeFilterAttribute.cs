using Fanex.B2C.Ext.Service.Core.Constant;
using Fanex.B2C.Ext.Service.Core.Models.MetaModel;
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
                .SingleOrDefault(x => x.Key.Contains("param")).Value is LicenseeParam param
                && Licensee.LicenseeCodes.Contains(param.LicenseeCode.ToUpperInvariant());

            if (!isValidLicencee)
            {
                context.Result = new BadRequestObjectResult("The Licensee is invalid or unsupport!");
            }
        }
    }
}
