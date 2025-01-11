using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Comment;
using Streetcode.DAL.Entities.Comment;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.WebApi.Attributes;
using Xunit;
using UserRole = Streetcode.DAL.Enums.UserRole;

public class AuthorizeRoleOrOwnerAttributeTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryMock;
    private readonly Mock<ActionExecutionDelegate> _nextMock;
    private readonly AuthorizeRoleOrOwnerAttribute _attribute;

    public AuthorizeRoleOrOwnerAttributeTests()
    {
        _repositoryMock = new Mock<IRepositoryWrapper>();
        _nextMock = new Mock<ActionExecutionDelegate>();
        _attribute = new AuthorizeRoleOrOwnerAttribute(UserRole.Admin);
    }

    [Fact]
    public async Task OnActionExecutionAsync_UserNotAuthenticated_ReturnsUnauthorized()
    {
        // Arrange
        var context = CreateActionExecutingContext(isAuthenticated: false);

        // Act
        await _attribute.OnActionExecutionAsync(context, _nextMock.Object);

        // Assert
        Assert.IsType<UnauthorizedResult>(context.Result);
    }

    [Fact]
    public async Task OnActionExecutionAsync_UserInRequiredRole_ContinuesExecution()
    {
        // Arrange
        var context = CreateActionExecutingContext(isAuthenticated: true, roles: new[] { UserRole.Admin.ToString() });

        // Act
        await _attribute.OnActionExecutionAsync(context, _nextMock.Object);

        // Assert
        _nextMock.Verify(next => next(), Times.Once);
        Assert.Null(context.Result);
    }

    [Fact]
    public async Task OnActionExecutionAsync_UserIsOwner_ContinuesExecution()
    {
        // Arrange
        var updateCommentDto = new UpdateCommentDto
        {
            CreatedDate = DateTime.UtcNow,
            UserFullName = "testUser"
        };

        var context = CreateActionExecutingContext(
            isAuthenticated: true,
            roles: new[] { UserRole.User.ToString() },
            actionArguments: new Dictionary<string, object> { { "updateCommentDto", updateCommentDto } },
            userNameFromToken: "testUser"
        );

        _repositoryMock.Setup(repo => repo.CommentRepository.GetFirstOrDefaultAsync(
            It.IsAny<Expression<Func<Comment, bool>>>(),
            It.IsAny<Func<IQueryable<Comment>, IIncludableQueryable<Comment, object>>>()))
            .ReturnsAsync(new Comment());

        // Act
        await _attribute.OnActionExecutionAsync(context, _nextMock.Object);

        // Assert
        _nextMock.Verify(next => next(), Times.Once);
        Assert.Null(context.Result);
    }

    [Fact]
    public async Task OnActionExecutionAsync_UserNotOwner_ReturnsForbidden()
    {
        // Arrange
        var updateCommentDto = new UpdateCommentDto
        {
            CreatedDate = DateTime.UtcNow,
            UserFullName = "otherUser"
        };

        var context = CreateActionExecutingContext(
            isAuthenticated: true,
            roles: new[] { UserRole.User.ToString() },
            actionArguments: new Dictionary<string, object> { { "updateCommentDto", updateCommentDto } },
            userNameFromToken: "testUser"
        );

        _repositoryMock.Setup(repo => repo.CommentRepository.GetFirstOrDefaultAsync(
            It.IsAny<Expression<Func<Comment, bool>>>(),
            It.IsAny<Func<IQueryable<Comment>, IIncludableQueryable<Comment, object>>>()))
            .ReturnsAsync((Comment)null);

        // Act
        await _attribute.OnActionExecutionAsync(context, _nextMock.Object);

        // Assert
        Assert.IsType<ForbidResult>(context.Result);
    }

    private ActionExecutingContext CreateActionExecutingContext(
        bool isAuthenticated,
        string[] roles = null,
        Dictionary<string, object> actionArguments = null,
        string userNameFromToken = null
    )
    {
        var userClaims = new List<Claim>();

        if (isAuthenticated)
        {
            userClaims.Add(new Claim(ClaimTypes.NameIdentifier, userNameFromToken ?? "defaultUser"));
        }

        if (roles != null)
        {
            foreach (var role in roles)
            {
                userClaims.Add(new Claim(ClaimTypes.Role, role));
            }
        }

        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(userClaims, isAuthenticated ? "TestAuthType" : null))
        };

        httpContext.RequestServices = Mock.Of<IServiceProvider>(sp =>
            sp.GetService(typeof(IRepositoryWrapper)) == _repositoryMock.Object);

        return new ActionExecutingContext(
            new ActionContext
            {
                HttpContext = httpContext,
                RouteData = new Microsoft.AspNetCore.Routing.RouteData(),
                ActionDescriptor = new Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor()
            },
            new List<IFilterMetadata>(),
            actionArguments ?? new Dictionary<string, object>(),
            controller: null
        );
    }
}