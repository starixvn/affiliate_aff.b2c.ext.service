using Fanex.B2C.Ext.Service.Core.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Fanex.B2C.Ext.Service.Core.Tests.Controllers
{
    /// <summary>
    /// Comprehensive unit tests for ErrorController.
    /// Tests error handling scenarios and HTTP status code responses.
    /// Following EFFECTIVE_UNIT_TESTING_RULES.md guidelines for business-focused testing.
    /// </summary>
    public class ErrorControllerTest
    {
        [Fact]
        public void Index_ShouldReturnUnauthorizedMessage_WhenStatusCodeIs401()
        {
            // Arrange - Unauthorized access scenario
            var controller = new ErrorController();
            var statusCode = (int)HttpStatusCode.Unauthorized;

            // Act - Handle unauthorized error
            var result = controller.Index(statusCode) as ObjectResult;

            // Assert - Should return appropriate unauthorized message
            result.Should().NotBeNull();
            result.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
            result.Value.Should().Be("Authentication required. Please provide a valid token.");
        }

        [Fact]
        public void Index_ShouldReturnForbiddenMessage_WhenStatusCodeIs403()
        {
            // Arrange - Forbidden access scenario
            var controller = new ErrorController();
            var statusCode = (int)HttpStatusCode.Forbidden;

            // Act - Handle forbidden error
            var result = controller.Index(statusCode) as ObjectResult;

            // Assert - Should return appropriate forbidden message
            result.Should().NotBeNull();
            result.StatusCode.Should().Be((int)HttpStatusCode.Forbidden);
            result.Value.Should().Be("You don't have permission to access this resource.");
        }

        [Fact]
        public void Index_ShouldReturnNotFoundMessage_WhenStatusCodeIs404()
        {
            // Arrange - Not found scenario
            var controller = new ErrorController();
            var statusCode = (int)HttpStatusCode.NotFound;

            // Act - Handle not found error
            var result = controller.Index(statusCode) as ObjectResult;

            // Assert - Should return appropriate not found message
            result.Should().NotBeNull();
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            result.Value.Should().Be("The requested resource was not found.");
        }

        [Theory]
        [InlineData(500)]
        [InlineData(502)]
        [InlineData(503)]
        [InlineData(400)]
        [InlineData(422)]
        public void Index_ShouldReturnGenericMessage_WhenStatusCodeIsUnhandled(int statusCode)
        {
            // Arrange - Unhandled status code scenario
            var controller = new ErrorController();

            // Act - Handle unhandled status code
            var result = controller.Index(statusCode) as ObjectResult;

            // Assert - Should return generic error message with status code
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(statusCode);
            result.Value.Should().Be($"An error occurred. Status code: {statusCode}");
        }

        [Fact]
        public void Index_ShouldSupportBusinessScenario_APIAuthenticationError()
        {
            // Arrange - API authentication error business scenario
            var controller = new ErrorController();
            var statusCode = (int)HttpStatusCode.Unauthorized;

            // Act - Handle API authentication error
            var result = controller.Index(statusCode) as ObjectResult;

            // Assert - Should provide clear authentication guidance for API consumers
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(401);
            result.Value.Should().Be("Authentication required. Please provide a valid token.");
        }

        [Fact]
        public void Index_ShouldSupportBusinessScenario_InsufficientPrivilegesError()
        {
            // Arrange - Insufficient privileges business scenario
            var controller = new ErrorController();
            var statusCode = (int)HttpStatusCode.Forbidden;

            // Act - Handle insufficient privileges error
            var result = controller.Index(statusCode) as ObjectResult;

            // Assert - Should provide clear authorization guidance
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(403);
            result.Value.Should().Be("You don't have permission to access this resource.");
        }

        [Fact]
        public void Index_ShouldSupportBusinessScenario_ResourceNotFoundError()
        {
            // Arrange - Resource not found business scenario
            var controller = new ErrorController();
            var statusCode = (int)HttpStatusCode.NotFound;

            // Act - Handle resource not found error
            var result = controller.Index(statusCode) as ObjectResult;

            // Assert - Should provide clear resource guidance
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(404);
            result.Value.Should().Be("The requested resource was not found.");
        }

        [Fact]
        public void Index_ShouldSupportBusinessScenario_ServerErrorHandling()
        {
            // Arrange - Server error handling business scenario
            var controller = new ErrorController();
            var statusCode = (int)HttpStatusCode.InternalServerError;

            // Act - Handle server error
            var result = controller.Index(statusCode) as ObjectResult;

            // Assert - Should provide generic error message for server errors
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(500);
            result.Value.Should().Be("An error occurred. Status code: 500");
        }

        [Fact]
        public void Index_ShouldSupportBusinessScenario_BadRequestHandling()
        {
            // Arrange - Bad request handling business scenario
            var controller = new ErrorController();
            var statusCode = (int)HttpStatusCode.BadRequest;

            // Act - Handle bad request error
            var result = controller.Index(statusCode) as ObjectResult;

            // Assert - Should provide generic error message for bad requests
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(400);
            result.Value.Should().Be("An error occurred. Status code: 400");
        }

        [Fact]
        public void Index_ShouldSupportBusinessScenario_ServiceUnavailableHandling()
        {
            // Arrange - Service unavailable business scenario
            var controller = new ErrorController();
            var statusCode = (int)HttpStatusCode.ServiceUnavailable;

            // Act - Handle service unavailable error
            var result = controller.Index(statusCode) as ObjectResult;

            // Assert - Should provide appropriate error message for service unavailable
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(503);
            result.Value.Should().Be("An error occurred. Status code: 503");
        }

        [Fact]
        public void Index_ShouldSupportBusinessScenario_LicenseeAccessDenied()
        {
            // Arrange - Licensee access denied business scenario (403 Forbidden)
            var controller = new ErrorController();
            var statusCode = (int)HttpStatusCode.Forbidden;

            // Act - Handle licensee access denied
            var result = controller.Index(statusCode) as ObjectResult;

            // Assert - Should provide clear message for licensee access issues
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(403);
            result.Value.Should().Be("You don't have permission to access this resource.");
        }

        [Fact]
        public void Index_ShouldSupportBusinessScenario_TokenExpiredError()
        {
            // Arrange - Token expired error business scenario (401 Unauthorized)
            var controller = new ErrorController();
            var statusCode = (int)HttpStatusCode.Unauthorized;

            // Act - Handle token expired error
            var result = controller.Index(statusCode) as ObjectResult;

            // Assert - Should provide authentication guidance for expired tokens
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(401);
            result.Value.Should().Be("Authentication required. Please provide a valid token.");
        }

        [Theory]
        [InlineData(401, "Authentication required. Please provide a valid token.")]
        [InlineData(403, "You don't have permission to access this resource.")]
        [InlineData(404, "The requested resource was not found.")]
        [InlineData(500, "An error occurred. Status code: 500")]
        [InlineData(502, "An error occurred. Status code: 502")]
        [InlineData(503, "An error occurred. Status code: 503")]
        public void Index_ShouldReturnConsistentMessages_ForVariousStatusCodes(int statusCode, string expectedMessage)
        {
            // Arrange - Various status codes scenario
            var controller = new ErrorController();

            // Act - Handle various status codes
            var result = controller.Index(statusCode) as ObjectResult;

            // Assert - Should return consistent messages for each status code
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(statusCode);
            result.Value.Should().Be(expectedMessage);
        }
    }
}