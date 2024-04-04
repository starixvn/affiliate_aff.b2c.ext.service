using Fanex.B2C.Ext.Service.Core.Controllers;
using Fanex.B2C.Ext.Service.Core.Models.DTO;
using Fanex.B2C.Ext.Service.Core.Models.MetaModel;
using Fanex.B2C.Ext.Service.Core.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Fanex.B2C.Ext.Service.Core.Tests.Controllers
{
    public class AffInfoControllerTest
    {
        private readonly ILogger<AffInfoController> _logger;
        private readonly IAffililateInfoService _affililateInfoService;

        public AffInfoControllerTest()
        {
            _logger = Substitute.For<ILogger<AffInfoController>>();
            _affililateInfoService = Substitute.For<IAffililateInfoService>();
        }

        [Fact]
        public void GetAffInfo_WhenCall_CallLogInformationAndReturnCorrect()
        {
            //Arrange
            var param = new AffInfoParam { LicenseeCode = "N8B" };
            _affililateInfoService.GetAffInfo(Arg.Any<AffInfoParam>()).Returns(new AffInfo());
            var controller = new AffInfoController(_logger, _affililateInfoService);

            //Action
            var result = controller.GetAffInfo(param);

            //Assert
            Assert.NotNull(result);
            _logger.Received().LogInformation($"Ext B2C - Get aff {param.LicenseeCode} info");
        }

        [Fact]
        public void GetAffDetailInfoList_WhenCall_CallLogInformationAndReturnCorrect()
        {
            //Arrange
            var param = new AffDetailInfoParam { LicenseeCode = "N88" };
            _affililateInfoService.GetAffDetailInfoList(Arg.Any<AffDetailInfoParam>()).Returns([new AffDetailInfo()]);
            var controller = new AffInfoController(_logger, _affililateInfoService);

            //Action
            var result = controller.GetAffDetailInfoList(param);

            //Assert
            Assert.NotNull(result);
            _logger.Received().LogInformation($"Ext B2C - Get aff {param.LicenseeCode} detail info list");
        }
    }
}