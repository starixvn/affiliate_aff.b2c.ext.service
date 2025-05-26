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
    }
}