using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MacSlopes.Models.UserManagementViewModels
{
    public class UserManagementListViewModel
    {
        public string Search { get; set; }

        public PagedList<UserManagementIndexViewModel> Users { get; set; }
    }
}
