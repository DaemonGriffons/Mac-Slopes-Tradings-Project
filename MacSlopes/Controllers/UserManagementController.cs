using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MacSlopes.Entities;
using MacSlopes.Models.UserManagementViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PagedList.Core;

namespace MacSlopes.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserManagementController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserManagementController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var users = _userManager.Users.Select(user => new UserManagementIndexViewModel
            {
                Id = user.Id,
                Username = user.UserName,
                Name = user.Name,
                Surname = user.Surname,
                ImageUrl = user.ImageUrl,
                DateRegistered = user.DateRegistered.ToLongDateString()
            });

            var model = new UserManagementListViewModel
            {
                Users = new PagedList<UserManagementIndexViewModel>(users, pageNumber, 12)
            };

            return View(model);
        }

        public async Task<IActionResult> UserDetails(string UserId)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var model = new UserDetailViewModel
            {
                Id = user.Id,
                Email = user.Email,
                ImageUrl = user.ImageUrl,
                Name = user.Name,
                Surname = user.Surname,
                PhoneNumber = user.PhoneNumber,
                Username = user.UserName,
                DateRegistered = user.DateRegistered.ToLongDateString()
            };
            return View(model);

        }

        [HttpGet]
        public IActionResult Search([FromQuery]string Search, [FromQuery]int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var users=_userManager.Users
                .Where(s=>s.UserName.Contains(Search)
                        || s.Name.Contains(Search)
                        || s.PhoneNumber.Contains(Search)
                        || s.Surname.Contains(Search)
                        || s.UserName.Contains(Search))
                .Select(user => new UserManagementIndexViewModel
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Name = user.Name,
                    Surname = user.Surname,
                    ImageUrl = user.ImageUrl,
                    DateRegistered = user.DateRegistered.ToLongDateString()
                });
            var model = new UserManagementListViewModel
            {
                Search=Search,
                Users = new PagedList<UserManagementIndexViewModel>(users, pageNumber, 12)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user==null)
            {
                return RedirectToAction(nameof(Index));
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult UserRoles()
        {
            return View();
        }
    }
}