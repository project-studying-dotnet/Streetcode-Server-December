using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCore.Identity.Mongo.Model;

namespace UserService.DAL.Entities.Users
{
    // [CollectionName("Users")]
    public class User : MongoUser
    {
        public string FullName { get; set; }
    }
}
