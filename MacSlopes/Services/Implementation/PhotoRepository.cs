using MacSlopes.Entities;
using MacSlopes.Entities.Data;
using MacSlopes.Services.Abstract;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace MacSlopes.Services.Implementation
{
    public class PhotoRepository : IPhoto
    {
        private readonly DataContext _context;

        public PhotoRepository(DataContext context)
        {
            _context = context;
        }
        public Task AddPhoto(Photo photo)
        {
           return _context.Photos.AddAsync(photo);
        }

        public IQueryable<Photo> GetAllPhotos()
        {
            return _context.Photos.OrderByDescending(x=>x.DateCreated);
        }

        public Task<Photo> GetPhoto(string Id)
        {
            return _context.Photos.FirstOrDefaultAsync(x => x.Id.Equals(Id));
        }

        public Task RemovePhoto(Photo photo)
        {
            return Task.FromResult(_context.Photos.Remove(photo));
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }

        public IQueryable<Photo> SearchPhotos(string search)
        {
            return _context.Photos.Where(p => p.Name.Contains(search)
                                    || p.Description.Contains(search)
                                    || p.Category.Contains(search)
                                    || p.PhotoUrl.Contains(search)
                                    || p.FaceBookLink.Contains(search)
                                    || p.InstagramLink.Contains(search)
                                    || p.TwitterLink.Contains(search));
        }

        public Task UpdatePhoto(Photo photo)
        {
            return Task.FromResult(_context.Photos.Update(photo));
        }
    }
}
