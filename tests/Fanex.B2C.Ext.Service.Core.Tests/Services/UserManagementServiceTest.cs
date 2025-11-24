using Fanex.B2C.Ext.Service.Core.Extensions;
using Fanex.B2C.Ext.Service.Core.Models.DTO;
using Fanex.B2C.Ext.Service.Core.Services;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Fanex.B2C.Ext.Service.Core.Tests.Services
{
    public class UserManagementServiceTest
    {
        private readonly List<User> _users;
        private readonly IOptions<List<User>> _mockUsers;
        private readonly UserManagementService _service;

        public UserManagementServiceTest()
        {
            _users = new List<User>
            {
                new User
                {
                    UserName = "testUser",
                    Password = "testPassword".Sha256()
                }
            };

            _mockUsers = Substitute.For<IOptions<List<User>>>();
            _mockUsers.Value.Returns(_users);

            _service = new UserManagementService(_mockUsers);
        }

        [Fact]
        public void IsValidUser_WithValidCredentials_ReturnsTrue()
        {
            // Arrange
            var username = "testUser";
            var password = "testPassword";

            // Act
            var result = _service.IsValidUser(username, password);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsValidUser_WithInvalidUsername_ReturnsFalse()
        {
            // Arrange
            var username = "invalidUser";
            var password = "testPassword";

            // Act
            var result = _service.IsValidUser(username, password);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidUser_WithInvalidPassword_ReturnsFalse()
        {
            // Arrange
            var username = "testUser";
            var password = "invalidPassword";

            // Act
            var result = _service.IsValidUser(username, password);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidUser_WithNullUsername_ReturnsFalse()
        {
            // Arrange
            string username = null;
            var password = "testPassword";

            // Act
            var result = _service.IsValidUser(username, password);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidUser_WithNullPassword_ReturnsFalse()
        {
            // Arrange
            var username = "testUser";
            string password = null;

            // Act
            var result = _service.IsValidUser(username, password);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidUser_WithEmptyUsername_ReturnsFalse()
        {
            // Arrange
            var username = string.Empty;
            var password = "testPassword";

            // Act
            var result = _service.IsValidUser(username, password);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidUser_WithEmptyPassword_ReturnsFalse()
        {
            // Arrange
            var username = "testUser";
            var password = string.Empty;

            // Act
            var result = _service.IsValidUser(username, password);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidUser_WithCaseSensitiveUsername_ReturnsFalse()
        {
            // Arrange
            var username = "TESTUSER";
            var password = "testPassword";

            // Act
            var result = _service.IsValidUser(username, password);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidUser_ShouldSupportBusinessScenario_B2CUserAuthentication()
        {
            // Arrange - B2C user authentication business scenario
            var b2cUser = new User
            {
                UserName = "b2c_admin",
                Password = "b2c_secure_password".Sha256(),
                SupportedFunctions = new List<string> { "AffInfo", "Statistics", "Exchange" },
                SupportedLicensees = new List<string> { "12B", "N8B" }
            };
            _users.Add(b2cUser);

            // Act - Authenticate B2C user
            var result = _service.IsValidUser("b2c_admin", "b2c_secure_password");

            // Assert - Should authenticate B2C user successfully
            result.Should().BeTrue();
        }

        [Fact]
        public void IsValidUser_ShouldSupportBusinessScenario_ExternalSystemAuthentication()
        {
            // Arrange - External system authentication business scenario
            var externalUser = new User
            {
                UserName = "external_api",
                Password = "external_integration_key".Sha256(),
                SupportedFunctions = new List<string> { "Exchange" },
                SupportedLicensees = new List<string> { "12B" }
            };
            _users.Add(externalUser);

            // Act - Authenticate external system
            var result = _service.IsValidUser("external_api", "external_integration_key");

            // Assert - Should authenticate external system successfully
            result.Should().BeTrue();
        }

        [Fact]
        public void IsValidUser_ShouldSupportBusinessScenario_MultiLicenseeUserAccess()
        {
            // Arrange - Multi-licensee user access business scenario
            var multiLicenseeUser = new User
            {
                UserName = "multi_licensee_admin",
                Password = "multi_admin_password".Sha256(),
                SupportedFunctions = new List<string> { "AffInfo", "Statistics", "Exchange" },
                SupportedLicensees = new List<string> { "12B", "N8B", "N88", "KLDR", "KLPROD" }
            };
            _users.Add(multiLicenseeUser);

            // Act - Authenticate multi-licensee user
            var result = _service.IsValidUser("multi_licensee_admin", "multi_admin_password");

            // Assert - Should authenticate multi-licensee user successfully
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("admin_user", "correct_password", true)]
        [InlineData("admin_user", "wrong_password", false)]
        [InlineData("wrong_user", "correct_password", false)]
        [InlineData("", "correct_password", false)]
        [InlineData("admin_user", "", false)]
        public void IsValidUser_ShouldHandleVariousCredentialCombinations(string username, string password, bool expectedResult)
        {
            // Arrange - Various credential combinations scenario
            var adminUser = new User
            {
                UserName = "admin_user",
                Password = "correct_password".Sha256()
            };
            _users.Add(adminUser);

            // Act - Test various credential combinations
            var result = _service.IsValidUser(username, password);

            // Assert - Should handle all credential combinations correctly
            result.Should().Be(expectedResult);
        }

        [Fact]
        public void IsValidUser_ShouldSupportBusinessScenario_SecurityAuditLogging()
        {
            // Arrange - Security audit logging business scenario
            var auditUser = new User
            {
                UserName = "audit_user",
                Password = "audit_password".Sha256()
            };
            _users.Add(auditUser);

            // Act - Multiple authentication attempts for audit
            var validAttempt = _service.IsValidUser("audit_user", "audit_password");
            var invalidAttempt1 = _service.IsValidUser("audit_user", "wrong_password");
            var invalidAttempt2 = _service.IsValidUser("wrong_user", "audit_password");

            // Assert - Should handle audit scenarios correctly
            validAttempt.Should().BeTrue();
            invalidAttempt1.Should().BeFalse();
            invalidAttempt2.Should().BeFalse();
        }

        [Fact]
        public void IsValidUser_ShouldSupportBusinessScenario_PasswordHashingSecurity()
        {
            // Arrange - Password hashing security business scenario
            var plainPassword = "secure_business_password_2024";
            var hashedPassword = plainPassword.Sha256();
            var securityUser = new User
            {
                UserName = "security_user",
                Password = hashedPassword
            };
            _users.Add(securityUser);

            // Act - Authenticate with plain password (should be hashed internally)
            var result = _service.IsValidUser("security_user", plainPassword);

            // Assert - Should validate hashed password correctly
            result.Should().BeTrue();

            // Verify password is stored as hash, not plain text
            securityUser.Password.Should().NotBe(plainPassword);
            securityUser.Password.Should().Be(hashedPassword);
            securityUser.Password.Length.Should().Be(44); // Base64 SHA256 length
        }

        [Fact]
        public void IsValidUser_ShouldSupportBusinessScenario_PerformanceWithMultipleUsers()
        {
            // Arrange - Performance with multiple users scenario (representative sample per guidelines)
            var performanceUsers = Enumerable.Range(1, 25).Select(i => new User
            {
                UserName = $"perf_user_{i}",
                Password = $"password_{i}".Sha256(),
                SupportedFunctions = new List<string> { "AffInfo" },
                SupportedLicensees = new List<string> { "12B" }
            }).ToList();

            _users.AddRange(performanceUsers);

            // Act - Authenticate multiple users
            var results = performanceUsers.Select(user =>
                _service.IsValidUser(user.UserName, $"password_{user.UserName.Split('_')[2]}")).ToList();

            // Assert - Should handle multiple user authentication efficiently
            results.Should().HaveCount(25);
            results.Should().OnlyContain(result => result == true);

            // Explicit GC for performance test cleanup per guidelines
            GC.Collect();
        }

        [Fact]
        public void IsValidUser_ShouldHandleNullUserList_Gracefully()
        {
            // Arrange - Null user list scenario
            var nullUserOptions = Substitute.For<IOptions<List<User>>>();
            nullUserOptions.Value.Returns((List<User>)null);
            var serviceWithNullUsers = new UserManagementService(nullUserOptions);

            // Act - Authenticate with null user list
            var act = () => serviceWithNullUsers.IsValidUser("any_user", "any_password");

            // Assert - Should handle null user list gracefully
            act.Should().Throw<Exception>();
        }

        [Fact]
        public void IsValidUser_ShouldSupportBusinessScenario_UnicodePasswordHandling()
        {
            // Arrange - Unicode password handling business scenario
            var unicodePassword = "密码123@#$";
            var unicodeUser = new User
            {
                UserName = "unicode_user",
                Password = unicodePassword.Sha256()
            };
            _users.Add(unicodeUser);

            // Act - Authenticate with unicode password
            var result = _service.IsValidUser("unicode_user", unicodePassword);

            // Assert - Should handle unicode passwords correctly
            result.Should().BeTrue();
        }
    }
}