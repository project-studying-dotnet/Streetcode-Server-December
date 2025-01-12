using AutoMapper;
using FluentResults;
using MediatR;
using Moq;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Art.GetAll;
using Streetcode.BLL.MediatR.Media.Art.GetById;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.Domain.Entities.Media.Images;
using System.Linq.Expressions;
using Xunit;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Streetcode.XUnitTest.MediatRTests.MediaTests.ArtTests
{
    public class GetArtByIdHandlerTest
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRepositoryWrapper> _repositoryMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly GetArtByIdHandler _getArtByIdHandler;
        public GetArtByIdHandlerTest()
        {
            _mapperMock = new();
            _repositoryMock = new();
            _loggerMock = new();
            _getArtByIdHandler = new(_repositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnArtById_WhenArtExists()
        {
            // A(Arrange):

            int artId = 3;
            var artById = new Art { Id = artId, Description = "None1", Title = "ArtById", ImageId = 11 };          
            var artDtoById = new ArtDto { Id = artId, Description = "None1", Title = "ArtById", ImageId = 11 };
            var query = new GetArtByIdQuery(artId);

            _repositoryMock.Setup(r => r.ArtRepository.GetFirstOrDefaultAsync(It.Is<Expression<Func<Art, bool>>>(predicate => predicate.Compile()(artById)), null)).ReturnsAsync(artById);
            _mapperMock.Setup(m => m.Map<ArtDto>(artById)).Returns(artDtoById);

            // A(Act):

            var res = await _getArtByIdHandler.Handle(query, CancellationToken.None);

            // A(Assert):

            Assert.True(res.IsSuccess);
            Assert.Equal(artDtoById.Id, res.Value.Id);
           
            _repositoryMock.Verify(r => r.ArtRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Art, bool>>>(), null), Times.Once);
            _mapperMock.Verify(m => m.Map<ArtDto>(artById), Times.Once);
            _loggerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenArtDoesntExists()
        {
            // A(Arrange):

            int artIncorrectId = 3000;        

            _repositoryMock.Setup(r => r.ArtRepository.GetFirstOrDefaultAsync(It.Is<Expression<Func<Art, bool>>>(predicate => predicate.Compile()(new Art { Id = artIncorrectId })), null)).ReturnsAsync(null as Art);

            // A(Act):

            var res = await _getArtByIdHandler.Handle(new GetArtByIdQuery(artIncorrectId), CancellationToken.None);

            // A(Assert):

            Assert.True(res.IsFailed);
            Assert.Single(res.Errors);
            Assert.Equal($"Cannot find a art with corresponding id: {artIncorrectId}", res.Errors[0].Message);

            _repositoryMock.Verify(r => r.ArtRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Art, bool>>>(), null), Times.Once);
            _loggerMock.Verify(l => l.LogError(new GetArtByIdQuery(artIncorrectId), $"Cannot find a art with corresponding id: {artIncorrectId}"), Times.Once);
        }
    }
}
