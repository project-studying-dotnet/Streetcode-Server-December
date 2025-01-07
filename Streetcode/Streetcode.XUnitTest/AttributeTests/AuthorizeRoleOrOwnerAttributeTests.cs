using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Streetcode.DAL.Entities.Comment;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.WebApi.Attributes;
using System.Linq.Expressions;
using System.Security.Claims;
using Xunit;
using UserRole = Streetcode.DAL.Enums.UserRole;
using CommentEntity = Streetcode.DAL.Entities.Comment.Comment;

public class AuthorizeRoleOrOwnerAttributeTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryMock;
    private readonly Mock<ActionExecutionDelegate> _nextMock;

    public AuthorizeRoleOrOwnerAttributeTests()
    {
        _repositoryMock = new Mock<IRepositoryWrapper>();
        _nextMock = new Mock<ActionExecutionDelegate>();
    }

    // Helper: Create ActionExecutingContext
    private ActionExecutingContext CreateContext(
        ClaimsPrincipal user = null,
        object routeId = null,
        IServiceProvider serviceProvider = null)
    {
        var httpContext = new DefaultHttpContext
        {
            User = user,
            RequestServices = serviceProvider ?? new ServiceCollection().BuildServiceProvider()
        };

        var actionArguments = new Dictionary<string, object>();
        if (routeId != null)
        {
            actionArguments.Add("id", routeId);
        }

        return new ActionExecutingContext(
            new ActionContext(httpContext, new RouteData(), new ActionDescriptor()),
            new List<IFilterMetadata>(),
            actionArguments,
            new object());
    }

    // Helper: Create IServiceProvider with repository
    private IServiceProvider CreateServiceProvider(IRepositoryWrapper repository = null)
    {
        var services = new ServiceCollection();

        if (repository != null)
        {
            services.AddSingleton(repository);
        }

        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task OnActionExecutionAsync_UserInRole_ContinuesToNext()
    {
        // Arrange
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, UserRole.Admin.ToString()) }, "mock"));
        var context = CreateContext(user);

        var attribute = new AuthorizeRoleOrOwnerAttribute(UserRole.Admin);

        // Act
        await attribute.OnActionExecutionAsync(context, _nextMock.Object);

        // Assert
        _nextMock.Verify(next => next(), Times.Once);
        Assert.Null(context.Result); // Action should continue without interruption
    }

    [Fact]
    public async Task OnActionExecutionAsync_MissingRepository_ThrowsException()
    {
        // Arrange
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "123") }, "mock"));
        var context = CreateContext(user, routeId: 1, serviceProvider: CreateServiceProvider());

        var attribute = new AuthorizeRoleOrOwnerAttribute(UserRole.User);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => attribute.OnActionExecutionAsync(context, _nextMock.Object));
    }

    [Fact]
    public async Task OnActionExecutionAsync_UserIsOwner_ContinuesToNext()
    {
        // Arrange
        var userId = "123";
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) }, "mock"));
        var commentId = 1;

        _repositoryMock.Setup(repo => repo.CommentRepository.GetFirstOrDefaultAsync(
            It.IsAny<Expression<Func<CommentEntity, bool>>>(),
            It.IsAny<Func<IQueryable<CommentEntity>, IIncludableQueryable<CommentEntity, object>>>()))
            .ReturnsAsync(new Comment { Id = commentId, UserId = userId });

        var context = CreateContext(
            user,
            routeId: commentId,
            serviceProvider: CreateServiceProvider(_repositoryMock.Object));

        var attribute = new AuthorizeRoleOrOwnerAttribute(UserRole.User);

        // Act
        await attribute.OnActionExecutionAsync(context, _nextMock.Object);

        // Assert
        _nextMock.Verify(next => next(), Times.Once);
        Assert.Null(context.Result); // Action should continue without interruption
    }

    [Fact]
    public async Task OnActionExecutionAsync_InvalidCommentId_ReturnsBadRequest()
    {
        // Arrange
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "123") }, "mock"));
        var context = CreateContext(user, routeId: null);

        var attribute = new AuthorizeRoleOrOwnerAttribute(UserRole.User);

        // Act
        await attribute.OnActionExecutionAsync(context, _nextMock.Object);

        // Assert
        Assert.IsType<BadRequestObjectResult>(context.Result);
    }

    [Fact]
    public async Task OnActionExecutionAsync_UserNotOwner_ReturnsForbidden()
    {
        // Arrange
        var userId = "123";
        var otherUserId = "456";
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) }, "mock"));
        var commentId = 1;

        _repositoryMock.Setup(repo => repo.CommentRepository.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<CommentEntity, bool>>>(),
                It.IsAny<Func<IQueryable<CommentEntity>, IIncludableQueryable<CommentEntity, object>>>()))
            .ReturnsAsync(new Comment { Id = commentId, UserId = otherUserId });

        var context = CreateContext(
            user,
            routeId: commentId,
            serviceProvider: CreateServiceProvider(_repositoryMock.Object));

        var attribute = new AuthorizeRoleOrOwnerAttribute(UserRole.User);

        // Act
        await attribute.OnActionExecutionAsync(context, _nextMock.Object);

        // Assert
        Assert.IsType<ForbidResult>(context.Result);
        _nextMock.Verify(next => next(), Times.Never);
    }
}
