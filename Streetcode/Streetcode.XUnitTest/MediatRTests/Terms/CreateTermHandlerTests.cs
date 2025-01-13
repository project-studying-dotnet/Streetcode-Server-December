using Moq;
using FluentAssertions;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces;
using Streetcode.BLL.MediatR.Terms;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using AutoMapper;
using Streetcode.BLL.DTO.Terms;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources;
using System.Resources;
using Streetcode.BLL.Interfaces.Logging;

namespace Streetcode.BLL.Tests.MediatR.Terms
{
    public class CreateTermHandlerTests
    {
        private readonly Mock<ITermRepository> _mockTermRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly CreateTermHandler _handler;
        private readonly Mock<ILoggerService> _mockLogger;


        public CreateTermHandlerTests()
        {
            _mockTermRepository = new Mock<ITermRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockLogger = new Mock<ILoggerService>();
            _handler = new CreateTermHandler(_mockTermRepository.Object, _mockMapper.Object, _mockRepositoryWrapper.Object, _mockLogger.Object);
        }


        [Fact]
        public async Task Handle_ShouldReturnTermDto_WhenTermIsCreated()
        {
            var termCreateDTO = new TermCreateDTO
            {
                Title = "Sample Term",
                Description = "Description of the term"
            };

            var term = new Term
            {
                Title = "Sample Term",
                Description = "Description of the term"
            };

            var termDto = new TermDto
            {
                Title = "Sample Term",
                Description = "Description of the term"
            };

            _mockMapper.Setup(m => m.Map<Term>(It.IsAny<TermCreateDTO>())).Returns(term);
            _mockMapper.Setup(m => m.Map<TermDto>(It.IsAny<Term>())).Returns(termDto);
            _mockTermRepository.Setup(r => r.Create(It.IsAny<Term>())).Verifiable();
            _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            var command = new CreateTermCommand(termCreateDTO);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(termDto);

            _mockTermRepository.Verify(r => r.Create(It.IsAny<Term>()), Times.Once);
            _mockRepositoryWrapper.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenSaveFails()
        {
            // Arrange
            var termCreateDTO = new TermCreateDTO
            {

                Title = "Sample Term",
                Description = "Description of the term"
            };

            var term = new Term
            {
                Title = "Sample Term",
                Description = "Description of the term"
            };

            var termDto = new TermDto
            {
                Title = "Sample Term",
                Description = "Description of the term"
            };

            _mockMapper.Setup(m => m.Map<Term>(It.IsAny<TermCreateDTO>())).Returns(term);
            _mockMapper.Setup(m => m.Map<TermDto>(It.IsAny<Term>())).Returns(termDto);
            _mockTermRepository.Setup(r => r.Create(It.IsAny<Term>())).Verifiable();
            _mockRepositoryWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

            var command = new CreateTermCommand(termCreateDTO);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
            _mockLogger.Verify(l => l.LogError(It.IsAny<CreateTermCommand>(), It.IsAny<string>()), Times.Once);
        }

    }
}
