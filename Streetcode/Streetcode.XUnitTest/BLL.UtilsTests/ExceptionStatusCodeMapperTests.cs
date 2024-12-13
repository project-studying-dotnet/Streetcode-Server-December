using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Streetcode.BLL.Exceptions;
using Streetcode.BLL.Util;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Streetcode.XUnitTest.BLL.UtilsTests
{
    public class ExceptionStatusCodeMapperTests
    {
        [Fact]
        public void MapToStatusCode_ShouldReturn404ForNotFoundException()
        {
            // Arrange
            var exception = new NotFoundException("Not found");

            // Act
            var statusCode = ExceptionStatusCodeMapper.MapToStatusCode(exception);

            // Assert
            Assert.Equal((int)HttpStatusCode.NotFound, statusCode);
        }

        [Fact]
        public void MapToStatusCode_ShouldReturn400ForBadRequestException()
        {
            // Arrange
            var exception = new BadRequestException("Bad request");

            // Act
            var statusCode = ExceptionStatusCodeMapper.MapToStatusCode(exception);

            // Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, statusCode);
        }

        [Fact]
        public void MapToStatusCode_ShouldReturn500ForGeneralException()
        {
            // Arrange
            var exception = new Exception("Other error");

            // Act
            var statusCode = ExceptionStatusCodeMapper.MapToStatusCode(exception);

            // Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }
    }
}
