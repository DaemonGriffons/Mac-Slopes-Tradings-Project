using MacSlopes.Models.PostsViewModel;
using MacSlopes.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MacSlopes.Components
{
    public class BlogComponent :ViewComponent
    {
        private readonly IBlogRepository _repository;

        public BlogComponent(IBlogRepository repository)
        {
            _repository = repository;
        }

        public IViewComponentResult Invoke()
        {
            var host = Request.Scheme + "://" + Request.Host;
            var posts = _repository.GetPosts().Select(post => new PostsIndexViewModel
            {
                Author = post.Author,
                Title = post.Title,
                Description = post.Description,
                ImageUrl = post.ImageUrl,
                Category=post.CategoryId,
                Link = host + _repository.GetLink(post.Slug),
                DatePublished = post.DateCreated,
                CommentCount = post.Comments.Count
            }).OrderByDescending(x => x.DatePublished).Take(10).ToList();

            return View(posts);
        }
    }
}
