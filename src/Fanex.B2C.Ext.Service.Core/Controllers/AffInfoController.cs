using Fanex.B2C.Ext.Service.Core.Attributes;
using Fanex.B2C.Ext.Service.Core.Authorization;
using Fanex.B2C.Ext.Service.Core.Models.Criteria;
using Fanex.B2C.Ext.Service.Core.Services;
using Microsoft.AspNetCore.Authorization;   
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Fanex.B2C.Ext.Service.Core.Controllers
{
    [Authorize(Policy = "AffInfo")]
    [ApiController]
    [ServiceFilter(typeof(IPFilterAttribute))]
    public class AffInfoController(
        ILogger<AffInfoController> logger,
        IAffililateInfoService affililateInfoService,
        IAuthorizationService authorizationService) : ControllerBase
    {
        [HttpPost]
        [Route("api/AffInfo")]
        public async Task<IActionResult> GetAffInfoAsync([FromBody] AffInfoCriteria criteria)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest($"{ModelState}");
            }

            // Check licensee authorization
            var authResult = await authorizationService.AuthorizeAsync(
                User, criteria.LicenseeCode, new LicenseeRequirement(criteria.LicenseeCode));

            if (!authResult.Succeeded)
            {
                return StatusCode(403, "The licensee is either invalid or not authorized to use this function!");
            }

            logger.LogInformation($"Ext B2C - Get aff {criteria.LicenseeCode} info");
            return Ok(await affililateInfoService.GetAffInfoAsync(criteria));
        }

        [HttpPost]
        [Route("api/AffDetailInfoList")]
        public async Task<IActionResult> GetAffDetailInfoListAsync([FromBody] AffDetailInfoCriteria criteria)
        {
            // Check licensee authorization
            var authResult = await authorizationService.AuthorizeAsync(
                User, criteria.LicenseeCode, new LicenseeRequirement(criteria.LicenseeCode));

            if (!authResult.Succeeded)
            {
                return StatusCode(403, "The licensee is either invalid or not authorized to use this function!");
            }

            logger.LogInformation($"Ext B2C - Get aff {criteria.LicenseeCode} detail info list");
            return Ok(await affililateInfoService.GetAffDetailInfoListAsync(criteria));
        }
    }
}
