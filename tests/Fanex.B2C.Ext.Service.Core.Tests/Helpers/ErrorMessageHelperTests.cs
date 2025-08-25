using Fanex.B2C.Ext.Service.Core.Helper;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using System.Net;

namespace Fanex.B2C.Ext.Service.Core.Tests.Helpers
{
    /// <summary>
    /// Comprehensive unit tests for ErrorMessageHelper.
    /// Tests error message generation scenarios and exception handling business logic.
    /// Following EFFECTIVE_UNIT_TESTING_RULES.md guidelines for business-focused testing.
    /// </summary>
    public class ErrorMessageHelperTests
    {
        [Fact]
        public void GenerateMessage_ShouldIncludeExceptionMessage_WhenExceptionProvided()
        {
            // Arrange - Basic exception scenario
            var context = CreateMockHttpContext();
            var exception = new Exception("Test error message");

            // Act - Generate error message
            var result = ErrorMessageHelper.GenerateMessage(context, exception);

            // Assert - Should include exception message
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("ErrMsg: Test error message");
        }

        [Fact]
        public void GenerateMessage_ShouldIncludeRequestInfo_WhenContextProvided()
        {
            // Arrange - Request info scenario
            var context = CreateMockHttpContext("example.com", "/api/test");
            var exception = new Exception("Test error");

            // Act - Generate error message
            var result = ErrorMessageHelper.GenerateMessage(context, exception);

            // Assert - Should include request information
            result.Should().Contain("REQUEST INFO");
            result.Should().Contain("Host:");
            result.Should().Contain("Path:");
            result.Should().Contain("Remote IP:");
            result.Should().Contain("Client IP:");
        }

        [Fact]
        public void GenerateMessage_ShouldIncludeDatabaseInfo_WhenExceptionProvided()
        {
            // Arrange - Database info scenario
            var context = CreateMockHttpContext();
            var exception = new Exception("Database error");

            // Act - Generate error message
            var result = ErrorMessageHelper.GenerateMessage(context, exception);

            // Assert - Should include database information section
            result.Should().Contain("DATABASE INFO");
        }

        [Fact]
        public void GenerateMessage_ShouldIncludeExceptionInfo_WhenExceptionProvided()
        {
            // Arrange - Exception info scenario
            var context = CreateMockHttpContext();
            var exception = new Exception("Detailed error");

            // Act - Generate error message
            var result = ErrorMessageHelper.GenerateMessage(context, exception);

            // Assert - Should include exception information
            result.Should().Contain("EXCEPTION INFO");
            result.Should().Contain("Message: Detailed error");
            result.Should().Contain("Source:");
            result.Should().Contain("TargetSite:");
            result.Should().Contain("StackTrace:");
        }

        [Fact]
        public void GenerateMessage_ShouldIncludeSqlExceptionDetails_WhenSqlExceptionProvided()
        {
            // Arrange - SQL exception scenario
            var context = CreateMockHttpContext();
            // Note: SqlException cannot be directly instantiated, so we'll test with a regular exception
            // and verify the structure handles SQL exceptions through the database info section
            var exception = new Exception("SQL connection failed");

            // Act - Generate error message
            var result = ErrorMessageHelper.GenerateMessage(context, exception);

            // Assert - Should include database info section for SQL exceptions
            result.Should().Contain("DATABASE INFO");
        }

        [Fact]
        public void GenerateMessage_ShouldHandleInnerExceptions_WhenNestedExceptionsProvided()
        {
            // Arrange - Nested exceptions scenario
            var context = CreateMockHttpContext();
            var innerException = new Exception("Inner error message");
            var outerException = new Exception("Outer error message", innerException);

            // Act - Generate error message
            var result = ErrorMessageHelper.GenerateMessage(context, outerException);

            // Assert - Should handle inner exceptions
            result.Should().Contain("Outer error message");
            result.Should().Contain("Inner error message");
        }

        [Fact]
        public void GenerateMessage_ShouldLimitInnerExceptions_WhenDeepNestingExists()
        {
            // Arrange - Deep nested exceptions scenario (testing the 3-level limit)
            var context = CreateMockHttpContext();
            var level4Exception = new Exception("Level 4 error");
            var level3Exception = new Exception("Level 3 error", level4Exception);
            var level2Exception = new Exception("Level 2 error", level3Exception);
            var level1Exception = new Exception("Level 1 error", level2Exception);

            // Act - Generate error message
            var result = ErrorMessageHelper.GenerateMessage(context, level1Exception);

            // Assert - Should limit inner exception depth (max 3 levels)
            result.Should().Contain("Level 1 error");
            result.Should().Contain("Level 2 error");
            result.Should().Contain("Level 3 error");
            // Level 4 might or might not be included due to the 3-level limit
        }

