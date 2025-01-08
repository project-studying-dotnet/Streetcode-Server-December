using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using UserRole = Streetcode.DAL.Enums.UserRole;

namespace Streetcode.WebApi.Attributes
{
    public class AuthorizeRoleOrOwnerAttribute : Attribute, IAsyncActionFilter
    {
        private readonly UserRole _role;

        public AuthorizeRoleOrOwnerAttribute(UserRole role)
        {
            _role = role;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var user = context.HttpContext.User;

            if (!IsUserAuthenticated(user))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (IsUserInRole(user, _role))
            {
                await next();
                return;
            }

            if (!TryGetCommentId(context, out var commentId))
            {
                context.Result = new BadRequestObjectResult("Invalid comment ID");
                return;
            }

            if (!TryGetUserName(user, out var userName))
            {
                context.Result = new ForbidResult();
                return;
            }

            var repository = context.HttpContext.RequestServices.GetService<IRepositoryWrapper>();

            if (repository == null)
            {
                throw new InvalidOperationException("IRepositoryWrapper is not configured.");
            }

            if (!await IsUserOwner(repository, commentId, userName))
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

        private static bool TryGetCommentId(ActionExecutingContext context, out int commentId)
        {
            commentId = 0;
            return context.ActionArguments.TryGetValue("id", out var value) && value is int id && (commentId = id) > 0;
        }

        private static bool TryGetUserName(ClaimsPrincipal user, out string userName)
        {
            userName = user?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            return !string.IsNullOrEmpty(userName);
        }

        private static async Task<bool> IsUserOwner(IRepositoryWrapper repository, int commentId, string userName)
        {
            var comment = await repository.CommentRepository
                .GetFirstOrDefaultAsync(c => c.Id == commentId);

            return comment != null && comment.UserName == userName;
        }
    }
}
