using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Newss.Update;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Update;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Sources.SourceLinkCategory;

public class UpdateSourceLinkCategoryHandlerTest
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<ILoggerService> _loggerMock;

    private readonly UpdateSourceLinkCategoryHandler _handler;

    public UpdateSourceLinkCategoryHandlerTest()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _loggerMock = new Mock<ILoggerService>();

        _handler = new(_mapperMock.Object, _repositoryWrapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnOkResult_WhenSourceLinkCategoryAreUpdate()
    {
        // Arrange
        var sourceLinkCategory = new DAL.Entities.Sources.SourceLinkCategory
        {
            Id = 1,
            Title = "Test 1",
            ImageId = 2,
            Image = new Image { Id = 2 },
        };

        var sourceLinkCategoryDto = new SourceLinkCategoryDTO
        {
            Id = 1,
            Title = "Test 1",
            ImageId = 2,
            Image = new ImageDTO { Id = 2 },
        };

        var command = new UpdateSourceLinkCategoryCommand(sourceLinkCategoryDto);
        
        _mapperMock
            .Setup(m => m.Map<DAL.Entities.Sources.SourceLinkCategory>(sourceLinkCategoryDto))
            .Returns(sourceLinkCategory);
        
        _mapperMock
            .Setup(m => m.Map<SourceLinkCategoryDTO>(sourceLinkCategory))
            .Returns(sourceLinkCategoryDto);

        _repositoryWrapperMock
            .Setup(rep => rep.SourceCategoryRepository
                .Update(sourceLinkCategory));
        
        _repositoryWrapperMock.Setup(rep => rep.SourceCategoryRepository
                .GetFirstOrDefaultAsync(
                    It.Is<Expression<Func<DAL.Entities.Sources.SourceLinkCategory, bool>>>(exp => exp.Compile().Invoke(sourceLinkCategory)),
                    It.IsAny<Func<IQueryable<DAL.Entities.Sources.SourceLinkCategory>, IIncludableQueryable<DAL.Entities.Sources.SourceLinkCategory, object>>>()))
            .ReturnsAsync(sourceLinkCategory);
        
        _repositoryWrapperMock
            .Setup(rep => rep
                .SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(sourceLinkCategoryDto, result.Value);

        _repositoryWrapperMock.Verify(rep => rep.SourceCategoryRepository.Update(sourceLinkCategory), Times.Once);
        _mapperMock.Verify(m => m.Map<DAL.Entities.Sources.SourceLinkCategory>(sourceLinkCategoryDto), Times.Once);
        _repositoryWrapperMock.Verify(rep => rep.SaveChangesAsync(), Times.Once());
    }

    [Fact]
    public async Task Handler_ReturnsError_WhenSourceLinkCategoryAreNotMapped()
    {
        // Arrange
        var sourceLinkCategory = new DAL.Entities.Sources.SourceLinkCategory
        {
            Id = 1,
            Title = "Test 1",
            ImageId = 2,
            Image = new Image { Id = 2 },
        };

        var sourceLinkCategoryDto = new SourceLinkCategoryDTO
        {
            Id = 1,
            Title = "Test 1",
            ImageId = 2,
            Image = new ImageDTO { Id = 2 },
        };
        var command = new UpdateSourceLinkCategoryCommand(sourceLinkCategoryDto);
        const string errorMsg = $"Cannot convert null to source link category";
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.Equal(errorMsg, result.Errors[0].Message);
    }
    
    [Fact]
    public async Task Handler_ReturnsError_WhenExistSourceLinkCategoryAreNotFound()
    {
        // Arrange
        var sourceLinkCategory = new DAL.Entities.Sources.SourceLinkCategory
        {
            Id = 1,
            Title = "Test 1",
            ImageId = 2,
            Image = new Image { Id = 2 },
        };

        var sourceLinkCategoryDto = new SourceLinkCategoryDTO
        {
            Id = 1,
            Title = "Test 1",
            ImageId = 2,
            Image = new ImageDTO { Id = 2 },
        };
        var command = new UpdateSourceLinkCategoryCommand(sourceLinkCategoryDto);
        const string errorMsg = $"Cannot find source link category in db";
        
        _mapperMock
            .Setup(m => m.Map<DAL.Entities.Sources.SourceLinkCategory>(sourceLinkCategoryDto))
            .Returns(sourceLinkCategory);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.Equal(errorMsg, result.Errors[0].Message);
    }

    [Fact]
    public async Task Handler_ReturnsError_WhenSaveInDbFailed()
    {
        // Arrange
        var sourceLinkCategory = new DAL.Entities.Sources.SourceLinkCategory
        {
            Id = 1,
            Title = "Test 1",
            ImageId = 2,
            Image = new Image { Id = 2 },
        };

        var sourceLinkCategoryDto = new SourceLinkCategoryDTO
        {
            Id = 1,
            Title = "Test 1",
            ImageId = 2,
            Image = new ImageDTO { Id = 2 },
        };

        var command = new UpdateSourceLinkCategoryCommand(sourceLinkCategoryDto);
        const string errorMsg = $"Failed to update news";
        
        _mapperMock
            .Setup(m => m.Map<DAL.Entities.Sources.SourceLinkCategory>(sourceLinkCategoryDto))
            .Returns(sourceLinkCategory);
        
        _mapperMock
            .Setup(m => m.Map<SourceLinkCategoryDTO>(sourceLinkCategory))
            .Returns(sourceLinkCategoryDto);

        _repositoryWrapperMock
            .Setup(rep => rep.SourceCategoryRepository
                .Update(sourceLinkCategory));
        
        _repositoryWrapperMock.Setup(rep => rep.SourceCategoryRepository
                .GetFirstOrDefaultAsync(
                    It.Is<Expression<Func<DAL.Entities.Sources.SourceLinkCategory, bool>>>(exp => exp.Compile().Invoke(sourceLinkCategory)),
                    It.IsAny<Func<IQueryable<DAL.Entities.Sources.SourceLinkCategory>, IIncludableQueryable<DAL.Entities.Sources.SourceLinkCategory, object>>>()))
            .ReturnsAsync(sourceLinkCategory);
        
        _repositoryWrapperMock
            .Setup(rep => rep
                .SaveChangesAsync())
            .ReturnsAsync(0);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(errorMsg, result.Errors[0].Message);
        
    }

}