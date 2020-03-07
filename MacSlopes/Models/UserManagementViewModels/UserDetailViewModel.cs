using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MacSlopes.Models.UserManagementViewModels
{
    public class UserDetailViewModel
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string Name { get; set; }


        public string Surname { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string DateRegistered { get; set; }


        public string ImageUrl { get; set; }
    }
}
