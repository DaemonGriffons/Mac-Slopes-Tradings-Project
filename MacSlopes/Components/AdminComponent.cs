using MacSlopes.Entities;
using MacSlopes.Models.AdminViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MacSlopes.Components
{
    public class AdminComponent : ViewComponent
    {
        private readonly UserManager<User> _userManager;

        public AdminComponent(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(UserClaimsPrincipal);

            var model = new AdminIndexViewModel
            {
                Name = user.Name,
                Surname = user.Surname,
                ImageUrl = user.ImageUrl
            };

            return View(model);
        }
    }
}
