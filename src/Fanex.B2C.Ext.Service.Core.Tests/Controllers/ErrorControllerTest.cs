using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Fanex.B2C.Ext.Service.Core.Controllers;

namespace Fanex.B2C.Ext.Service.Core.Tests.Controllers
{
    public class ErrorControllerTest
    {
        [Fact]
        public void Index_WhenCallWith_UnauthorizedCode_ReturnCorrect()
        {
            //Arrange
            var controller = new ErrorController();

            //Action
            var result = controller.Index(401) as ObjectResult;

            //Assert
            Assert.Equal("You don't have permission to access", result?.Value);
            Assert.Equal((int)HttpStatusCode.Unauthorized, result?.StatusCode);
        }

        [Fact]
        public void Index_WhenCallWith_AnotherStatusCode_ReturnCorrect()
        {
            //Arrange
            var controller = new ErrorController();

            //Action
            var result = controller.Index(0) as ObjectResult;

            //Assert
            Assert.Equal("Page not found", result?.Value);
            Assert.Equal((int)HttpStatusCode.NotFound, result?.StatusCode);
        }
    }
}