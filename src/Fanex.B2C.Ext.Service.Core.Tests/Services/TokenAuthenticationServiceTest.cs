using Fanex.B2C.Ext.Service.Core.Models;
using Fanex.B2C.Ext.Service.Core.Services;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Fanex.B2C.Ext.Service.Core.Tests.Services
{
    public class TokenAuthenticationServiceTest
    {
        private readonly IAuthenticateService _authenticateService;
        private readonly IUserManagementService _mockUserManagement;
        private readonly TokenManagement _tokenManagement = new TokenManagement
        {
            AccessExpiration = 30,
            Audience = "test",
            Issuer = "test",
            Secret = "04241d01a776a7e5095b7dffbff6518f33c9e325c6f6cd72b2d1c59b94755d16",
            RefreshExpiration = 30
        };

        public TokenAuthenticationServiceTest()
        {
            _mockUserManagement = Substitute.For<IUserManagementService>();
            var mockTokenManagement = Substitute.For<IOptions<TokenManagement>>();
            mockTokenManagement.Value.Returns(_tokenManagement);

            _authenticateService = new TokenAuthenticationService(
                _mockUserManagement,
               mockTokenManagement);
        }

        [Theory]
        [InlineData("asx", "1234aa")]
        [InlineData("12bet@asx", "1234aa")]
        public void IsAuthenticated_InValidUser_ReturnFalse(string userName, string pass)
        {
            //Arrange
            var token = new TokenRequest
            {
                Username = userName,
                Password = pass
            };
            _mockUserManagement.IsValidUser(token.Username, token.Password).Returns(false);

            //Action
            var result = _authenticateService.IsAuthenticated(token);

            //Assert
            Assert.False(result.Item1);
        }

        [Theory]
        [InlineData("asx", "1234aa")]
        [InlineData("12bet@asx", "1234aa")]
        public void IsAuthenticated_ValidUser_ReturnTrueAndNotNullToken(string userName, string pass)
        {
            //Arrange
            var token = new TokenRequest
            {
                Username = userName,
                Password = pass
            };
            _mockUserManagement.IsValidUser(Arg.Any<string>(), token.Password).Returns(true);

            //Action
            var result = _authenticateService.IsAuthenticated(token);

            //Assert
            Assert.True(result.Item1);
            Assert.NotNull(result.Item2);
            Assert.True(result.Item2.ExpiresIn == 30);
            Assert.False(string.IsNullOrEmpty(result.Item2.TokenType));
        }
    }
}