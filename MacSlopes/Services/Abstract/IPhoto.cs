using MacSlopes.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace MacSlopes.Services.Abstract
{
    public interface IPhoto
    {
        Task AddPhoto(Photo photo);
        Task UpdatePhoto(Photo photo);
        Task RemovePhoto(Photo photo);

        IQueryable<Photo> GetAllPhotos();
        IQueryable<Photo> SearchPhotos(string search);

        Task<Photo> GetPhoto(string Id);

        Task<bool> SaveChangesAsync();
    }
}
