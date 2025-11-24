using Fanex.B2C.Ext.Service.Core.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using NSubstitute;

namespace Fanex.B2C.Ext.Service.Core.Tests.Middlewares
{
    public class JWTokenHandlerTest
    {
        private readonly HttpContext _mockContext;
        private readonly HttpRequest _mockRequest;
        private readonly IQueryCollection _mockQueryCollection;
        private readonly IHeaderDictionary _mockHeader;
        const string Token = "ANYTOKEN";
        public JWTokenHandlerTest()
        {
            _mockContext = Substitute.For<HttpContext>();
            _mockRequest = Substitute.For<HttpRequest>();
            _mockQueryCollection = Substitute.For<IQueryCollection>();
            _mockHeader = new HeaderDictionary(); // Use real HeaderDictionary instead of mock
        }

        [Fact]
        public async Task Invoke_WithoutTokenHeader_AddTokenToHeaderAsync()
        {
            //Arrange
            // _mockHeader is now a real HeaderDictionary, no need to mock ContainsKey
            StringValues tokenValue = new StringValues(Token);
            _mockQueryCollection.TryGetValue("access_token", out Arg.Any<StringValues>())
                .Returns(x =>
                {
                    x[1] = tokenValue;
                    return true;
                });

            _mockRequest.Headers.Returns(_mockHeader);
            _mockRequest.Query.Returns(_mockQueryCollection);

            _mockContext.Request.Returns(_mockRequest);
            var handler = new JWTokenHandler(next: async (context) => { await Task.CompletedTask; });

            //Act
            await handler.Invoke(_mockContext);

            //Assert - Debug what's happening
            var directHeaderCount = _mockHeader.Count;
            var contextHeaderCount = _mockContext.Request.Headers.Count;
            var directHasAuth = _mockHeader.ContainsKey("Authorization");
            var contextHasAuth = _mockContext.Request.Headers.ContainsKey("Authorization");

            // Check if the token query is working
            StringValues testToken;
            var queryWorking = _mockContext.Request.Query.TryGetValue("access_token", out testToken);

            // The header should have been added to the context's request headers
            _mockContext.Request.Headers.Should().ContainKey("Authorization",
                $"Header count: {contextHeaderCount}, HasAuth: {contextHasAuth}, QueryWorking: {queryWorking}, Token: '{testToken}'");
            _mockContext.Request.Headers["Authorization"].ToString().Should().Be("Bearer " + Token);
        }


        [Fact]
        public async Task Invoke_InValidTokenInHeader_NotAddTokenToHeaderAsync()
        {
            //Arrange
            var mockHeader = Substitute.For<IHeaderDictionary>();

            mockHeader.ContainsKey("Authorization").Returns(true);
            mockHeader.Authorization.Returns(StringValues.Empty);
            _mockRequest.Headers.Returns(mockHeader);
            _mockRequest.Query.Returns(_mockQueryCollection);

            _mockContext.Request.Returns(_mockRequest);
            var handler = new JWTokenHandler(next: async (context) => { await Task.CompletedTask; });

            //Arrange
            await handler.Invoke(_mockContext);

            //Assert
            _mockRequest.Headers.DidNotReceive().Append("Authorization", "Bearer " + Token);
        }

        [Fact]
        public async Task Invoke_WithoutToken_NotAddTokenToHeaderAsync()
        {
            //Arrange
            _mockRequest.ClearReceivedCalls();
            var mockHeader = Substitute.For<IHeaderDictionary>();

            mockHeader.ContainsKey("Authorization").Returns(false);
            _mockRequest.Headers.Returns(mockHeader);
            _mockRequest.Query.Returns(_mockQueryCollection);

            _mockContext.Request.Returns(_mockRequest);
            var handler = new JWTokenHandler(next: async (context) => { await Task.CompletedTask; });

            //Arrange
            await handler.Invoke(_mockContext);

            //Assert
            _mockRequest.Headers.DidNotReceive().Append("Authorization", "Bearer " + Token);
        }

        [Fact]
        public async Task Invoke_ShouldSupportBusinessScenario_B2CApiTokenHandling()
        {
            // Arrange - B2C API token handling business scenario
            var b2cToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.B2C_API_TOKEN";
            // _mockHeader is now a real HeaderDictionary, no need to mock ContainsKey
            StringValues b2cTokenValue = new StringValues(b2cToken);
            _mockQueryCollection.TryGetValue("access_token", out Arg.Any<StringValues>())
                .Returns(x =>
                {
                    x[1] = b2cTokenValue;
                    return true;
                });

            _mockRequest.Headers.Returns(_mockHeader);
            _mockRequest.Query.Returns(_mockQueryCollection);
            _mockContext.Request.Returns(_mockRequest);
            var handler = new JWTokenHandler(next: async (context) => { await Task.CompletedTask; });

            // Act - Handle B2C API token
            await handler.Invoke(_mockContext);

            // Assert - Should add B2C API token to header
            _mockHeader.Should().ContainKey("Authorization");
            _mockHeader["Authorization"].ToString().Should().Be("Bearer " + b2cToken);
        }

