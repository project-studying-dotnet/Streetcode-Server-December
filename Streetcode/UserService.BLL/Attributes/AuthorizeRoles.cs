using Microsoft.AspNetCore.Authorization;
using System.Linq;
using UserService.DAL.Enums;

namespace UserService.BLL.Attributes
{
    public class AuthorizeRoles : AuthorizeAttribute
    {
        public AuthorizeRoles(params UserRole[] userRoles)
        {
            Roles = string.Join(",", userRoles.Select(r => r.ToString()).ToArray());
        }
    }
}
