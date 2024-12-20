using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.DAL.Entities.Roles
{
    [CollectionName("Roles")]
    public class Role : MongoIdentityRole<Guid>
    {
    }
}
