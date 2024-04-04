using Fanex.B2C.Ext.Service.Core.Attributes;
using Fanex.B2C.Ext.Service.Core.Models.MetaModel;
using Fanex.B2C.Ext.Service.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Fanex.B2C.Ext.Service.Core.Controllers
{
    [Authorize]
    [ApiController]
    [ServiceFilter(typeof(IPFilterAttribute))]
    [ServiceFilter(typeof(LicenseeFilterAttribute))]
    public class AffInfoController(
        ILogger<AffInfoController> logger,
        IAffililateInfoService affililateInfoService) : ControllerBase
    {
        [HttpPost]
        [Route("api/AffInfo")]
        public ActionResult GetAffInfo([FromBody] AffInfoParam param)
        {
            if (ModelState.IsValid)
            {
                logger.LogInformation($"Ext B2C - Get aff {param.LicenseeCode} info");

                return Ok(affililateInfoService.GetAffInfo(param));
            }

            return BadRequest($"{ModelState} ");
        }

        [HttpPost]
        [Route("api/AffDetailInfoList")]
        public ActionResult GetAffDetailInfoList([FromBody] AffDetailInfoParam param)
        {
            logger.LogInformation($"Ext B2C - Get aff {param.LicenseeCode} detail info list");

            return Ok(affililateInfoService.GetAffDetailInfoList(param));
        }
    }
}
