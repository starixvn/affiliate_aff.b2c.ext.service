using Fanex.B2C.Ext.Service.Core.Attributes;
using Fanex.B2C.Ext.Service.Core.Models;
using Fanex.B2C.Ext.Service.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.B2C.Ext.Service.Core.Controllers
{
    [ApiController]
    [ServiceFilter(typeof(IPFilterAttribute))]
    public class AuthController(IAuthenticateService authService) : ControllerBase
    {
        [AllowAnonymous]
        [Route("api/Auth")]
        [HttpPost]
        public ActionResult RequestToken([FromBody] TokenRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Request");
            }
            var result = authService.IsAuthenticated(request);

            return result.Item1 ? Ok(result.Item2)
                : BadRequest("Wrong username or password");
        }
    }
}