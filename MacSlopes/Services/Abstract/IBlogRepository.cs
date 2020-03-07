using MacSlopes.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace MacSlopes.Services.Abstract
{
    public interface IBlogRepository
    {
        IQueryable<Post> GetPosts();
        IQueryable<Post> GetCategoryPosts(string category);
        IQueryable<Post> SearchPosts(string query);

        //IQueryable<Post> GetPostCategory(string Id);

        Post GetPost(string Id);

        Post GetPostBySlug(string slug);

        string CreateSlug(string title);
        string GetLink(string Slug);
        string GetEncodedLink(string Slug);
        void AddPost(Post post);
        void UpdatePost(Post post);
        void RemovePost(string Id);
        void RemoveComment(Comment comment);

        Task<bool> SaveChangesAsync();
    }
}
