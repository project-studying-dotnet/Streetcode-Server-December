using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Comment;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Comment.CreateComment;
using Streetcode.BLL.MediatR.Comment.GetCommentsByStreetcodeId;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Specifications.Comment;
using Streetcode.DAL.Entities.AdditionalContent.Coordinates.Types;
using Streetcode.DAL.Entities.Comment;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using CommentEntity = Streetcode.DAL.Entities.Comment.Comment;

namespace Streetcode.XUnitTest.MediatRTests.Comment;

public class CreateCommentHandlerTest
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly CreateCommentHandler _handler;

    public CreateCommentHandlerTest()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryMock = new Mock<IRepositoryWrapper>();
        _loggerMock = new Mock<ILoggerService>();

        _handler = new CreateCommentHandler(
            _mapperMock.Object,
            _repositoryMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_ReturnOkResult_WhenCommentAreCreate()
    {
        // Arrange
        var comment = new CommentEntity
        {
            Content = "Test",
            StreetcodeId = 1
        };

        var createCommentDto = new CreateCommentDto
        {
            Content = "Test",
            StreetcodeId = 1
        };

        var getCommentDto = new GetCommentDto
        {
            Content = "Test",
            StreetcodeId = 1
        };

        var streetcode = new StreetcodeContent
        {
            Id = 1
        };

        var request = new CreateCommentCommand(createCommentDto);

        _mapperMock.Setup(m => m.Map<CommentEntity>(createCommentDto))
            .Returns(comment);

        _repositoryMock.Setup(rep => rep.CommentRepository.CreateAsync(comment))
            .ReturnsAsync(comment);

        _repositoryMock.Setup(rep => rep.SaveChangesAsync())
            .ReturnsAsync(1);

        _mapperMock.Setup(m => m.Map<GetCommentDto>(comment))
            .Returns(getCommentDto);
        
        _repositoryMock.Setup(rep => rep.StreetcodeRepository
                .GetFirstOrDefaultAsync(
                    It.Is<Expression<Func<StreetcodeContent, bool>>>(exp => exp.Compile().Invoke(streetcode)),
                    It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>>()))
            .ReturnsAsync(streetcode);
        
        // Act
        var result = await _handler.Handle(request, CancellationToken.None);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(getCommentDto, result.Value);
    }
    
    [Fact]
    public async Task Handle_ReturnOkResult_WhenStreetcodeAreNotFound()
    {
        // Arrange
        var createCommentDto = new CreateCommentDto
        {
            Content = "Test",
            StreetcodeId = 1
        };
        
        var request = new CreateCommentCommand(createCommentDto);

        var errMsg = ErrorManager.GetCustomErrorText("CantFindByIdError", "streetcode", createCommentDto.StreetcodeId);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);
        
        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(errMsg, result.Errors[0].Message);
    }
    
    [Fact]
    public async Task Handle_ReturnOkResult_WhenCommentAreNotMapped()
    {
        // Arrange
        var createCommentDto = new CreateCommentDto
        {
            Content = "Test",
            StreetcodeId = 1
        };

        var streetcode = new StreetcodeContent
        {
            Id = 1
        };

        _mapperMock.Setup(m => m.Map<CommentEntity>(createCommentDto))
            .Returns((CommentEntity)null);

        var request = new CreateCommentCommand(createCommentDto);

        var errMsg = ErrorManager.GetCustomErrorText("ConvertationError", "create comment dto", "CommentEntity");

        _repositoryMock.Setup(rep => rep.StreetcodeRepository
                .GetFirstOrDefaultAsync(
                    It.Is<Expression<Func<StreetcodeContent, bool>>>(exp => exp.Compile().Invoke(streetcode)),
                    It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>>()))
            .ReturnsAsync(streetcode);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(errMsg, result.Errors[0].Message);
    }
    
    [Fact]
    public async Task Handle_ReturnOkResult_WhenCommentAreNotSaveInDb()
    {
        // Arrange
        var comment = new CommentEntity
        {
            Content = "Test",
            StreetcodeId = 1
        };

        var createCommentDto = new CreateCommentDto
        {
            Content = "Test",
            StreetcodeId = 1
        };
        
        var streetcode = new StreetcodeContent
        {
            Id = 1
        };

        var request = new CreateCommentCommand(createCommentDto);
        var errMsg = ErrorManager.GetCustomErrorText("FailCreateError", "comment", request);


        _mapperMock.Setup(m => m.Map<CommentEntity>(createCommentDto))
            .Returns(comment);

        _repositoryMock.Setup(rep => rep.CommentRepository.CreateAsync(comment))
            .ReturnsAsync(comment);

        _repositoryMock.Setup(rep => rep.SaveChangesAsync())
            .ReturnsAsync(0);
        
        _repositoryMock.Setup(rep => rep.StreetcodeRepository
                .GetFirstOrDefaultAsync(
                    It.Is<Expression<Func<StreetcodeContent, bool>>>(exp => exp.Compile().Invoke(streetcode)),
                    It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>>()))
            .ReturnsAsync(streetcode);
        
        // Act
        var result = await _handler.Handle(request, CancellationToken.None);
        
        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(errMsg, result.Errors[0].Message);
    }

}