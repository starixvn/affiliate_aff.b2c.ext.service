using Fanex.B2C.Ext.Service.Core.Attributes;
using Fanex.B2C.Ext.Service.Core.Authorization;
using Fanex.B2C.Ext.Service.Core.Constant;
using Fanex.B2C.Ext.Service.Core.Extensions;
using Fanex.B2C.Ext.Service.Core.Models.Criteria;
using Fanex.B2C.Ext.Service.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Fanex.B2C.Ext.Service.Core.Controllers
{
    [Authorize(Policy = "Statistics")]
    [ApiController]
    [ServiceFilter(typeof(IPFilterAttribute))]
    public class StatisticsController(
        ILogger<StatisticsController> logger,
        IStatisticsService statisticsService,
        IAuthorizationService authorizationService) : ControllerBase
    {
        [HttpPost]
        [Route("api/AffMediaChannel")]
        public async Task<IActionResult> GetAffMediaChannelAsync([FromBody] AffMediaChannelCriteria criteria)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest($"{ModelState}");
            }

            // Check licensee authorization
            var authResult = await authorizationService.AuthorizeAsync(
                User, criteria.LicenseeCode, new LicenseeRequirement(criteria.LicenseeCode));

            if (!authResult.Succeeded || !criteria.LicenseeCode.MatchWith(Licensee._12B))
            {
                return StatusCode(403, "The licensee is either invalid or not authorized to use this function!");
            }

            logger.LogInformation(message: $"Ext B2C - Get aff {criteria.LicenseeCode} media channel");
            return Ok(await statisticsService.GetAffMediaChannelAsync(criteria));
        }

        [HttpPost]
        [Route("api/ClickStatistic")]
        public async Task<IActionResult> GetClickStatisticAsync([FromBody] ClickStatisticCriteria criteria)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest($"{ModelState}");
            }

            // Check licensee authorization
            var authResult = await authorizationService.AuthorizeAsync(
                User, criteria.LicenseeCode, new LicenseeRequirement(criteria.LicenseeCode));

            if (!authResult.Succeeded || !criteria.LicenseeCode.MatchWith(Licensee._12B))
            {
                return StatusCode(403, "The licensee is either invalid or not authorized to use this function!");
            }

            logger.LogInformation(message: $"Ext B2C - Get click statistics from {criteria.FromDate} to {criteria.ToDate}");
            return Ok(await statisticsService.GetClickStatisticAsync(criteria));
        }

        #region OH Statistics Endpoints

        [HttpPost]
        [Route("api/OH/Campaign")]
        public Task<IActionResult> GetOHCampaignAsync([FromBody] OHCampaignCriteria criteria)
            => ExecuteOHQueryAsync(criteria, "Campaign", () => statisticsService.GetOHCampaignAsync(criteria));

        [HttpPost]
        [Route("api/OH/Component")]
        public Task<IActionResult> GetOHComponentAsync([FromBody] OHComponentCriteria criteria)
            => ExecuteOHQueryAsync(criteria, "Component", () => statisticsService.GetOHComponentAsync(criteria));

        [HttpPost]
        [Route("api/OH/LinkGenerator")]
        public Task<IActionResult> GetOHLinkGeneratorAsync([FromBody] OHLinkGeneratorCriteria criteria)
            => ExecuteOHQueryAsync(criteria, "LinkGenerator", () => statisticsService.GetOHLinkGeneratorAsync(criteria));

        [HttpPost]
        [Route("api/OH/ClickStatistics")]
        public Task<IActionResult> GetOHClickStatisticsAsync([FromBody] OHClickStatisticsCriteria criteria)
            => ExecuteOHQueryAsync(criteria, "ClickStatistics", () => statisticsService.GetOHClickStatisticsAsync(criteria));

        private async Task<IActionResult> ExecuteOHQueryAsync<T>(OHStatisticsCriteriaBase criteria, string logName, Func<Task<IEnumerable<T>>> query)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest($"{ModelState}");
            }

            logger.LogInformation("Ext B2C - Get OH {LogName} from {FromDate} to {ToDate}", logName, criteria.FromDate, criteria.ToDate);
            return Ok(await query());
        }

        #endregion
    }
}