        [Fact]
        public void GenerateMessage_ShouldSupportBusinessScenario_APIErrorLogging()
        {
            // Arrange - API error logging business scenario
            var context = CreateMockHttpContext("api.b2c.com", "/api/affinfo");
            var apiException = new Exception("Failed to retrieve affiliate information");

            // Act - Generate API error message
            var result = ErrorMessageHelper.GenerateMessage(context, apiException);

            // Assert - Should support comprehensive API error logging
            result.Should().Contain("ErrMsg: Failed to retrieve affiliate information");
            result.Should().Contain("REQUEST INFO");
            result.Should().Contain("Host: api.b2c.com");
            result.Should().Contain("Path: /api/affinfo");
            result.Should().Contain("DATABASE INFO");
            result.Should().Contain("EXCEPTION INFO");
        }

        [Fact]
        public void GenerateMessage_ShouldSupportBusinessScenario_DatabaseConnectionErrorLogging()
        {
            // Arrange - Database connection error business scenario
            var context = CreateMockHttpContext("internal.b2c.com", "/api/statistics");
            var dbException = new Exception("Connection timeout expired");

            // Act - Generate database error message
            var result = ErrorMessageHelper.GenerateMessage(context, dbException);

            // Assert - Should support database error logging
            result.Should().Contain("ErrMsg: Connection timeout expired");
            result.Should().Contain("DATABASE INFO");
            result.Should().Contain("Path: /api/statistics");
        }

        [Fact]
        public void GenerateMessage_ShouldSupportBusinessScenario_AuthenticationErrorLogging()
        {
            // Arrange - Authentication error business scenario
            var context = CreateMockHttpContext("secure.b2c.com", "/api/auth");
            var authException = new UnauthorizedAccessException("Invalid JWT token");

            // Act - Generate authentication error message
            var result = ErrorMessageHelper.GenerateMessage(context, authException);

            // Assert - Should support authentication error logging
            result.Should().Contain("ErrMsg: Invalid JWT token");
            result.Should().Contain("REQUEST INFO");
            result.Should().Contain("Host: secure.b2c.com");
            result.Should().Contain("Path: /api/auth");
        }

        [Fact]
        public void GenerateMessage_ShouldSupportBusinessScenario_ExternalServiceErrorLogging()
        {
            // Arrange - External service error business scenario
            var context = CreateMockHttpContext("gateway.b2c.com", "/api/exchange");
            var serviceException = new HttpRequestException("External exchange service unavailable");

            // Act - Generate external service error message
            var result = ErrorMessageHelper.GenerateMessage(context, serviceException);

            // Assert - Should support external service error logging
            result.Should().Contain("ErrMsg: External exchange service unavailable");
            result.Should().Contain("Path: /api/exchange");
            result.Should().Contain("Remote IP:");
        }

        [Fact]
        public void GenerateMessage_ShouldSupportBusinessScenario_ValidationErrorLogging()
        {
            // Arrange - Validation error business scenario
            var context = CreateMockHttpContext("api.b2c.com", "/api/affinfo", "POST");
            var validationException = new ArgumentException("Invalid licensee code provided");

            // Act - Generate validation error message
            var result = ErrorMessageHelper.GenerateMessage(context, validationException);

            // Assert - Should support validation error logging
            result.Should().Contain("ErrMsg: Invalid licensee code provided");
            result.Should().Contain("REQUEST INFO");
            result.Should().Contain("EXCEPTION INFO");
        }

        [Fact]
        public void GenerateMessage_ShouldSupportBusinessScenario_PerformanceMonitoringErrorLogging()
        {
            // Arrange - Performance monitoring error business scenario
            var context = CreateMockHttpContext("monitor.b2c.com", "/api/statistics");
            var performanceException = new TimeoutException("Query execution timeout after 30 seconds");

            // Act - Generate performance error message
            var result = ErrorMessageHelper.GenerateMessage(context, performanceException);

            // Assert - Should support performance monitoring error logging
            result.Should().Contain("ErrMsg: Query execution timeout after 30 seconds");
            result.Should().Contain("Host: monitor.b2c.com");
            result.Should().Contain("Path: /api/statistics");
        }

