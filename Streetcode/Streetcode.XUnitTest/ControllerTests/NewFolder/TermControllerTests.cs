using Moq;
using FluentAssertions;
using MediatR;
using Streetcode.BLL.DTO.Terms;
using Streetcode.BLL.MediatR.Terms;
using Streetcode.WebApi.Controllers.Terms;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Streetcode.BLL.DTO.Streetcode.TextContent;

namespace Streetcode.WebApi.Tests.Controllers
{
    public class TermControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly TermController _controller;

        public TermControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new TermController(_mockMediator.Object);
        }

        [Fact]
        public async Task CreateTerm_ShouldReturnCreatedResult_WhenTermIsSuccessfullyCreated()
        {
            var termCreateDTO = new TermCreateDTO
            {
                Title = "Sample Term",
                Description = "Description of the sample term"
            };

            var termDto = new TermDto
            {
                Id = 1,
                Title = "Sample Term",
                Description = "Description of the sample term"
            };

            var command = new CreateTermCommand(termCreateDTO);
            var resultFromMediator = FluentResults.Result.Ok(termDto);

            _mockMediator.Setup(m => m.Send(It.IsAny<CreateTermCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(resultFromMediator);

            var result = await _controller.CreateTerm(termCreateDTO);

            var actionResult = result as CreatedAtActionResult;
            actionResult.Should().NotBeNull();
            actionResult.StatusCode.Should().Be(201); 
            actionResult.Value.Should().BeEquivalentTo(termDto);
            actionResult.RouteValues["id"].Should().Be(termDto.Id);
        }

        [Fact]
        public async Task CreateTerm_ShouldReturnBadRequest_WhenTermCreationFails()
        {
            var termCreateDTO = new TermCreateDTO
            {
                Title = "Sample Term",
                Description = "Description of the sample term"
            };

            var command = new CreateTermCommand(termCreateDTO);
            var resultFromMediator = FluentResults.Result.Fail("An error occurred during term creation");

            _mockMediator.Setup(m => m.Send(It.IsAny<CreateTermCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(resultFromMediator);

            var result = await _controller.CreateTerm(termCreateDTO);

            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400); 
            badRequestResult.Value.Should().BeEquivalentTo(resultFromMediator.Errors);
        }
    }
}
