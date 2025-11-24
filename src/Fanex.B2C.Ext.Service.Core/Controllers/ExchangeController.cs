using Fanex.B2C.Ext.Service.Core.Attributes;
using Fanex.B2C.Ext.Service.Core.Models.Param;
using Fanex.B2C.Ext.Service.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Fanex.B2C.Ext.Service.Core.Controllers
{
    [Authorize(Policy = "Exchange")]
    [ApiController]
    [ServiceFilter(typeof(IPFilterAttribute))]
    public class ExchangeController(
        ILogger<ExchangeController> logger,
        IExchangeService exchangeService) : ControllerBase
    {
        [HttpPost]
        [Route("api/DailyExchangeRate")]
        public async Task<IActionResult> Post([FromBody] DailyExchangeRateParam param)
        {
            if (ModelState.IsValid)
            {
                logger.LogInformation("Ext B2C - Get Daily Exchange Rate");

                param.ExchangeIds = WebUtility.HtmlEncode(param.ExchangeIds.Trim());

                return Ok(await exchangeService.GetDailyExchangeRateAsync(param));
            }

            return BadRequest(ModelState);
        }

        [HttpPost]
        [Route("api/DailyExchangeBidRate")]
        public async Task<IActionResult> PostBidRate([FromBody] DailyExchangeRateParam param)
        {
            if (ModelState.IsValid)
            {
                logger.LogInformation("Ext B2C - Get Daily Exchange Bid Rate");

                param.ExchangeIds = WebUtility.HtmlEncode(param.ExchangeIds.Trim());

                return Ok(await exchangeService.GetDailyExchangeBidRateAsync(param));
            }

            return BadRequest(ModelState);
        }
    }
}
