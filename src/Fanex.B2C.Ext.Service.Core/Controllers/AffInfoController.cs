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
        public async Task<IActionResult> GetAffInfoAsync([FromBody] AffInfoCriteria criteria)
        {
            if (ModelState.IsValid)
            {
                logger.LogInformation($"Ext B2C - Get aff {criteria.LicenseeCode} info");

                return Ok(await affililateInfoService.GetAffInfoAsync(criteria));
            }

            return BadRequest($"{ModelState}");
        }

        [HttpPost]
        [Route("api/AffDetailInfoList")]
        public async Task<IActionResult> GetAffDetailInfoListAsync([FromBody] AffDetailInfoCriteria criteria)
        {
            logger.LogInformation($"Ext B2C - Get aff {criteria.LicenseeCode} detail info list");

            return Ok(await affililateInfoService.GetAffDetailInfoListAsync(criteria));
        }
    }
}
