using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MacSlopes.Extensions;
using MacSlopes.Services.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace MacSlopes.Controllers
{
    public class SharedController : Controller
    {
        private readonly IBlogRepository _repository;

        public SharedController(IBlogRepository repository)
        {
            _repository = repository;
        }
        [Route("/robots.txt")]
        public string RobotsTxt()
        {
            string host = Request.Scheme + "://" + Request.Host;
            var sb = new StringBuilder();
            sb.AppendLine("User-agent: *");
            sb.AppendLine("Disallow:");
            sb.AppendLine($"sitemap: {host}/sitemap.xml");

            return sb.ToString();
        }

        [Route("/sitemap")]
        public IActionResult Sitemap()
        {

            string host = Request.Scheme + "://" + Request.Host;
            var about = host + "/Home/About";
            var contact = host + "/Home/Contact";
            var gallery = host + "/Home/Gallery";
            var services = host + "/Home/Services";
            var blog = host + "/Blog/Index";
            var siteMapBuilder = new SitemapBuilder();
            siteMapBuilder.AddUrl(host, modified: DateTime.UtcNow, changeFrequency: ChangeFrequency.Never, priority: 1.0);
            siteMapBuilder.AddUrl(about, modified: DateTime.UtcNow, changeFrequency: ChangeFrequency.Never, priority: 1.0);
            siteMapBuilder.AddUrl(contact, modified: DateTime.UtcNow, changeFrequency: ChangeFrequency.Never, priority: 1.0);
            siteMapBuilder.AddUrl(services, modified: DateTime.UtcNow, changeFrequency: ChangeFrequency.Never, priority: 1.0);
            if (_repository.GetPosts().Any())
            {
                foreach (var post in _repository.GetPosts())
                {
                    siteMapBuilder.AddUrl(host + _repository.GetLink(post.Slug), modified: post.DateCreated,
                        changeFrequency: ChangeFrequency.Weekly, priority: 0.9);
                }
            }

            string xml = siteMapBuilder.ToString();
            return Content(xml, "text/xml");
        }
    }
}