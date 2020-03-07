using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MacSlopes.Models.PhotosViewModels;
using MacSlopes.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using PagedList.Core;

namespace MacSlopes.Controllers
{
    [Route("[controller]/[action]")]
    public class GalleryController : Controller
    {
        private readonly IPhoto _photoRepo;

        public GalleryController(IPhoto photoRepo)
        {
            _photoRepo = photoRepo;
        }

        [HttpGet]
        public IActionResult Index([FromQuery]int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var photos = _photoRepo.GetAllPhotos()
                .Select(photo => new PhotosIndexViewModel
                {
                    Id = photo.Id,
                    Name = photo.Name,
                    Description = photo.Description,
                    Category = photo.Category,
                    PhotoUrl = photo.PhotoUrl,
                    DateCreated = photo.DateCreated.ToLongDateString(),
                    FaceBookLink = photo.FaceBookLink,
                    InstagramLink = photo.InstagramLink,
                    TwitterLink = photo.TwitterLink
                });

            var model = new PhotosListViewModel
            {
                Photos = new PagedList<PhotosIndexViewModel>(photos, pageNumber, 12)
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult Search([FromQuery]string Search,[FromQuery]int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            if (String.IsNullOrWhiteSpace(Search))
            {
                return RedirectToAction(nameof(Index));
            }
            var photos = _photoRepo.SearchPhotos(Search)
                .Select(photo => new PhotosIndexViewModel
                {
                    Id = photo.Id,
                    Name = photo.Name,
                    Description = photo.Description,
                    Category = photo.Category,
                    PhotoUrl = photo.PhotoUrl,
                    DateCreated = photo.DateCreated.ToLongDateString(),
                    FaceBookLink = photo.FaceBookLink,
                    InstagramLink = photo.InstagramLink,
                    TwitterLink = photo.TwitterLink
                });

            var model = new PhotosListViewModel
            {
                Search = Search,
                Photos = new PagedList<PhotosIndexViewModel>(photos, pageNumber, 12)
            };
            return View(model);
        }
    }
}