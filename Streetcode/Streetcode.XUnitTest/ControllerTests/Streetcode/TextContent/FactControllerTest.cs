using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Streetcode.BLL.DTO.Streetcode.TextContent;
using Streetcode.WebApi.Controllers.Streetcode.TextContent;
using Streetcode.WebApi.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Streetcode.BLL.MediatR.Streetcode.Fact.FactReorder;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;

namespace Streetcode.XUnitTest.ControllerTests.Streetcode.TextContent
{
    public class FactControllerTest
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly FactController _controller;

        public FactControllerTest()
        {
            this._mediatorMock = new Mock<IMediator>();

            this._controller = new FactController();

            var mediatorField = typeof(BaseApiController)
                .GetField("_mediator", BindingFlags.Instance | BindingFlags.NonPublic);

            mediatorField.SetValue(this._controller, this._mediatorMock.Object);
        }

        [Fact]
        public async Task Update_ReturnsReorderedFactsResult()
        {
            // Arrange
            var reorderedFacts = new List<FactDto> 
            {
                new()
                {
                    Id = 1,
                    Title = "Test",
                    ImageId = 1,
                    FactContent = "Test",
                    Index = 1,
                },
                new()
                {
                    Id = 2,
                    Title = "Test",
                    ImageId = 2,
                    FactContent = "Test",
                    Index = 2,
                }
            };
            this._mediatorMock.Setup(m => m.Send(It.IsAny<FactReorderCommand>(), default))
                              .ReturnsAsync(reorderedFacts);

            var factReorderDto = new FactReorderDto()
            {
                StreetcodeId = 1,
                IdPositions = new List<int> { 1, 2 }
            };

            // Act
            var result = await this._controller.ReorderFacts(factReorderDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(reorderedFacts, okResult.Value);
        }
    }
}
