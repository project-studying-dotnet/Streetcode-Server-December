using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using UserService.DAL.Entities.Roles;
using UserService.DAL.Entities.Users;
using UserService.DAL.Enums;

namespace UserService.WebApi.Extensions;

public static class SeedingLocalExtension
{
    public static async Task SeedDataAsync(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

            if (!roleManager.Roles.Any())
            {
                var adminRole = new Role
                {
                    Id = ObjectId.GenerateNewId(),
                    Name = UserRole.Admin.ToString(),
                };
                await roleManager.CreateAsync(adminRole);

                var userRole = new Role
                {
                    Id = ObjectId.GenerateNewId(),
                    Name = UserRole.User.ToString(),
                };
                await roleManager.CreateAsync(userRole);
                
            }

            if (!userManager.Users.Any())
            {
                var admin = new User
                {
                    Id = ObjectId.GenerateNewId(),
                    UserName = "admin",
                    Email = "admin@gmail.com",
                };
                await userManager.CreateAsync(admin, "qwertA*");
                await userManager.AddToRoleAsync(admin, "Admin");
                
                var user = new User
                {
                    Id = ObjectId.GenerateNewId(),
                    UserName = "user",
                    Email = "user@gmail.com"
                };
                await userManager.CreateAsync(user, "qwertA*");
                await userManager.AddToRoleAsync(user, "User");
                
            }
        }

    }
}