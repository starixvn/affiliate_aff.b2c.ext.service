using Fanex.B2C.Ext.Service.Core.Extensions;
using System.Security.Cryptography;
using System.Text;

namespace Fanex.B2C.Ext.Service.Core.Tests.Extensions
{
    /// <summary>
    /// Comprehensive unit tests for HashExtensions.
    /// Tests SHA256 hashing scenarios and security business logic.
    /// Following EFFECTIVE_UNIT_TESTING_RULES.md guidelines for business-focused testing.
    /// </summary>
    public class HashExtensionsTests
    {
        [Fact]
        public void Sha256_ShouldReturnConsistentHash_WhenInputIsSame()
        {
            // Arrange - Consistent hashing scenario
            var input = "testPassword123";

            // Act - Hash the same input multiple times
            var hash1 = input.Sha256();
            var hash2 = input.Sha256();

            // Assert - Should return consistent hash for same input
            hash1.Should().NotBeNullOrEmpty();
            hash2.Should().NotBeNullOrEmpty();
            hash1.Should().Be(hash2);
        }

        [Fact]
        public void Sha256_ShouldReturnDifferentHashes_WhenInputsAreDifferent()
        {
            // Arrange - Different inputs scenario
            var input1 = "password123";
            var input2 = "password124";

            // Act - Hash different inputs
            var hash1 = input1.Sha256();
            var hash2 = input2.Sha256();

            // Assert - Should return different hashes for different inputs
            hash1.Should().NotBeNullOrEmpty();
            hash2.Should().NotBeNullOrEmpty();
            hash1.Should().NotBe(hash2);
        }

        [Fact]
        public void Sha256_ShouldReturnEmptyString_WhenInputIsNull()
        {
            // Arrange - Null input scenario
            string input = null;

            // Act - Hash null input
            var hash = input.Sha256();

            // Assert - Should return empty string for null input
            hash.Should().Be(string.Empty);
        }

        [Fact]
        public void Sha256_ShouldReturnEmptyString_WhenInputIsEmpty()
        {
            // Arrange - Empty input scenario
            var input = string.Empty;

            // Act - Hash empty input
            var hash = input.Sha256();

            // Assert - Should return empty string for empty input
            hash.Should().Be(string.Empty);
        }

        [Fact]
        public void Sha256_ShouldReturnEmptyString_WhenInputIsWhitespace()
        {
            // Arrange - Whitespace input scenario
            var input = "   ";

            // Act - Hash whitespace input
            var hash = input.Sha256();

            // Assert - Should return empty string for whitespace input
            hash.Should().Be(string.Empty);
        }

        [Theory]
        [InlineData("admin")]
        [InlineData("user123")]
        [InlineData("P@ssw0rd!")]
        [InlineData("simple")]
        [InlineData("ComplexPassword123!@#")]
        public void Sha256_ShouldReturnValidBase64Hash_ForVariousInputs(string input)
        {
            // Arrange - Various input scenarios

            // Act - Hash various inputs
            var hash = input.Sha256();

            // Assert - Should return valid Base64 encoded hash
            hash.Should().NotBeNullOrEmpty();

            // Verify it's valid Base64
            var act = () => Convert.FromBase64String(hash);
            act.Should().NotThrow();

            // Verify hash length (SHA256 produces 32 bytes, Base64 encoded should be 44 characters)
            hash.Length.Should().Be(44);
        }

        [Fact]
        public void Sha256_ShouldMatchExpectedHash_ForKnownInput()
        {
            // Arrange - Known input with expected hash scenario
            var input = "testPassword";

            // Calculate expected hash manually
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var expectedHashBytes = sha.ComputeHash(bytes);
            var expectedHash = Convert.ToBase64String(expectedHashBytes);

            // Act - Hash known input
            var actualHash = input.Sha256();

            // Assert - Should match expected hash
            actualHash.Should().Be(expectedHash);
        }

        [Fact]
        public void Sha256_ShouldSupportBusinessScenario_PasswordHashing()
        {
            // Arrange - Password hashing business scenario
            var plainTextPassword = "userPassword123";
            var storedHashedPassword = plainTextPassword.Sha256();

            // Act - Verify password by hashing input and comparing
            var inputPassword = "userPassword123";
            var inputHash = inputPassword.Sha256();
            var isPasswordValid = storedHashedPassword == inputHash;

            // Assert - Should support password verification business logic
            isPasswordValid.Should().BeTrue();
            storedHashedPassword.Should().NotBe(plainTextPassword); // Hash should be different from plain text
            storedHashedPassword.Length.Should().Be(44); // Base64 encoded SHA256 length
        }

        [Fact]
        public void Sha256_ShouldSupportBusinessScenario_UserCredentialValidation()
        {
            // Arrange - User credential validation business scenario
            var users = new List<(string Username, string HashedPassword)>
            {
                ("admin", "adminPassword123".Sha256()),
                ("user1", "userPassword456".Sha256()),
                ("guest", "guestPassword789".Sha256())
            };

            var loginAttempt = ("admin", "adminPassword123");

            // Act - Validate user credentials
            var user = users.FirstOrDefault(u => u.Username == loginAttempt.Item1);
            var isValidCredential = user.HashedPassword == loginAttempt.Item2.Sha256();

            // Assert - Should support user credential validation
            isValidCredential.Should().BeTrue();
            user.Username.Should().Be("admin");
            user.HashedPassword.Should().NotBe(loginAttempt.Item2); // Stored hash should not be plain text
        }

