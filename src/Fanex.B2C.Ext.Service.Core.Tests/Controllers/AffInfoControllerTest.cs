using Fanex.B2C.Ext.Service.Core.Controllers;
using Fanex.B2C.Ext.Service.Core.Models.DTO;
using Fanex.B2C.Ext.Service.Core.Models.Criteria;
using Fanex.B2C.Ext.Service.Core.Services;
using Fanex.B2C.Ext.Service.Core.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Security.Claims;

namespace Fanex.B2C.Ext.Service.Core.Tests.Controllers
{
    public class AffInfoControllerTest
    {
        [Fact]
        public async Task GetAffInfo_WhenCall_CallLogInformationAndReturnCorrect()
        {
            //Arrange
            var param = new AffInfoCriteria { LicenseeCode = "N8B" };
            
            // Create a test user with claims
            var testUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.NameIdentifier, "123")
            }, "test"));
            
            // Create fresh substitutes for this test
            var logger = Substitute.For<ILogger<AffInfoController>>();
            var affililateInfoService = Substitute.For<IAffililateInfoService>();
            var authorizationService = Substitute.For<IAuthorizationService>();
            
            affililateInfoService.GetAffInfoAsync(Arg.Any<AffInfoCriteria>()).Returns(new AffInfo());
            
            // Mock successful authorization - match the exact signature the controller uses
            authorizationService.AuthorizeAsync(
                Arg.Any<ClaimsPrincipal>(), 
                Arg.Any<object>(), 
                Arg.Any<LicenseeRequirement>()).Returns(AuthorizationResult.Success());
            
            var controller = new AffInfoController(logger, affililateInfoService, authorizationService);
            
            // Set up controller context with the test user
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = testUser }
            };

            //Action
            var result = await controller.GetAffInfoAsync(param);

            //Assert
            Assert.NotNull(result);
            logger.Received().LogInformation($"Ext B2C - Get aff {param.LicenseeCode} info");
        }

        [Fact]
        public async Task GetAffDetailInfoList_WhenCall_CallLogInformationAndReturnCorrect()
        {
            //Arrange
            var param = new AffDetailInfoCriteria { LicenseeCode = "N88" };
            
            // Create a test user with claims
            var testUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.NameIdentifier, "123")
            }, "test"));
            
            // Create fresh substitutes for this test
            var logger = Substitute.For<ILogger<AffInfoController>>();
            var affililateInfoService = Substitute.For<IAffililateInfoService>();
            var authorizationService = Substitute.For<IAuthorizationService>();
            
            affililateInfoService.GetAffDetailInfoListAsync(Arg.Any<AffDetailInfoCriteria>()).Returns([new AffDetailInfo()]);
            
            // Mock successful authorization - match the exact signature the controller uses
            authorizationService.AuthorizeAsync(
                Arg.Any<ClaimsPrincipal>(), 
                Arg.Any<object>(), 
                Arg.Any<LicenseeRequirement>()).Returns(AuthorizationResult.Success());
            
            var controller = new AffInfoController(logger, affililateInfoService, authorizationService);
            
            // Set up controller context with the test user
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = testUser }
            };

            //Action
            var result = await controller.GetAffDetailInfoListAsync(param);

            //Assert
            Assert.NotNull(result);
            logger.Received().LogInformation($"Ext B2C - Get aff {param.LicenseeCode} detail info list");
        }
    }
}