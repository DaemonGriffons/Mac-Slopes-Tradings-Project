using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MacSlopes.Models.PhotosViewModels
{
    public class PhotosListViewModel
    {
        public string Search { get; set; }

        public PagedList<PhotosIndexViewModel> Photos { get; set; }
    }
}
