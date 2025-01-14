using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Art.GetAll;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.Domain.Entities.Media.Images;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Streetcode.XUnitTest.MediatRTests.MediaTests.ArtTests
{
    public class GetAllArtsHandlerTest
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRepositoryWrapper> _repositoryMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly GetAllArtsHandler _getAllArtsHandler;

        public GetAllArtsHandlerTest()
        {
            _mapperMock = new();
            _repositoryMock = new();
            _loggerMock = new();
            _getAllArtsHandler = new(_repositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnAllArts_WhenArtsMoreThan0()
        {
            // A(Arrange):

            var allArts = new List<Art>()
            {
                new Art { Id = 1, Description = "None1", Title = "Art_0", ImageId = 11 },
                new Art { Id = 2, Description = "None2", Title = "Art_1", ImageId = 12 },
                new Art { Id = 3, Description = "None3", Title = "Art_2", ImageId = 13 },
                new Art { Id = 4, Description = "None4", Title = "Art_3", ImageId = 14 }
            };

            var allDtos = new List<ArtDto>()
            {
                new ArtDto { Id = 1, Description = "None1", Title = "Art_0", ImageId = 11 },
                new ArtDto { Id = 2, Description = "None2", Title = "Art_1", ImageId = 12 },
                new ArtDto { Id = 3, Description = "None3", Title = "Art_2", ImageId = 13 },
                new ArtDto { Id = 4, Description = "None4", Title = "Art_3", ImageId = 14 }
            };

            _repositoryMock.Setup(r => r.ArtRepository.GetAllAsync(It.IsAny<Expression<Func<Art, bool>>>(), It.IsAny<List<string>>())).ReturnsAsync(allArts);
            _mapperMock.Setup(m => m.Map<IEnumerable<ArtDto>>(allArts)).Returns(allDtos);

            // A(Act):

            var res = await _getAllArtsHandler.Handle(new GetAllArtsQuery(), CancellationToken.None);

            // A(Assert):
            
            Assert.True(res.IsSuccess);
            Assert.Equal(allDtos.Count, res.Value.Count());
            Assert.Collection(allArts,
                allDtos.Select(exp => (Action<Art>)(actual =>
                {
                    Assert.Equal(exp.Description, actual.Description);
                    Assert.Equal(exp.Title, actual.Title);
                    Assert.Equal(exp.ImageId, actual.ImageId);
                })).ToArray());

            _repositoryMock.Verify(r => r.ArtRepository.GetAllAsync(It.IsAny<Expression<Func<Art, bool>>?>(), It.IsAny<List<string>?>()), Times.Once);
            _mapperMock.Verify(m => m.Map<IEnumerable<ArtDto>>(allArts), Times.Once);
            _loggerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenNoArts()
        {
            // A(Arrange):

            _repositoryMock.Setup(r => r.ArtRepository.GetAllAsync(It.IsAny<Expression<Func<Art, bool>>>(), It.IsAny<List<string>>())).ReturnsAsync((IEnumerable<Art>)null!);

            // A(Act):

            var res = await _getAllArtsHandler.Handle(new GetAllArtsQuery(), CancellationToken.None);

            // A(Assert):

            Assert.Single(res.Errors);
            Assert.True(res.IsFailed);
            Assert.Equal("Cannot find any art", res.Errors[0].Message);

            _loggerMock.Verify(l => l.LogError(new GetAllArtsQuery(), "Cannot find any art"), Times.Once);
        }
    }
}
