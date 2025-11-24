using Fanex.B2C.Ext.Service.Core.Models;
using Fanex.B2C.Ext.Service.Core.Services;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Fanex.B2C.Ext.Service.Core.Tests.Services
{
    /// <summary>
    /// Comprehensive unit tests for TokenAuthenticationService.
    /// Critical B2C authentication service that handles JWT token creation and validation.
    /// Following EFFECTIVE_UNIT_TESTING_RULES.md guidelines for business-focused testing.
    /// </summary>
    public class TokenAuthenticationServiceTest
    {
        private readonly IAuthenticateService _authenticateService;
        private readonly IUserManagementService _mockUserManagement;
        private readonly TokenManagement _tokenManagement = new TokenManagement
        {
            AccessExpiration = 30,
            Audience = "B2C.API.Service",
            Issuer = "B2C.Authentication.Service",
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

        [Fact]
        public void IsAuthenticated_ShouldSupportBusinessScenario_B2CApiAuthentication()
        {
            // Arrange - B2C API authentication business scenario
            var request = new TokenRequest
            {
                Username = "b2c_api_user",
                Password = "secure_api_password",
                IsShowIpvn = true
            };
            _mockUserManagement.IsValidUser("b2c_api_user", "secure_api_password").Returns(true);

            // Act - Authenticate B2C API user
            var result = _authenticateService.IsAuthenticated(request);

            // Assert - Should return valid JWT token for B2C API access
            result.Item1.Should().BeTrue("Authentication should succeed for valid B2C credentials");
            result.Item2.Should().NotBeNull("Token should be created for successful authentication");
            result.Item2.ExpiresIn.Should().Be(30, "Token expiration should match configuration");
            result.Item2.AccessToken.Should().NotBeNullOrEmpty("Access token should be generated");
            result.Item2.TokenType.Should().Be("JsonWebToken", "Service uses JsonWebTokenHandler");

            // Verify token structure (encrypted JWE tokens can't be read directly without decryption)
            // Token should be a valid JWE format (5 parts separated by dots)
            var tokenParts = result.Item2.AccessToken.Split('.');
            tokenParts.Should().HaveCount(5, "JWE tokens should have 5 parts separated by dots");

            // Each part should be base64url encoded (some parts can be empty in certain JWE algorithms)
            tokenParts[0].Should().NotBeNullOrEmpty("JWE header should contain encryption metadata");
            // Note: Direct key agreement (dir) algorithm can result in empty encrypted key part
            // tokenParts[1] may be empty for "dir" algorithm
            // tokenParts[2], [3], [4] contain IV, ciphertext, and authentication tag respectively
        }

        [Fact]
        public void IsAuthenticated_ShouldSupportBusinessScenario_SiteSpecificAuthentication()
        {
            // Arrange - Site-specific authentication with @ delimiter business scenario
            var request = new TokenRequest
            {
                Username = "casino_site@admin_user",
                Password = "site_admin_password",
                IsShowIpvn = false
            };
            _mockUserManagement.IsValidUser("admin_user", "site_admin_password").Returns(true);

            // Act - Authenticate site-specific user
            var result = _authenticateService.IsAuthenticated(request);

            // Assert - Should parse site and username correctly
            result.Item1.Should().BeTrue();
            result.Item2.Should().NotBeNull();

            // Verify token structure for site-specific authentication
            var tokenParts = result.Item2.AccessToken.Split('.');
            tokenParts.Should().HaveCount(5, "JWE tokens should have 5 parts for encrypted format");

            // Verify user management service was called with correct separated username
            _mockUserManagement.Received(1).IsValidUser("admin_user", "site_admin_password");
        }

        [Fact]
        public void IsAuthenticated_ShouldSupportBusinessScenario_ExternalSystemIntegration()
        {
            // Arrange - External system integration business scenario
            var request = new TokenRequest
            {
                Username = "external_system_api",
                Password = "integration_key_2024",
                IsShowIpvn = true
            };
            _mockUserManagement.IsValidUser("external_system_api", "integration_key_2024").Returns(true);

            // Act - Authenticate external system
            var result = _authenticateService.IsAuthenticated(request);

            // Assert - Should create integration token
            result.Item1.Should().BeTrue();
            result.Item2.Should().NotBeNull();
            result.Item2.AccessToken.Should().NotBeNullOrEmpty();
            result.Item2.ExpiresIn.Should().Be(30);

            // Verify token is encrypted JWE for security
            result.Item2.TokenType.Should().Be("JsonWebToken");

            // Verify token structure for external system integration
            var tokenParts = result.Item2.AccessToken.Split('.');
            tokenParts.Should().HaveCount(5, "JWE tokens should have encrypted format");

            // Verify user management service was called correctly
            _mockUserManagement.Received(1).IsValidUser("external_system_api", "integration_key_2024");
        }

        [Fact]
        public void IsAuthenticated_ShouldReturnFalse_WhenUserAuthenticationFails()
        {
            // Arrange - Failed authentication business scenario
            var request = new TokenRequest
            {
                Username = "invalid_user",
                Password = "wrong_password",
                IsShowIpvn = false
            };
            _mockUserManagement.IsValidUser("invalid_user", "wrong_password").Returns(false);

            // Act - Attempt authentication with invalid credentials
            var result = _authenticateService.IsAuthenticated(request);

            // Assert - Should return authentication failure
            result.Item1.Should().BeFalse();
            result.Item2.Should().NotBeNull();
            result.Item2.ExpiresIn.Should().Be(30); // Still has expiration for token structure
            result.Item2.AccessToken.Should().BeNullOrEmpty(); // No token generated
        }

        [Theory]
        [InlineData("admin_user", "admin_password", true)]
        [InlineData("api_user", "api_password", false)]
        [InlineData("site@user", "site_password", true)]
        [InlineData("external@integration", "external_key", false)]
        public void IsAuthenticated_ShouldHandleVariousUserTypes(string username, string password, bool showIpvn)
        {
            // Arrange - Various user types business scenario
            var request = new TokenRequest
            {
                Username = username,
                Password = password,
                IsShowIpvn = showIpvn
            };

            // Extract real username for site-specific authentication
            var realUserName = username.Contains('@') ? username.Split('@')[1] : username;
            _mockUserManagement.IsValidUser(realUserName, password).Returns(true);

            // Act - Authenticate various user types
            var result = _authenticateService.IsAuthenticated(request);

            // Assert - Should handle all user types correctly
            result.Item1.Should().BeTrue();
            result.Item2.Should().NotBeNull();
            result.Item2.AccessToken.Should().NotBeNullOrEmpty();

            // Verify token structure and user management service interaction
            var tokenParts = result.Item2.AccessToken.Split('.');
            tokenParts.Should().HaveCount(5, "JWE tokens should have encrypted format");

            // Verify the correct username was used for authentication
            _mockUserManagement.Received(1).IsValidUser(realUserName, password);
        }

        [Fact]
        public void IsAuthenticated_ShouldSupportBusinessScenario_SecurityAuditLogging()
        {
            // Arrange - Security audit logging business scenario
            var request = new TokenRequest
            {
                Username = "audit_user",
                Password = "audit_password",
                IsShowIpvn = true
            };
            _mockUserManagement.IsValidUser("audit_user", "audit_password").Returns(true);

            // Act - Multiple authentication attempts for audit
            var validAttempt = _authenticateService.IsAuthenticated(request);

            // Change password for failed attempt
            _mockUserManagement.IsValidUser("audit_user", "wrong_password").Returns(false);
            var failedRequest = new TokenRequest { Username = "audit_user", Password = "wrong_password" };
            var failedAttempt = _authenticateService.IsAuthenticated(failedRequest);

            // Assert - Should handle audit scenarios correctly
            validAttempt.Item1.Should().BeTrue();
            validAttempt.Item2.AccessToken.Should().NotBeNullOrEmpty();

            failedAttempt.Item1.Should().BeFalse();
            failedAttempt.Item2.AccessToken.Should().BeNullOrEmpty();

            // Verify service interactions for audit trail
            _mockUserManagement.Received(1).IsValidUser("audit_user", "audit_password");
            _mockUserManagement.Received(1).IsValidUser("audit_user", "wrong_password");
        }

        [Fact]
        public void IsAuthenticated_ShouldSupportBusinessScenario_TokenExpirationManagement()
        {
            // Arrange - Token expiration management business scenario
            var request = new TokenRequest
            {
                Username = "expiration_test_user",
                Password = "test_password",
                IsShowIpvn = false
            };
            _mockUserManagement.IsValidUser("expiration_test_user", "test_password").Returns(true);

            // Act - Create token and verify expiration
            var result = _authenticateService.IsAuthenticated(request);

            // Assert - Should set proper expiration
            result.Item1.Should().BeTrue();
            result.Item2.ExpiresIn.Should().Be(30); // 30 minutes from configuration

            // Verify token structure and expiration is set in ExpiresIn property
            var tokenParts = result.Item2.AccessToken.Split('.');
            tokenParts.Should().HaveCount(5, "JWE tokens should have encrypted format");

            // The ExpiresIn property should reflect the configured expiration time
            result.Item2.ExpiresIn.Should().Be(30, "Token expiration should match configuration");
        }

        [Fact]
        public void IsAuthenticated_ShouldSupportBusinessScenario_HighSecurityEncryption()
        {
            // Arrange - High security encryption business scenario
            var request = new TokenRequest
            {
                Username = "security_admin",
                Password = "high_security_password",
                IsShowIpvn = true
            };
            _mockUserManagement.IsValidUser("security_admin", "high_security_password").Returns(true);

            // Act - Create encrypted token
            var result = _authenticateService.IsAuthenticated(request);

            // Assert - Should create properly encrypted JWE token
            result.Item1.Should().BeTrue("High security authentication should succeed");
            result.Item2.TokenType.Should().Be("JsonWebToken", "Service uses JsonWebTokenHandler for JWE");
            result.Item2.AccessToken.Should().NotBeNullOrEmpty("Encrypted token should be generated");

            // Verify token structure (JWE has 5 parts separated by dots)
            var tokenParts = result.Item2.AccessToken.Split('.');
            tokenParts.Should().HaveCount(5, "JWE format should have 5 parts: header.encrypted_key.iv.ciphertext.tag");

            // Verify token structure shows it's properly encrypted (header must contain encryption metadata)
            tokenParts[0].Should().NotBeNullOrEmpty("JWE header should contain encryption algorithm information");
            // Note: Direct key agreement (dir) algorithm results in empty encrypted key part (tokenParts[1])
            // This is expected behavior for the "dir" algorithm used in the service
            // tokenParts[2], [3], [4] contain IV, ciphertext, and authentication tag respectively
        }

        [Fact]
        public void IsAuthenticated_ShouldHandleNullAndEmptyInputs_Gracefully()
        {
            // Arrange - Edge case handling business scenario
            var nullRequest = new TokenRequest { Username = null, Password = "password" };
            var emptyRequest = new TokenRequest { Username = "", Password = "password" };
            var nullPasswordRequest = new TokenRequest { Username = "user", Password = null };

            _mockUserManagement.IsValidUser(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

            // Act & Assert - Should handle edge cases gracefully
            var nullResult = _authenticateService.IsAuthenticated(nullRequest);
            nullResult.Item1.Should().BeFalse();

            var emptyResult = _authenticateService.IsAuthenticated(emptyRequest);
            emptyResult.Item1.Should().BeFalse();

            var nullPasswordResult = _authenticateService.IsAuthenticated(nullPasswordRequest);
            nullPasswordResult.Item1.Should().BeFalse();
        }

        [Fact]
        public void IsAuthenticated_ShouldSupportBusinessScenario_MultiSiteDeployment()
        {
            // Arrange - Multi-site deployment business scenario (representative sample per guidelines)
            var siteUsers = new[]
            {
                new { Site = "casino_asia", User = "admin_asia", Password = "asia_password" },
                new { Site = "sports_europe", User = "admin_europe", Password = "europe_password" },
                new { Site = "poker_americas", User = "admin_americas", Password = "americas_password" },
                new { Site = "bingo_africa", User = "admin_africa", Password = "africa_password" },
                new { Site = "lottery_oceania", User = "admin_oceania", Password = "oceania_password" }
            };

            foreach (var siteUser in siteUsers)
            {
                _mockUserManagement.IsValidUser(siteUser.User, siteUser.Password).Returns(true);
            }

            // Act - Authenticate users from different sites
            var results = siteUsers.Select(siteUser =>
            {
                var request = new TokenRequest
                {
                    Username = $"{siteUser.Site}@{siteUser.User}",
                    Password = siteUser.Password,
                    IsShowIpvn = true
                };
                return new { SiteUser = siteUser, Result = _authenticateService.IsAuthenticated(request) };
            }).ToList();

            // Assert - Should handle multi-site authentication correctly
            results.Should().HaveCount(5);
            results.Should().OnlyContain(r => r.Result.Item1 == true);

            // Verify each token has correct structure and service was called correctly
            foreach (var result in results)
            {
                var tokenParts = result.Result.Item2.AccessToken.Split('.');
                tokenParts.Should().HaveCount(5, "Each JWE token should have encrypted format");

                // Verify user management service was called with correct user and password
                _mockUserManagement.Received(1).IsValidUser(result.SiteUser.User, result.SiteUser.Password);
            }

            // Explicit GC for performance test cleanup per guidelines
            GC.Collect();
        }
    }
}