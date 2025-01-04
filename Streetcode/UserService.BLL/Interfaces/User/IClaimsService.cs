using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using UserEntity = UserService.DAL.Entities.Users.User;

namespace UserService.BLL.Interfaces.User
{
    public interface IClaimsService
    {
        Task<List<Claim>> CreateClaimsAsync(UserEntity user,string sessionId);
    }
}
