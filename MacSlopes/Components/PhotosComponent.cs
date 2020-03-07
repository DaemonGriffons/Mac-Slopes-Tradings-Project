using MacSlopes.Models.ViewComponentsViewModels;
using MacSlopes.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MacSlopes.Components
{
    [ViewComponent]
    public class PhotosComponent : ViewComponent
    {
        private readonly IPhoto _photos;

        public PhotosComponent(IPhoto photos)
        {
            _photos = photos;
        }

        public IViewComponentResult Invoke()
        {
            var allphotos = _photos.GetAllPhotos();

            var model = new UsersViewComponentViewModel
            {
                UserCount = allphotos.Count()
            };

            return View(model);
        }
    }
}