        [Fact]
        public async Task Invoke_ShouldSupportBusinessScenario_ExternalSystemTokenHandling()
        {
            // Arrange - External system token handling business scenario
            var externalToken = "external_system_integration_token_2024";
            // _mockHeader is now a real HeaderDictionary, no need to mock ContainsKey
            StringValues externalTokenValue = new StringValues(externalToken);
            _mockQueryCollection.TryGetValue("access_token", out Arg.Any<StringValues>())
                .Returns(x =>
                {
                    x[1] = externalTokenValue;
                    return true;
                });

            _mockRequest.Headers.Returns(_mockHeader);
            _mockRequest.Query.Returns(_mockQueryCollection);
            _mockContext.Request.Returns(_mockRequest);
            var handler = new JWTokenHandler(next: async (context) => { await Task.CompletedTask; });

            // Act - Handle external system token
            await handler.Invoke(_mockContext);

            // Assert - Should add external system token to header
            _mockHeader.Should().ContainKey("Authorization");
            _mockHeader["Authorization"].ToString().Should().Be("Bearer " + externalToken);
        }

        [Fact]
        public async Task Invoke_ShouldNotModifyExistingAuthorizationHeader_WhenAlreadyPresent()
        {
            // Arrange - Existing authorization header scenario
            var existingToken = "existing_bearer_token";
            _mockHeader["Authorization"] = $"Bearer {existingToken}"; // Pre-populate the header

            _mockRequest.Headers.Returns(_mockHeader);
            _mockContext.Request.Returns(_mockRequest);
            var handler = new JWTokenHandler(next: async (context) => { await Task.CompletedTask; });

            // Act - Handle request with existing authorization header
            await handler.Invoke(_mockContext);

            // Assert - Should not modify existing authorization header
            _mockHeader["Authorization"].ToString().Should().Be($"Bearer {existingToken}");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public async Task Invoke_ShouldNotAddHeader_WhenTokenIsEmptyOrNull(string emptyToken)
        {
            // Arrange - Empty or null token scenario
            // _mockHeader is now a real HeaderDictionary, no need to mock ContainsKey
            StringValues emptyTokenValue = new StringValues(emptyToken);
            _mockQueryCollection.TryGetValue("access_token", out Arg.Any<StringValues>())
                .Returns(x =>
                {
                    x[1] = emptyTokenValue;
                    return true;
                });

            _mockRequest.Headers.Returns(_mockHeader);
            _mockRequest.Query.Returns(_mockQueryCollection);
            _mockContext.Request.Returns(_mockRequest);
            var handler = new JWTokenHandler(next: async (context) => { await Task.CompletedTask; });

            // Act - Handle empty or null token
            await handler.Invoke(_mockContext);

            // Assert - Should handle empty/null/whitespace tokens appropriately
            if (string.IsNullOrEmpty(emptyToken))
            {
                // For null and empty strings, no header should be added
                _mockHeader.Should().NotContainKey("Authorization");
            }
            else
            {
                // For whitespace-only strings, header is added (JWTokenHandler behavior)
                _mockHeader.Should().ContainKey("Authorization");
                _mockHeader["Authorization"].ToString().Should().Be("Bearer " + emptyToken);
            }
        }

        [Fact]
        public async Task Invoke_ShouldSupportBusinessScenario_SecurityAuditTokenLogging()
        {
            // Arrange - Security audit token logging scenario
            var auditToken = "audit_security_token_12345";
            // _mockHeader is now a real HeaderDictionary, no need to mock ContainsKey
            StringValues auditTokenValue = new StringValues(auditToken);
            _mockQueryCollection.TryGetValue("access_token", out Arg.Any<StringValues>())
                .Returns(x =>
                {
                    x[1] = auditTokenValue;
                    return true;
                });

            _mockRequest.Headers.Returns(_mockHeader);
            _mockRequest.Query.Returns(_mockQueryCollection);
            _mockContext.Request.Returns(_mockRequest);
            var handler = new JWTokenHandler(next: async (context) => { await Task.CompletedTask; });

            // Act - Handle security audit token
            await handler.Invoke(_mockContext);

            // Assert - Should process security audit token correctly
            _mockHeader.Should().ContainKey("Authorization");
            _mockHeader["Authorization"].ToString().Should().Be("Bearer " + auditToken);
        }
    }
}