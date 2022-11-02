using Fanex.B2C.Ext.Service.Core.Attributes;
using Fanex.B2C.Ext.Service.Core.Models.MetaModel;
using Fanex.B2C.Ext.Service.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;


namespace Fanex.B2C.Ext.Service.Core.Controllers
{
    [Authorize]
    [ApiController]
    [ServiceFilter(typeof(IPFilterAttribute))]
    public class ExchangeController : ControllerBase
    {
        private readonly ILogger<AffInfoController> _logger;
        private readonly IExchangeService _exchangeService;

        public ExchangeController(
            ILogger<AffInfoController> logger,
            IExchangeService exchangeService)
        {
            _logger = logger;
            _exchangeService = exchangeService;
        }

        [HttpPost]
        [Route("api/DailyExchangeRate")]
        public ActionResult Post([FromBody] DailyExchangeRateParam param)
        {
            if(ModelState.IsValid)
            {
                _logger.LogCritical("Ext B2C - Get Daily Exchange Rate");

                param.ExchangeIds = WebUtility.HtmlEncode(param.ExchangeIds.Trim());

                return Ok(_exchangeService.GetDailyExchangeRate(param));
            }    

            return BadRequest(ModelState);
        }
    }
}
