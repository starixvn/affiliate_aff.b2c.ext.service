using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.ExtService.Controllers
{
    public class ErrorController : ControllerBase
    {
        public IResult Index()
        {
            return Results.NotFound();
        }
    }
}