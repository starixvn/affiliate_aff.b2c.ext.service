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
            _affililateInfoService.GetAffInfo(Arg.Any<AffInfoParam>()).Returns(new AffInfo());
            var controller = new AffInfoController(_logger, _affililateInfoService);

            //Action
            var result = controller.GetAffInfo(new AffInfoParam{ LicenseeCode = "N8C" });

            //Assert
            Assert.NotNull(result);
            _logger.Received().LogInformation("Ext B2C - Get aff info");
        }

                [Fact]
        public void GetAffDetailInfoList_WhenCall_CallLogInformationAndReturnCorrect()
        {
            //Arrange
            _affililateInfoService.GetAffDetailInfoList(Arg.Any<AffDetailInfoParam>()).Returns(new List<AffDetailInfo>{new AffDetailInfo()});
            var controller = new AffInfoController(_logger, _affililateInfoService);

            //Action
            var result = controller.GetAffDetailInfoList(new AffDetailInfoParam{ LicenseeCode = "N88" });

            //Assert
            Assert.Single(result);
            _logger.Received().LogInformation("Ext B2C - Get aff detail info list");
        }
    }
}