        [Fact]
        public void Sha256_ShouldSupportBusinessScenario_SecureTokenGeneration()
        {
            // Arrange - Secure token generation business scenario
            var userInfo = "user123|admin|2024-01-01T00:00:00Z";
            var secretKey = "MySecretKey2024!";
            var tokenData = $"{userInfo}|{secretKey}";

            // Act - Generate secure token hash
            var secureToken = tokenData.Sha256();

            // Assert - Should generate secure token for business use
            secureToken.Should().NotBeNullOrEmpty();
            secureToken.Should().NotContain(userInfo); // Token should not contain original data
            secureToken.Should().NotContain(secretKey); // Token should not contain secret key
            secureToken.Length.Should().Be(44);
        }

        [Fact]
        public void Sha256_ShouldSupportBusinessScenario_DataIntegrityVerification()
        {
            // Arrange - Data integrity verification business scenario
            var originalData = "TransactionId:12345|Amount:1000.00|Currency:USD|Date:2024-01-01";
            var originalHash = originalData.Sha256();

            // Simulate data transmission/storage
            var receivedData = "TransactionId:12345|Amount:1000.00|Currency:USD|Date:2024-01-01";
            var receivedHash = receivedData.Sha256();

            // Act - Verify data integrity
            var isDataIntact = originalHash == receivedHash;

            // Assert - Should verify data integrity correctly
            isDataIntact.Should().BeTrue();
            originalHash.Should().Be(receivedHash);
        }

        [Fact]
        public void Sha256_ShouldSupportBusinessScenario_ApiKeyHashing()
        {
            // Arrange - API key hashing business scenario
            var apiKeys = new Dictionary<string, string>
            {
                ["client1"] = "api_key_client1_2024".Sha256(),
                ["client2"] = "api_key_client2_2024".Sha256(),
                ["client3"] = "api_key_client3_2024".Sha256()
            };

            var incomingRequest = ("client1", "api_key_client1_2024");

            // Act - Validate API key
            var hashedIncomingKey = incomingRequest.Item2.Sha256();
            var isValidApiKey = apiKeys.ContainsKey(incomingRequest.Item1) &&
                               apiKeys[incomingRequest.Item1] == hashedIncomingKey;

            // Assert - Should support API key validation
            isValidApiKey.Should().BeTrue();
            apiKeys["client1"].Should().NotBe(incomingRequest.Item2); // Stored key should be hashed
        }

        [Fact]
        public void Sha256_ShouldSupportBusinessScenario_CaseSensitiveHashing()
        {
            // Arrange - Case sensitive hashing business scenario
            var password1 = "Password123";
            var password2 = "password123";

            // Act - Hash case different passwords
            var hash1 = password1.Sha256();
            var hash2 = password2.Sha256();

            // Assert - Should produce different hashes for different cases
            hash1.Should().NotBe(hash2);
            hash1.Should().NotBeNullOrEmpty();
            hash2.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Sha256_ShouldSupportBusinessScenario_UnicodeCharacterHandling()
        {
            // Arrange - Unicode character handling business scenario
            var unicodePassword = "密码123@#$";
            var asciiPassword = "password123";

            // Act - Hash unicode and ASCII passwords
            var unicodeHash = unicodePassword.Sha256();
            var asciiHash = asciiPassword.Sha256();

            // Assert - Should handle unicode characters correctly
            unicodeHash.Should().NotBeNullOrEmpty();
            asciiHash.Should().NotBeNullOrEmpty();
            unicodeHash.Should().NotBe(asciiHash);
            unicodeHash.Length.Should().Be(44); // Same length as ASCII hash
        }

        [Fact]
        public void Sha256_ShouldSupportBusinessScenario_LongStringHashing()
        {
            // Arrange - Long string hashing business scenario
            var longString = string.Concat(Enumerable.Repeat("LongPasswordData123!@#", 100));

            // Act - Hash long string
            var hash = longString.Sha256();

            // Assert - Should handle long strings correctly
            hash.Should().NotBeNullOrEmpty();
            hash.Length.Should().Be(44); // SHA256 always produces same length hash

            // Verify it's valid Base64
            var act = () => Convert.FromBase64String(hash);
            act.Should().NotThrow();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\t")]
        [InlineData("\n")]
        [InlineData("   \t\n   ")]
        public void Sha256_ShouldReturnEmptyString_ForWhitespaceOnlyInputs(string whitespaceInput)
        {
            // Arrange - Whitespace only inputs scenario

            // Act - Hash whitespace only input
            var hash = whitespaceInput.Sha256();

            // Assert - Should return empty string for whitespace only inputs
            hash.Should().Be(string.Empty);
        }

        [Fact]
        public void Sha256_ShouldSupportBusinessScenario_PerformanceWithMultipleHashes()
        {
            // Arrange - Performance testing scenario
            var inputs = Enumerable.Range(1, 25).Select(i => $"password{i}").ToList();

            // Act - Hash multiple inputs (using representative sample per guidelines)
            var hashes = inputs.Select(input => input.Sha256()).ToList();

            // Assert - Should handle multiple hashing operations efficiently
            hashes.Should().HaveCount(25);
            hashes.Should().OnlyHaveUniqueItems(); // All hashes should be unique
            hashes.Should().OnlyContain(hash => hash.Length == 44); // All should be valid Base64 SHA256

            // Explicit GC for performance test cleanup per guidelines
            GC.Collect();
        }
    }
}
