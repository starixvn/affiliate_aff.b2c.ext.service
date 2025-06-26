using Fanex.B2C.Ext.Service.Core.Controllers;
using Fanex.B2C.Ext.Service.Core.Models.DTO;
using Fanex.B2C.Ext.Service.Core.Models.Criteria;
using Fanex.B2C.Ext.Service.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Fanex.B2C.Ext.Service.Core.Tests.Controllers
{
    public class AffInfoControllerTest
    {
        private readonly ILogger<AffInfoController> _logger;
        private readonly IAffililateInfoService _affililateInfoService;
        private readonly IAuthorizationService _authorizationService;

        public AffInfoControllerTest()
        {
            _logger = Substitute.For<ILogger<AffInfoController>>();
            _affililateInfoService = Substitute.For<IAffililateInfoService>();
            _authorizationService = Substitute.For<IAuthorizationService>();
        }

        [Fact]
        public async Task GetAffInfo_WhenCall_CallLogInformationAndReturnCorrect()
        {
            //Arrange
            var param = new AffInfoCriteria { LicenseeCode = "N8B" };
            _affililateInfoService.GetAffInfoAsync(Arg.Any<AffInfoCriteria>()).Returns(new AffInfo());
            
            // Mock successful authorization
            var authResult = Substitute.For<AuthorizationResult>();
            authResult.Succeeded.Returns(true);
            _authorizationService.AuthorizeAsync(Arg.Any<System.Security.Claims.ClaimsPrincipal>(), 
                Arg.Any<object>(), Arg.Any<IAuthorizationRequirement>()).Returns(authResult);
            
            var controller = new AffInfoController(_logger, _affililateInfoService, _authorizationService);

            //Action
            var result = await controller.GetAffInfoAsync(param);

            //Assert
            Assert.NotNull(result);
            _logger.Received().LogInformation($"Ext B2C - Get aff {param.LicenseeCode} info");
        }

        [Fact]
        public async Task GetAffDetailInfoList_WhenCall_CallLogInformationAndReturnCorrect()
        {
            //Arrange
            var param = new AffDetailInfoCriteria { LicenseeCode = "N88" };
            _affililateInfoService.GetAffDetailInfoListAsync(Arg.Any<AffDetailInfoCriteria>()).Returns([new AffDetailInfo()]);
            
            // Mock successful authorization
            var authResult = Substitute.For<AuthorizationResult>();
            authResult.Succeeded.Returns(true);
            _authorizationService.AuthorizeAsync(Arg.Any<System.Security.Claims.ClaimsPrincipal>(), 
                Arg.Any<object>(), Arg.Any<IAuthorizationRequirement>()).Returns(authResult);
            
            var controller = new AffInfoController(_logger, _affililateInfoService, _authorizationService);

            //Action
            var result = await controller.GetAffDetailInfoListAsync(param);

            //Assert
            Assert.NotNull(result);
            _logger.Received().LogInformation($"Ext B2C - Get aff {param.LicenseeCode} detail info list");
        }
    }
}