using System;
using System.Collections.Generic;

using AspNetCore.Identity.Mongo.Model;
using UserService.BLL.DTO.User;

namespace UserService.DAL.Entities.Users
{
    public class User : MongoUser
    {
        public string FullName { get; set; }
        public List<RefreshTokenInfo> RefreshTokens { get; set; } = new List<RefreshTokenInfo>();
    }
}
