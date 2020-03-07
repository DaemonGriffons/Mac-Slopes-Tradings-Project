using MacSlopes.Entities;
using MacSlopes.Entities.Data;
using MacSlopes.Services.Abstract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                .Include(u=>u.User)
                .Where(cat => cat.CategoryId.Equals(category));
        }


        public Post GetPost(string Id)
        {
            return _context.Posts
                .Include(u=>u.User)
                .Include(p => p.Comments)
                .FirstOrDefault(options => options.Id.Equals(Id));

        }

        public IQueryable<Post> GetPosts()
        {
            return _context.Posts
                .Include(u=>u.User)
                .OrderBy(x=>x.DateCreated)
                .OrderByDescending(x => x.DateCreated);
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


        public IQueryable<Post> SearchPosts(string query)
        {
            return _context.Posts
                .Include(u=>u.User)
                .Where(x => x.Title.Contains(query)
                || x.Body.Contains(query)
                || x.Description.Contains(query)
                || x.CategoryId.Contains(query)
                || x.Author.Contains(query));
        }

        public Post GetPostBySlug(string slug)
        {
            return _context.Posts
                .Include(u=>u.User)
                .Include(x => x.Comments)
                 .FirstOrDefault(a => a.Slug.Equals(slug));
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

        public void RemoveComment(Comment comment)
        {
            _context.Comments.Remove(comment);
        }
    }
}
