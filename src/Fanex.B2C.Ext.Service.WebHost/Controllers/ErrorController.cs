using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.ExtService.Controllers
{
    public class ErrorController : ControllerBase
    {
        private const int UnauthorizedCode = 401;
        
        public ActionResult Index(int code)
        {
            return code == UnauthorizedCode ? Unauthorized("You don't have permission to access") : NotFound("Page not found");
        }
    }
}