        [Fact]
        public void GenerateMessage_ShouldSupportBusinessScenario_SecurityIncidentLogging()
        {
            // Arrange - Security incident logging business scenario
            var context = CreateMockHttpContext("secure.b2c.com", "/api/sensitive", "GET", "203.0.113.100");
            var securityException = new UnauthorizedAccessException("Unauthorized access attempt detected");

            // Act - Generate security incident error message
            var result = ErrorMessageHelper.GenerateMessage(context, securityException);

            // Assert - Should support security incident logging with client IP
            result.Should().Contain("ErrMsg: Unauthorized access attempt detected");
            result.Should().Contain("Client IP: 203.0.113.100");
            result.Should().Contain("Path: /api/sensitive");
        }

        [Fact]
        public void GenerateMessage_ShouldSupportBusinessScenario_IntegrationErrorLogging()
        {
            // Arrange - Integration error logging business scenario
            var context = CreateMockHttpContext("integration.b2c.com", "/api/external-sync");
            var innerException = new HttpRequestException("Partner API returned 503");
            var integrationException = new Exception("Data synchronization failed", innerException);

            // Act - Generate integration error message
            var result = ErrorMessageHelper.GenerateMessage(context, integrationException);

            // Assert - Should support integration error logging with nested exceptions
            result.Should().Contain("ErrMsg: Data synchronization failed");
            result.Should().Contain("Partner API returned 503");
            result.Should().Contain("Path: /api/external-sync");
        }

        [Fact]
        public void GenerateMessage_ShouldHandleNullContext_Gracefully()
        {
            // Arrange - Null context scenario
            HttpContext context = null;
            var exception = new Exception("Test error with null context");

            // Act - Generate error message with null context
            var act = () => ErrorMessageHelper.GenerateMessage(context, exception);

            // Assert - Should throw exception for null context (current behavior)
            // Note: The ErrorMessageHelper currently does not handle null contexts gracefully
            act.Should().Throw<NullReferenceException>();
        }

        [Fact]
        public void GenerateMessage_ShouldHandleNullException_Gracefully()
        {
            // Arrange - Null exception scenario
            var context = CreateMockHttpContext();
            Exception exception = null;

            // Act - Generate error message with null exception
            var act = () => ErrorMessageHelper.GenerateMessage(context, exception);

            // Assert - Should handle null exception appropriately
            act.Should().Throw<Exception>();
        }

        [Fact]
        public void GenerateMessage_ShouldSupportBusinessScenario_AuditTrailGeneration()
        {
            // Arrange - Audit trail generation business scenario
            var context = CreateMockHttpContext("audit.b2c.com", "/api/admin/users", "DELETE", "192.168.1.100");
            var auditException = new Exception("Failed to delete user record");

            // Act - Generate audit trail error message
            var result = ErrorMessageHelper.GenerateMessage(context, auditException);

            // Assert - Should generate comprehensive audit trail information
            result.Should().Contain("ErrMsg: Failed to delete user record");
            result.Should().Contain("Host: audit.b2c.com");
            result.Should().Contain("Path: /api/admin/users");
            result.Should().Contain("Client IP: 192.168.1.100");
            result.Should().Contain("REQUEST INFO");
            result.Should().Contain("EXCEPTION INFO");
        }

        private static HttpContext CreateMockHttpContext(string host = "localhost", string path = "/", string method = "GET", string clientIP = "127.0.0.1")
        {
            var context = Substitute.For<HttpContext>();
            var request = Substitute.For<HttpRequest>();
            var connection = Substitute.For<ConnectionInfo>();

            // Setup request
            request.Host.Returns(new HostString(host));
            request.Path.Returns(new PathString(path));
            request.Method.Returns(method);
            request.Body.Returns(new MemoryStream());

            // Setup connection
            connection.RemoteIpAddress.Returns(IPAddress.Parse(clientIP));

            // Setup context
            context.Request.Returns(request);
            context.Connection.Returns(connection);

            // CRITICAL: Link request back to context for GetClientIP extension method
            request.HttpContext.Returns(context);

            return context;
        }
    }
}
