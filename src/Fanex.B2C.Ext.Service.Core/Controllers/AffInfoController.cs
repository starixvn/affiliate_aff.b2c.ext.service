using Fanex.B2C.Ext.Service.Core.Attributes;
using Fanex.B2C.Ext.Service.Core.Constant;
using Fanex.B2C.Ext.Service.Core.Models.DTO;
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
    public class AffInfoController : ControllerBase
    {
        private readonly ILogger<AffInfoController> _logger;
        private readonly IAffililateInfoService _affililateInfoService;

        public AffInfoController(
            ILogger<AffInfoController> logger,
            IAffililateInfoService affililateInfoService)
        {
            _logger = logger;
            _affililateInfoService = affililateInfoService;
        }

        [HttpPost]
        [Route("api/AffInfo")]
        public AffInfo GetAffInfo([FromBody] AffInfoParam param)
        {
            _logger.LogInformation("Ext B2C - Get aff info");

            param.LicenseeCode = GetLicenseeCode(param.LicenseeCode);

            return _affililateInfoService.GetAffInfo(param);
        }

        [HttpPost]
        [Route("api/AffDetailInfoList")]
        public IEnumerable<AffDetailInfo> GetAffDetailInfoList([FromBody] AffDetailInfoParam param)
        {
            _logger.LogInformation("Ext B2C - Get aff detail info list");

            param.LicenseeCode = GetLicenseeCode(param.LicenseeCode);

            return _affililateInfoService.GetAffDetailInfoList(param);
        }

        private static string GetLicenseeCode(string licenseeCode)
        {
            if (!Licensee.LicenseeCodes.Contains(licenseeCode))
            {
                return Licensee.LicenseeDefault;
            }

            return licenseeCode;
        }
    }
}
