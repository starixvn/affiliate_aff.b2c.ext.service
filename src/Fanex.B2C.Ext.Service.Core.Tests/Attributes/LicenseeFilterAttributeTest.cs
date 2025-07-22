using Fanex.B2C.Ext.Service.Core.Attributes;
using Fanex.B2C.Ext.Service.Core.Models.DTO;
using Fanex.B2C.Ext.Service.Core.Models.MetaModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.Security.Claims;
using System.Security.Principal;

namespace Fanex.B2C.Ext.Service.Core.Tests.Attributes
{
    public class LicenseeFilterAttributeTest
    {
        private readonly IOptions<List<User>> _mockUserListAccessor;
        private readonly ActionExecutingContext _mockContext;
        private readonly List<User> _users;

        public LicenseeFilterAttributeTest()
        {
            _users = new List<User>
            {
                new User
                {
                    UserName = "testUser",
                    SupportedLicensees = new List<string> { "LICENSEE1", "LICENSEE2" }
                }
            };

            _mockUserListAccessor = Substitute.For<IOptions<List<User>>>();
            _mockUserListAccessor.Value.Returns(_users);

            var mockHttpContext = Substitute.For<HttpContext>();
            var mockUser = Substitute.For<ClaimsPrincipal>();
            var mockIdentity = Substitute.For<IIdentity>();
            mockIdentity.Name.Returns("testUser");
            mockUser.Identity.Returns(mockIdentity);
            mockHttpContext.User.Returns(mockUser);

            _mockContext = new ActionExecutingContext(
                new ActionContext(
                    mockHttpContext,
                    new RouteData(),
                    new ActionDescriptor()
                ),
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                Substitute.For<Controller>()
            );
        }

        [Fact]
        public void OnActionExecuting_WithNullSupportedLicensees_ReturnsUnauthorized()
        {
            // Arrange
            _users[0].SupportedLicensees = null;
            var filter = new LicenseeFilterAttribute(_mockUserListAccessor);

            // Act
            filter.OnActionExecuting(_mockContext);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(_mockContext.Result);
            Assert.Equal("You are not authorized on this function!",
                ((UnauthorizedObjectResult)_mockContext.Result).Value);
        }

        [Fact]
        public void OnActionExecuting_WithValidLicensee_DoesNotSetResult()
        {
            // Arrange
            var criteria = new AffInfoCriteria { LicenseeCode = "LICENSEE1" };
            _mockContext.ActionArguments.Add("criteria", criteria);
            var filter = new LicenseeFilterAttribute(_mockUserListAccessor);

            // Act
            filter.OnActionExecuting(_mockContext);

            // Assert
            Assert.Null(_mockContext.Result);
        }

        [Fact]
        public void OnActionExecuting_WithInvalidLicensee_ReturnsBadRequest()
        {
            // Arrange
            var criteria = new AffInfoCriteria { LicenseeCode = "INVALID" };
            _mockContext.ActionArguments.Add("criteria", criteria);
            var filter = new LicenseeFilterAttribute(_mockUserListAccessor);

            // Act
            filter.OnActionExecuting(_mockContext);

            // Assert
            Assert.IsType<BadRequestObjectResult>(_mockContext.Result);
            Assert.Equal("The Licensee is invalid or unsupport!",
                ((BadRequestObjectResult)_mockContext.Result).Value);
        }

        [Fact]
        public void OnActionExecuting_WithNoCriteria_ReturnsBadRequest()
        {
            // Arrange
            var filter = new LicenseeFilterAttribute(_mockUserListAccessor);

            // Act
            filter.OnActionExecuting(_mockContext);

            // Assert
            Assert.IsType<BadRequestObjectResult>(_mockContext.Result);
            Assert.Equal("The Licensee is invalid or unsupport!",
                ((BadRequestObjectResult)_mockContext.Result).Value);
        }
    }
}