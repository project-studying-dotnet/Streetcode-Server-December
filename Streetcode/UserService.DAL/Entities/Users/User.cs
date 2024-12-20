using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.DAL.Entities.Users
{
    [CollectionName("Users")]
    public class User : MongoIdentityUser<Guid>
    {
        public string FullName { get; set; }
    }
}
