using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Streetcode.BLL.DTO.Comment;
using UserRole = Streetcode.DAL.Enums.UserRole;

namespace Streetcode.WebApi.Attributes
{
    public class AuthorizeRoleOrOwnerAttribute : Attribute, IAsyncActionFilter
    {
        private readonly UserRole _requiredRole;

        public AuthorizeRoleOrOwnerAttribute(UserRole role)
        {
            _requiredRole = role;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var user = context.HttpContext.User;

            // Check if the user is authenticated
            if (!IsUserAuthenticated(user))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Check if the user has the required role
            if (IsUserInRole(user, _requiredRole))
            {
                await next();
                return;
            }

            // Get the repository
            var repository = context.HttpContext.RequestServices.GetService<IRepositoryWrapper>();

            if (repository == null)
            {
                throw new InvalidOperationException("Repository not found.");
            }

            // Get the user's name from the token
            if (!TryGetUserName(user, out var userNameFromToken))
            {
                context.Result = new ForbidResult();
                return;
            }

            // Check if the user is the owner of the object
            if (!await IsUserOwnerAsync(context, repository, userNameFromToken))
            {
                context.Result = new ForbidResult();
                return;
            }

            await next();
        }

        private static bool IsUserAuthenticated(ClaimsPrincipal user) =>
            user?.Identity?.IsAuthenticated == true;

        private static bool IsUserInRole(ClaimsPrincipal user, UserRole role) =>
            user?.IsInRole(role.ToString()) == true;

        private static bool TryGetUserName(ClaimsPrincipal user, out string userNameFromToken)
        {
            userNameFromToken = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return !string.IsNullOrEmpty(userNameFromToken);
        }

        private static async Task<bool> IsUserOwnerAsync(ActionExecutingContext context, IRepositoryWrapper repository, string userNameFromToken)
        {
            if (!context.ActionArguments.TryGetValue("updateCommentDto", out var updateCommentDtoObj) || updateCommentDtoObj == null)
            {
                context.Result = new BadRequestObjectResult("Invalid input data");
                return false;
            }

            var updateCommentDto = updateCommentDtoObj as UpdateCommentDto;
            if (updateCommentDto == null)
            {
                return false;
            }

            var createdDate = updateCommentDto.CreatedDate;

            // Check if a comment exists with the given created date and username
            var comment = await repository.CommentRepository
                .GetFirstOrDefaultAsync(c => c.CreatedDate == createdDate && c.UserName == userNameFromToken);

            return comment != null;
        }
    }
}