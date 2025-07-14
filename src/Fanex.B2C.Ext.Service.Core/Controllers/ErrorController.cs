using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Fanex.B2C.Ext.Service.Core.Controllers
{
    public class ErrorController : ControllerBase
    {
        public ActionResult Index(int code)
        {
            return code switch
            {
                (int)HttpStatusCode.Unauthorized => Unauthorized("Authentication required. Please provide a valid token."),
                (int)HttpStatusCode.Forbidden => StatusCode(403, "You don't have permission to access this resource."),
                (int)HttpStatusCode.NotFound => NotFound("The requested resource was not found."),
                _ => StatusCode(code, $"An error occurred. Status code: {code}")
            };
        }
    }
}