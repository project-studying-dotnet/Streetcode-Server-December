using AutoMapper;
using FluentAssertions;
using Streetcode.BLL.DTO.Streetcode.TextContent;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Streetcode.TextContent;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Create;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.GetById;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Update;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Streetcode.BLL.MediatR.Streetcode.Fact.FactReorder;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Entities.AdditionalContent;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;

namespace Streetcode.XUnitTest.MediatRTests.Streetcode.FactTests
{
    public class FactReorderHandlerTest
    {
        private readonly IMapper _mapper;
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly FactReorderHandler _handler;

        public FactReorderHandlerTest()
        {
            this._mockLogger = new Mock<ILoggerService>();
            this._mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new FactProfile());
            });
            this._mapper = configuration.CreateMapper();
            this._handler = new FactReorderHandler(this._mapper, this._mockRepositoryWrapper.Object, this._mockLogger.Object);
        }

        [Fact]
        public async Task Handle_WhenFactsAreReorderedSuccessfully_ReturnsOkResult()
        {
            // Arrange
            var factReorders = new List<FactUpdateCreateDto>
            {
               new ()
                {
                    Id = 1,
                    Title = "Fact 1",
                    ImageId = 10,
                    FactContent = "Content 1",
                    ImageDescription = "Description 1",
                },
               new ()
                {
                    Id = 2,
                    Title = "Fact 2",
                    ImageId = 20,
                    FactContent = "Content 2",
                    ImageDescription = "Description 2",
                },
            };
            var idPositions = new List<int> { 2, 1 };

            var factReorderDto = new FactReorderDto
            {
                FactReorders = factReorders,
                IdPositions = idPositions,
            };

            var command = new FactReorderCommand(factReorderDto);

            var existingFacts = new List<Fact>
            {
                new ()
                {
                    Id = 1,
                    StreetcodeId = 1,
                },
                new ()
                {
                   Id = 2,
                   StreetcodeId = 1,
                },
            };

            this._mockRepositoryWrapper.Setup(repo => repo.FactRepository.GetAllAsync(It.IsAny<Expression<Func<Fact, bool>>>(), null))
                .ReturnsAsync(existingFacts);

            this._mockRepositoryWrapper.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await this._handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(factReorders.Count, result.Value.FactReorders.Count);
        }

        [Fact]
        public async Task Handle_WhenFactsCountAndPositionsCountAreNotEqual_ReturnsFailureResult()
        {
            // Arrange
            var factReorders = new List<FactUpdateCreateDto>
            {
               new ()
                {
                    Id = 1,
                    Title = "Fact 1",
                    ImageId = 10,
                    FactContent = "Content 1",
                    ImageDescription = "Description 1",
                },
               new ()
                {
                    Id = 2,
                    Title = "Fact 2",
                    ImageId = 20,
                    FactContent = "Content 2",
                    ImageDescription = "Description 2",
                },
            };
            var idPositions = new List<int> { 2 };

            var factReorderDto = new FactReorderDto
            {
                FactReorders = factReorders,
                IdPositions = idPositions,
            };

            var command = new FactReorderCommand(factReorderDto);

            var existingFacts = new List<Fact>
            {
                new ()
                {
                    Id = 1,
                    StreetcodeId = 1,
                },
                new ()
                {
                   Id = 2,
                   StreetcodeId = 1,
                },
            };

            this._mockRepositoryWrapper.Setup(repo => repo.FactRepository.GetAllAsync(It.IsAny<Expression<Func<Fact, bool>>>(), null))
                .ReturnsAsync(existingFacts);

            // Act
            var result = await this._handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("FactReorders and Positions should have equal count of objects!", result.Errors.First().Message);
        }

        [Fact]
        public async Task Handle_WhenFactsDoNotExist_ReturnsFailureResult()
        {
            // Arrange
            var factReorders = new List<FactUpdateCreateDto>
            {
               new ()
                {
                    Id = 1,
                    Title = "Fact 1",
                    ImageId = 10,
                    FactContent = "Content 1",
                    ImageDescription = "Description 1",
                },
               new ()
                {
                    Id = 2,
                    Title = "Fact 2",
                    ImageId = 20,
                    FactContent = "Content 2",
                    ImageDescription = "Description 2",
                },
            };
            var idPositions = new List<int> { 2, 1 };

            var factReorderDto = new FactReorderDto
            {
                FactReorders = factReorders,
                IdPositions = idPositions,
            };

            var command = new FactReorderCommand(factReorderDto);

            var existingFacts = new List<Fact>
            {
                new ()
                {
                    Id = 1,
                    StreetcodeId = 1,
                },
                new ()
                {
                   Id = 2,
                   StreetcodeId = 1,
                },
            };

            this._mockRepositoryWrapper.Setup(repo => repo.FactRepository.GetAllAsync(It.IsAny<Expression<Func<Fact, bool>>>(), null))
                .ReturnsAsync((List<Fact>?)null);

            // Act
            var result = await this._handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Facts not found!", result.Errors.First().Message);
        }

        [Fact]
        public async Task Handle_WhenSomeFactsDoesNotExist_ReturnsFailureResult()
        {
            // Arrange
            var factReorders = new List<FactUpdateCreateDto>
            {
               new ()
                {
                    Id = 1,
                    Title = "Fact 1",
                    ImageId = 10,
                    FactContent = "Content 1",
                    ImageDescription = "Description 1",
                },
               new ()
                {
                    Id = 3,
                    Title = "Fact 2",
                    ImageId = 20,
                    FactContent = "Content 2",
                    ImageDescription = "Description 2",
                },
            };
            var idPositions = new List<int> { 3, 1 };

            var factReorderDto = new FactReorderDto
            {
                FactReorders = factReorders,
                IdPositions = idPositions,
            };

            var command = new FactReorderCommand(factReorderDto);

            var existingFacts = new List<Fact>
            {
                new ()
                {
                    Id = 1,
                    StreetcodeId = 1,
                },
                new ()
                {
                   Id = 2,
                   StreetcodeId = 1,
                },
            };

            this._mockRepositoryWrapper.Setup(repo => repo.FactRepository.GetAllAsync(It.IsAny<Expression<Func<Fact, bool>>>(), null))
                .ReturnsAsync(existingFacts);

            this._mockRepositoryWrapper.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(0);

            // Act
            var result = await this._handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Failed to save the reordered facts.", result.Errors.First().Message);
        }

        [Fact]
        public async Task Handle_WhenFactsDoNotHaveSameStreetcodeId_ReturnsFailureResult()
        {
            // Arrange
            var factReorders = new List<FactUpdateCreateDto>
            {
               new ()
                {
                    Id = 1,
                    Title = "Fact 1",
                    ImageId = 10,
                    FactContent = "Content 1",
                    ImageDescription = "Description 1",
                },
               new ()
                {
                    Id = 3,
                    Title = "Fact 2",
                    ImageId = 20,
                    FactContent = "Content 2",
                    ImageDescription = "Description 2",
                },
            };
            var idPositions = new List<int> { 3, 1 };

            var factReorderDto = new FactReorderDto
            {
                FactReorders = factReorders,
                IdPositions = idPositions,
            };

            var command = new FactReorderCommand(factReorderDto);

            var existingFacts = new List<Fact>
            {
                new ()
                {
                    Id = 1,
                    StreetcodeId = 1,
                },
                new ()
                {
                   Id = 2,
                   StreetcodeId = 2,
                },
            };

            this._mockRepositoryWrapper.Setup(repo => repo.FactRepository.GetAllAsync(It.IsAny<Expression<Func<Fact, bool>>>(), null))
                .ReturnsAsync(existingFacts);

            this._mockRepositoryWrapper.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(0);

            // Act
            var result = await this._handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("All Facts should be linked to one Streetcode", result.Errors.First().Message);
        }

        [Fact]
        public async Task Handle_WhenSaveFails_ReturnsFailureResult()
        {
            // Arrange
            var factReorders = new List<FactUpdateCreateDto>
            {
               new ()
                {
                    Id = 1,
                    Title = "Fact 1",
                    ImageId = 10,
                    FactContent = "Content 1",
                    ImageDescription = "Description 1",
                },
               new ()
                {
                    Id = 2,
                    Title = "Fact 2",
                    ImageId = 20,
                    FactContent = "Content 2",
                    ImageDescription = "Description 2",
                },
            };
            var idPositions = new List<int> { 2, 1 };

            var factReorderDto = new FactReorderDto
            {
                FactReorders = factReorders,
                IdPositions = idPositions,
            };

            var command = new FactReorderCommand(factReorderDto);

            var existingFacts = new List<Fact>
            {
                new ()
                {
                    Id = 1,
                    StreetcodeId = 1,
                },
                new ()
                {
                   Id = 2,
                   StreetcodeId = 1,
                },
            };

            this._mockRepositoryWrapper.Setup(repo => repo.FactRepository.GetAllAsync(It.IsAny<Expression<Func<Fact, bool>>>(), null))
                .ReturnsAsync(existingFacts);

            this._mockRepositoryWrapper.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(0);

            // Act
            var result = await this._handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Failed to save the reordered facts.", result.Errors.First().Message);
        }
    }
}
