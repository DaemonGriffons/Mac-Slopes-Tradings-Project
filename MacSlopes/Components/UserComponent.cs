using MacSlopes.Entities;
using MacSlopes.Models.ViewComponentsViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MacSlopes.Components
{
    public class UserComponent : ViewComponent
    {
        private readonly UserManager<User> _userManager;

        public UserComponent(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public IViewComponentResult Invoke()
        {
            var users = _userManager.Users;
            var model = new UsersViewComponentViewModel
            {
                UserCount = users.Count()
            };
            return View(model);
        }
    }
}
