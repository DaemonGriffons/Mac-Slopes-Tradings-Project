using MacSlopes.Models.ViewComponentsViewModels;
using MacSlopes.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MacSlopes.Components
{
    public class PostComponent : ViewComponent
    {
        private readonly IBlogRepository _repository;

        public PostComponent(IBlogRepository repository)
        {
            _repository = repository;
        }

        public IViewComponentResult Invoke()
        {
            var posts = _repository.GetPosts();

            var model = new UsersViewComponentViewModel
            {
                UserCount = posts.Count()
            };

            return View(model);
        }
    }
}
