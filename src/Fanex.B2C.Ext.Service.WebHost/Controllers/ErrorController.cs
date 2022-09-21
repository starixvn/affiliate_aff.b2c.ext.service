using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.ExtService.Controllers
{
    public class ErrorController : ControllerBase
    {
        private const int UnauthorizedCode = 401;
        
        public IResult Index(int code)
        {
            return code == UnauthorizedCode ? Results.Unauthorized() : Results.NotFound();
        }
    }
}