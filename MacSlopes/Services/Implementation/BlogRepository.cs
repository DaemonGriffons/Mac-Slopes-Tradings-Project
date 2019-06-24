using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MacSlopes.Entities;
using MacSlopes.Entities.Data;
using MacSlopes.Services.Abstract;
using Microsoft.EntityFrameworkCore;
using Remotion.Linq.Clauses;

namespace MacSlopes.Services.Implementation
{
    public class BlogRepository : IBlogRepository
    {
        private readonly DataContext _context;
        private readonly IFileManager _fileManager;

        public BlogRepository(DataContext context, IFileManager fileManager)
        {
            _context = context;
            _fileManager = fileManager;
        }

        public void AddPost(Post post)
        {
            _context.Posts.Add(post);
        }

        public IQueryable<Post> GetCategoryPosts(string category)
        {
            return _context.Posts
                .Include(x => x.Categories.Where(c => c.Name.Equals(category, StringComparison.OrdinalIgnoreCase)));
        }


        public Post GetPost(string Id)
        {
            //This is what i tried to do but it is refusing
            //please help!!
            // can you push your code to a repo and i have a loo at it tomorrow? 
            // I have homeworks to complete tonight,
            // did youuse a generic repo for this?
            //wouldn't say generic but problem specific!
            //I will push it and i'll work on react for tonigth.. i
//sure case send me the link and i will check it clearly on my machine. you complicated things that were suppose to be simple.
//i was learning from a youtube tutorial that was very simplified and i needed to enhance a few things that were real world aplicable
            return _context.Posts.Include(c=>c.Categories)
                .Include(p => p.MainComments)
                .ThenInclude(p => p.SubComments)
                 .FirstOrDefault(options => options.Id.Equals(Id));

        }

        public IQueryable<Post> GetPosts()
        {
            return _context.Posts
                .Include(x => x.MainComments)
                .OrderByDescending(x => x.DatePublished);
        }

        public void RemovePost(string Id)
        {
            _fileManager.RemoveImage(GetPost(Id).ImageUrl);
            _context.Posts.Remove(GetPost(Id));
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }

        public void UpdatePost(Post post)
        {
            _context.Posts.Update(post);
        }

        public void AddSubComment(SubComment subComment)
        {
            _context.SubComments.Add(subComment);
        }

        public IQueryable<Post> SearchPosts(string query)
        {
            return _context.Posts
                .Include(x => x.MainComments)
                .Include(q=>q.Categories)
                .Where(x => x.Title.Contains(query, StringComparison.OrdinalIgnoreCase)
                || x.Body.Contains(query, StringComparison.OrdinalIgnoreCase)
                || x.Description.Contains(query, StringComparison.OrdinalIgnoreCase)
                || x.Author.Contains(query, StringComparison.OrdinalIgnoreCase));
        }

        public Post GetPostBySlug(string slug)
        {
            return _context.Posts.Include(s=>s.Categories)
                .Include(x => x.MainComments)
                 .ThenInclude(c => c.SubComments)
                 .FirstOrDefault(a => a.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));
        }

        public string CreateSlug(string title)
        {
            title = title.ToLowerInvariant().Replace(" ", "-");
            title = RemoveDiacritics(title);
            title = RemoveReservedUrlCharacters(title);

            return title.ToLowerInvariant();
        }

        public string GetLink(string Slug)
        {
            return $"/Blog/{Slug}/";
        }

        public string GetEncodedLink(string Slug)
        {
            return $"/Blog/{System.Net.WebUtility.UrlEncode(Slug)}/";
        }

        //public IQueryable<Post> GetPostCategory(string Id)
        //{
        //    var query = (from p in _context.Posts
        //                 join c in _context.Categories on p.CategoryId equals c.Id
        //                 where p.Id == Id
        //                 select new
        //                 {
        //                     Post = p,
        //                     CategoryName = c.Name
        //                 });
        //    return query.AsQueryable<Post>();
        //}

        private static string RemoveReservedUrlCharacters(string text)
        {
            var reservedCharacters = new List<string> { "!", "#", "$", "&", "'", "(", ")", "*", ",", "/", ":", ";", "=", "?", "@", "[", "]", "\"", "%", ".", "<", ">", "\\", "^", "_", "'", "{", "}", "|", "~", "`", "+" };

            foreach (var chr in reservedCharacters)
            {
                text = text.Replace(chr, "");
            }

            return text;
        }

        private static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
