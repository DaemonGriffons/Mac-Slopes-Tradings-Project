using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

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
                    Name="Programming"
                },
                new Category
                {

                    Name="Security"
                },
                new Category
                {
                    Name="Blogging"
                },
                new Category
                {
                    Name="Ethical Hacking"
                },
                new Category
                {
                    Name="Anonymous"
                },
                new Category
                {
                    Name="Technology"
                },
                new Category
                {
                    Name="Finance"
                },
                new Category
                {
                    Name="Programming"
                },
                new Category
                {

                    Name="Security"
                },
                new Category
                {
                    Name="Blogging"
                },
                new Category
                {
                    Name="Ethical Hacking"
                },
                new Category
                {
                    Name="Anonymous"
                },
                new Category
                {
                    Name="Technology"
                },
                new Category
                {
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

            //var photos = new List<Photo>()
            //{
            //    new Photo
            //    {
            //        Id=Guid.NewGuid().ToString().Replace("-",string.Empty),
            //        Name="Creative Designs",
            //        Description="This is a graphics design group that is based in polokwane with products ranging from logo design to interior design and decor",
            //        Category="Beauty",
            //        PhotoUrl="mac_slopes_trading_img_05072019230808.jpg",
            //        FaceBookLink="Creative_Designz_za",
            //        InstagramLink="Creative_Designz_za",
            //        TwitterLink="Creative_Designz_za",
            //    },
            //    new Photo
            //    {
            //        Id=Guid.NewGuid().ToString().Replace("-",string.Empty),
            //        Name="Creative Designs",
            //        Description="This is a graphics design group that is based in polokwane with products ranging from logo design to interior design and decor",
            //        Category="Beauty",
            //        PhotoUrl="mac_slopes_trading_img_05072019230808.jpg",
            //        FaceBookLink="Creative_Designz_za",
            //        InstagramLink="Creative_Designz_za",
            //        TwitterLink="Creative_Designz_za",
            //    },
            //    new Photo
            //    {
            //        Id=Guid.NewGuid().ToString().Replace("-",string.Empty),
            //        Name="Creative Designs",
            //        Description="This is a graphics design group that is based in polokwane with products ranging from logo design to interior design and decor",
            //        Category="Beauty",
            //        PhotoUrl="mac_slopes_trading_img_05072019230808.jpg",
            //        FaceBookLink="Creative_Designz_za",
            //        InstagramLink="Creative_Designz_za",
            //        TwitterLink="Creative_Designz_za",
            //    },
            //    new Photo
            //    {
            //        Id=Guid.NewGuid().ToString().Replace("-",string.Empty),
            //        Name="Creative Designs",
            //        Description="This is a graphics design group that is based in polokwane with products ranging from logo design to interior design and decor",
            //        Category="Beauty",
            //        PhotoUrl="mac_slopes_trading_img_05072019230808.jpg",
            //        FaceBookLink="Creative_Designz_za",
            //        InstagramLink="Creative_Designz_za",
            //        TwitterLink="Creative_Designz_za",
            //    },
            //    new Photo
            //    {
            //        Id=Guid.NewGuid().ToString().Replace("-",string.Empty),
            //        Name="Creative Designs",
            //        Description="This is a graphics design group that is based in polokwane with products ranging from logo design to interior design and decor",
            //        Category="Beauty",
            //        PhotoUrl="mac_slopes_trading_img_05072019230808.jpg",
            //        FaceBookLink="Creative_Designz_za",
            //        InstagramLink="Creative_Designz_za",
            //        TwitterLink="Creative_Designz_za",
            //    },
            //    new Photo
            //    {
            //        Id=Guid.NewGuid().ToString().Replace("-",string.Empty),
            //        Name="Creative Designs",
            //        Description="This is a graphics design group that is based in polokwane with products ranging from logo design to interior design and decor",
            //        Category="Beauty",
            //        PhotoUrl="mac_slopes_trading_img_05072019230808.jpg",
            //        FaceBookLink="Creative_Designz_za",
            //        InstagramLink="Creative_Designz_za",
            //        TwitterLink="Creative_Designz_za",
            //    },
            //    new Photo
            //    {
            //        Id=Guid.NewGuid().ToString().Replace("-",string.Empty),
            //        Name="Creative Designs",
            //        Description="This is a graphics design group that is based in polokwane with products ranging from logo design to interior design and decor",
            //        Category="Beauty",
            //        PhotoUrl="mac_slopes_trading_img_05072019230808.jpg",
            //        FaceBookLink="Creative_Designz_za",
            //        InstagramLink="Creative_Designz_za",
            //        TwitterLink="Creative_Designz_za",
            //    },
            //    new Photo
            //    {
            //        Id=Guid.NewGuid().ToString().Replace("-",string.Empty),
            //        Name="Creative Designs",
            //        Description="This is a graphics design group that is based in polokwane with products ranging from logo design to interior design and decor",
            //        Category="Beauty",
            //        PhotoUrl="mac_slopes_trading_img_05072019230808.jpg",
            //        FaceBookLink="Creative_Designz_za",
            //        InstagramLink="Creative_Designz_za",
            //        TwitterLink="Creative_Designz_za",
            //    },
            //    new Photo
            //    {
            //        Id=Guid.NewGuid().ToString().Replace("-",string.Empty),
            //        Name="Creative Designs",
            //        Description="This is a graphics design group that is based in polokwane with products ranging from logo design to interior design and decor",
            //        Category="Beauty",
            //        PhotoUrl="mac_slopes_trading_img_05072019230808.jpg",
            //        FaceBookLink="Creative_Designz_za",
            //        InstagramLink="Creative_Designz_za",
            //        TwitterLink="Creative_Designz_za",
            //    },
            //    new Photo
            //    {
            //        Id=Guid.NewGuid().ToString().Replace("-",string.Empty),
            //        Name="Creative Designs",
            //        Description="This is a graphics design group that is based in polokwane with products ranging from logo design to interior design and decor",
            //        Category="Beauty",
            //        PhotoUrl="mac_slopes_trading_img_05072019230808.jpg",
            //        FaceBookLink="Creative_Designz_za",
            //        InstagramLink="Creative_Designz_za",
            //        TwitterLink="Creative_Designz_za",
            //    },
            //    new Photo
            //    {
            //        Id=Guid.NewGuid().ToString().Replace("-",string.Empty),
            //        Name="Creative Designs",
            //        Description="This is a graphics design group that is based in polokwane with products ranging from logo design to interior design and decor",
            //        Category="Beauty",
            //        PhotoUrl="mac_slopes_trading_img_05072019230808.jpg",
            //        FaceBookLink="Creative_Designz_za",
            //        InstagramLink="Creative_Designz_za",
            //        TwitterLink="Creative_Designz_za",
            //    },
            //    new Photo
            //    {
            //        Id=Guid.NewGuid().ToString().Replace("-",string.Empty),
            //        Name="Creative Designs",
            //        Description="This is a graphics design group that is based in polokwane with products ranging from logo design to interior design and decor",
            //        Category="Beauty",
            //        PhotoUrl="mac_slopes_trading_img_05072019230808.jpg",
            //        FaceBookLink="Creative_Designz_za",
            //        InstagramLink="Creative_Designz_za",
            //        TwitterLink="Creative_Designz_za",
            //    },
            //    new Photo
            //    {
            //        Id=Guid.NewGuid().ToString().Replace("-",string.Empty),
            //        Name="Creative Designs",
            //        Description="This is a graphics design group that is based in polokwane with products ranging from logo design to interior design and decor",
            //        Category="Beauty",
            //        PhotoUrl="mac_slopes_trading_img_05072019230808.jpg",
            //        FaceBookLink="Creative_Designz_za",
            //        InstagramLink="Creative_Designz_za",
            //        TwitterLink="Creative_Designz_za",
            //    },

            //};


            if (!context.Photos.Any())
            {
                context.Photos.Add(new Photo
                {
                    Id = Guid.NewGuid().ToString().Replace("-", string.Empty),
                    Name = "Creative Designs",
                    Description = "This is a graphics design group that is based in polokwane with products ranging from logo design to interior design and decor",
                    Category = "Beauty",
                    PhotoUrl = "mac_slopes_trading_img_11072019182609.jpg",
                    FaceBookLink = "Creative_Designz_za",
                    InstagramLink = "Creative_Designz_za",
                    TwitterLink = "Creative_Designz_za",
                });
                context.SaveChangesAsync().GetAwaiter().GetResult();
            }
        }
    }
}
