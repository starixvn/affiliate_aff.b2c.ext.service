using Fanex.B2C.Ext.Service.Core.Controllers;
using Fanex.B2C.Ext.Service.Core.Models;
using Fanex.B2C.Ext.Service.Core.Services;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Net;

namespace Fanex.B2C.Ext.Service.Core.Tests.Controllers
{
    /// <summary>
    /// Comprehensive unit tests for AuthController.
    /// Tests authentication scenarios, validation, and business logic.
    /// Following EFFECTIVE_UNIT_TESTING_RULES.md guidelines for business-focused testing.
    /// </summary>
    public class AuthControllerTest
    {
        private readonly IAuthenticateService _authenticateService;

        public AuthControllerTest()
        {
            _authenticateService = Substitute.For<IAuthenticateService>();
        }

        [Fact]
        public void RequestToken_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange - Invalid model state scenario
            var controller = new AuthController(_authenticateService);
            controller.ModelState.AddModelError("Username", "Username is required");
            var request = new TokenRequest { Username = "", Password = "password123" };

            // Act - Request token with invalid model
            var result = controller.RequestToken(request) as ObjectResult;

            // Assert - Should return bad request with validation message
            result.Should().NotBeNull();
            result.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            result.Value.Should().Be("Invalid Request");
        }

        [Fact]
        public void RequestToken_ShouldReturnBadRequest_WhenAuthenticationFails()
        {
            // Arrange - Failed authentication scenario
            var request = new TokenRequest { Username = "invalidUser", Password = "wrongPassword" };
            _authenticateService.IsAuthenticated(Arg.Any<TokenRequest>())
                .Returns(Tuple.Create<bool, Token>(false, new Token()));
            var controller = new AuthController(_authenticateService);

            // Act - Request token with invalid credentials
            var result = controller.RequestToken(request) as ObjectResult;

            // Assert - Should return bad request with authentication error message
            result.Should().NotBeNull();
            result.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            result.Value.Should().Be("Wrong username or password");
        }

        [Fact]
        public void RequestToken_ShouldReturnOkWithToken_WhenAuthenticationSucceeds()
        {
            // Arrange - Successful authentication scenario
            var request = new TokenRequest { Username = "validUser", Password = "correctPassword" };
            var expectedToken = new Token
            {
                AccessToken = "valid-jwt-token",
                ExpiresIn = 30
            };
            _authenticateService.IsAuthenticated(Arg.Any<TokenRequest>())
                .Returns(Tuple.Create(true, expectedToken));
            var controller = new AuthController(_authenticateService);

            // Act - Request token with valid credentials
            var result = controller.RequestToken(request) as ObjectResult;

            // Assert - Should return OK with valid token
            result.Should().NotBeNull();
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            result.Value.Should().BeOfType<Token>();
            var returnedToken = result.Value as Token;
            returnedToken.Should().BeEquivalentTo(expectedToken);
        }

        [Theory]
        [InlineData("", "password123")]
        [InlineData("username", "")]
        [InlineData("", "")]
        [InlineData(null, "password123")]
        [InlineData("username", null)]
        public void RequestToken_ShouldReturnBadRequest_WhenRequiredFieldsAreMissing(string username, string password)
        {
            // Arrange - Missing required fields scenario
            var request = new TokenRequest { Username = username, Password = password };
            var controller = new AuthController(_authenticateService);

            // Simulate model validation failure for empty/null fields
            if (string.IsNullOrEmpty(username))
                controller.ModelState.AddModelError("Username", "Username is required");
            if (string.IsNullOrEmpty(password))
                controller.ModelState.AddModelError("Password", "Password is required");

            // Act - Request token with missing fields
            var result = controller.RequestToken(request) as ObjectResult;

            // Assert - Should return bad request for missing required fields
            result.Should().NotBeNull();
            result.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            result.Value.Should().Be("Invalid Request");
        }

        [Fact]
        public void RequestToken_ShouldCallAuthenticationService_WithCorrectRequest()
        {
            // Arrange - Service interaction verification scenario
            var request = new TokenRequest { Username = "testUser", Password = "testPassword" };
            _authenticateService.IsAuthenticated(Arg.Any<TokenRequest>())
                .Returns(Tuple.Create<bool, Token>(true, new Token()));
            var controller = new AuthController(_authenticateService);

            // Act - Request token
            var result = controller.RequestToken(request);

            // Assert - Should call authentication service with correct request
            _authenticateService.Received(1).IsAuthenticated(Arg.Is<TokenRequest>(r =>
                r.Username == request.Username && r.Password == request.Password));
        }

        [Fact]
        public void RequestToken_ShouldSupportBusinessScenario_B2CApiAuthentication()
        {
            // Arrange - B2C API authentication business scenario
            var request = new TokenRequest
            {
                Username = "b2c_api_user",
                Password = "secure_api_password"
            };
            var businessToken = new Token
            {
                AccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
                ExpiresIn = 30,
                TokenType = "Bearer"
            };
            _authenticateService.IsAuthenticated(Arg.Any<TokenRequest>())
                .Returns(Tuple.Create(true, businessToken));
            var controller = new AuthController(_authenticateService);

            // Act - Authenticate B2C API user
            var result = controller.RequestToken(request) as ObjectResult;

            // Assert - Should return valid JWT token for B2C API access
            result.Should().NotBeNull();
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var token = result.Value as Token;
            token.Should().NotBeNull();
            token.AccessToken.Should().NotBeNullOrEmpty();
            token.ExpiresIn.Should().BeGreaterThan(0);
            token.TokenType.Should().Be("Bearer");
        }

        [Fact]
        public void RequestToken_ShouldSupportBusinessScenario_ExternalSystemIntegration()
        {
            // Arrange - External system integration scenario
            var request = new TokenRequest
            {
                Username = "external_system",
                Password = "integration_key_2024"
            };
            var integrationToken = new Token
            {
                AccessToken = "integration-token-12345",
                ExpiresIn = 60 // Longer expiration for external systems (60 minutes)
            };
            _authenticateService.IsAuthenticated(Arg.Any<TokenRequest>())
                .Returns(Tuple.Create(true, integrationToken));
            var controller = new AuthController(_authenticateService);

            // Act - Authenticate external system
            var result = controller.RequestToken(request) as ObjectResult;

            // Assert - Should return integration token with extended expiration
            result.Should().NotBeNull();
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var token = result.Value as Token;
            token.Should().NotBeNull();
            token.AccessToken.Should().StartWith("integration-token");
            token.ExpiresIn.Should().BeGreaterThan(30); // Longer than standard tokens
        }

        [Fact]
        public void RequestToken_ShouldSupportBusinessScenario_SecurityAuditLogging()
        {
            // Arrange - Security audit logging scenario
            var request = new TokenRequest
            {
                Username = "audit_user",
                Password = "wrong_password"
            };
            _authenticateService.IsAuthenticated(Arg.Any<TokenRequest>())
                .Returns(Tuple.Create<bool, Token>(false, new Token()));
            var controller = new AuthController(_authenticateService);

            // Act - Failed authentication attempt
            var result = controller.RequestToken(request) as ObjectResult;

            // Assert - Should handle failed authentication for audit purposes
            result.Should().NotBeNull();
            result.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            result.Value.Should().Be("Wrong username or password");

            // Verify authentication service was called (for audit logging)
            _authenticateService.Received(1).IsAuthenticated(Arg.Any<TokenRequest>());
        }
    }
}