using Fanex.B2C.Ext.Service.Core.Middleware;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Net;

namespace Fanex.B2C.Ext.Service.Core.Tests.Middlewares
{
    public class ErrorHandlerTest
    {
        private RequestDelegate _mockNext { get; set; }
        private ILogger<ErrorHandler> _mockLogger { get; set; }

        public ErrorHandlerTest()
        {
            _mockNext = Substitute.For<RequestDelegate>();
            _mockLogger = Substitute.For<ILogger<ErrorHandler>>();
        }

        [Fact]
        public async Task Invoke_WhenCallWithNoException_NotLogging()
        {
            //Arrange
            var errorHandler = new ErrorHandler(_mockNext, _mockLogger);

            //Action
            await errorHandler.Invoke(CreateMockHttpContext());

            // Assert
            await _mockNext.Received().Invoke(Arg.Any<HttpContext>());
        }

        [Fact]
        public async Task Invoke_WhenCallInvokeAndHasException_ErrorHandlerHandleExceptionAndCallNextInvoke()
        {
            //Arrange
            var context = CreateMockHttpContext();
            var reponse = Substitute.For<HttpResponse>();
            var features = Substitute.For<IFeatureCollection>();

            using (var stream = new MemoryStream())
            {
                context.Response.ReturnsForAnyArgs(reponse);
                context.Features.ReturnsForAnyArgs(features);
                reponse.HttpContext.ReturnsForAnyArgs(context);
                reponse.Headers.ReturnsForAnyArgs(new HeaderDictionary());
                reponse.Body = stream;

                features[typeof(IExceptionHandlerFeature)].ReturnsForAnyArgs(new ExceptionHandlerFeature { Error = new Exception("Test") });
                var errorHandler = new ErrorHandler(_mockNext, _mockLogger);

                //Action
                await errorHandler.Invoke(context);

                //Assert
                await _mockNext.Received().Invoke(Arg.Any<HttpContext>());
                Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
            }
        }

        [Fact]
        public async Task Invoke_WhenCallNextInvokeAndThrowException_ErrorHandlerHandleException()
        {
            //Arrange
            var context = CreateMockHttpContext();
            var reponse = Substitute.For<HttpResponse>();
            var features = Substitute.For<IFeatureCollection>();

            using (var stream = new MemoryStream())
            {
                context.Response.ReturnsForAnyArgs(reponse);
                context.Features.ReturnsForAnyArgs(features);
                reponse.HttpContext.ReturnsForAnyArgs(context);
                reponse.Headers.ReturnsForAnyArgs(new HeaderDictionary());
                reponse.Body = stream;

                features.Get<IHttpResponseFeature>().ReturnsForAnyArgs(new HttpResponseFeature());
                _mockNext.Invoke(context).ReturnsForAnyArgs(x => throw new Exception("Test"));
                var errorHandler = new ErrorHandler(_mockNext, _mockLogger);

                //Action
                await errorHandler.Invoke(context);

                //Assert
                await _mockNext.Received().Invoke(Arg.Any<HttpContext>());
                Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
            }
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

            // Setup connection
            connection.RemoteIpAddress.Returns(System.Net.IPAddress.Parse(clientIP));

            // Setup context
            context.Request.Returns(request);
            context.Connection.Returns(connection);

            // CRITICAL: Link request back to context for GetClientIP extension method
            request.HttpContext.Returns(context);

            return context;
        }
    }
}