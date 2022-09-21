using Fanex.B2C.Ext.Service.Core.Constant;
using Fanex.B2C.Ext.Service.Core.DTO;
using Fanex.B2C.Ext.Service.Core.Models.MetaModel;
using Fanex.B2C.Ext.Service.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Fanex.B2C.Ext.Service.Core.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
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
        public AffInfo Post([FromBody] AffInfoParam param)
        {
            if(!Licensee.LicenseeCodes.Contains(param.LicenseeCode))
            {
                param.LicenseeCode = Licensee.LicenseeDefault;
            }    

            return _affililateInfoService.GetAffInfo(param);
        }
    }
}
