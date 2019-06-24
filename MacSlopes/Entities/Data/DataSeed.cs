using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MacSlopes.Entities.Data
{
    public class DataSeed
    {
        private readonly DataContext context;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public DataSeed(DataContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public void SeedData()
        {
            context.Database.EnsureCreated();
            if (!context.Categories.Any())
            {
                var cats = new List<Category>()
            {
                new Category
                {
                    Id=Guid.NewGuid().ToString().Replace("-",string.Empty).ToLowerInvariant(),
                    Name="Programming"
                },
                new Category
                {
                    Id=Guid.NewGuid().ToString().Replace("-",string.Empty).ToLowerInvariant(),
                    Name="Security"
                },
                new Category
                {
                    Id=Guid.NewGuid().ToString().Replace("-",string.Empty).ToLowerInvariant(),
                    Name="Blogging"
                },
                new Category
                {
                    Id=Guid.NewGuid().ToString().Replace("-",string.Empty).ToLowerInvariant(),
                    Name="Ethical Hacking"
                },
                new Category
                {
                    Id=Guid.NewGuid().ToString().Replace("-",string.Empty).ToLowerInvariant(),
                    Name="Anonymous"
                },
                new Category
                {
                    Id=Guid.NewGuid().ToString().Replace("-",string.Empty).ToLowerInvariant(),
                    Name="Technology"
                },
                new Category
                {
                    Id=Guid.NewGuid().ToString().Replace("-",string.Empty).ToLowerInvariant(),
                    Name="Finance"
                },
            };

                context.Categories.AddRange(cats);
                context.SaveChanges();
            }
            
            var AdminRole = new IdentityRole("Admin");
            if (!roleManager.Roles.Any())
            {
                roleManager.CreateAsync(AdminRole).GetAwaiter().GetResult();
            }

            if (!userManager.Users.Any())
            {
                var adminUser = new User
                {
                    Name = "Daemon",
                    Surname="Griffons",
                    UserName = "Administrator",
                    Email = "daemongriffons@gmail.com",
                    EmailConfirmed = true,
                    PhoneNumber = "0812189106",
                    DateRegistered = DateTime.MinValue,
                    PhoneNumberConfirmed = true
                };

                userManager.CreateAsync(adminUser, "MyPassword@123").GetAwaiter().GetResult();
                userManager.AddToRoleAsync(adminUser, AdminRole.Name).GetAwaiter().GetResult();
            }

        }
    }
}
