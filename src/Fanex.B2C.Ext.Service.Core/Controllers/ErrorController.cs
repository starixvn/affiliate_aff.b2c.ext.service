using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Fanex.B2C.Ext.Service.Core.Controllers
{
    public class ErrorController : ControllerBase
    {
        public ActionResult Index(int code)
        {
            return code == (int)HttpStatusCode.Unauthorized ? Unauthorized("You don't have permission to access") : NotFound("Page not found");
        }
    }
}