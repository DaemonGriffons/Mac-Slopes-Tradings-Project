using MacSlopes.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MacSlopes.Models.UserManagementViewModels
{
    public class UserRoleViewModel
    {
        public UserRoleViewModel()
        {
            Users = new List<User>();
        }

        public string RoleId { get; set; }

        public string UserId { get; set; }

        public List<User> Users { get; set; }
    }
}
