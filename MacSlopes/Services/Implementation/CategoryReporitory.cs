using MacSlopes.Entities;
using MacSlopes.Entities.Data;
using MacSlopes.Services.Abstract;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MacSlopes.Services.Implementation
{
    public class CategoryReporitory : ICategoryReporitory
    {
        private readonly DataContext _context;

        public CategoryReporitory(DataContext context)
        {
            _context = context;
        }
        public IQueryable<Category> GetCategories()=>
            _context.Categories.OrderBy(c=>c.Name);

        public Category GetCategory(string Id)=> 
            _context.Categories.SingleOrDefault(x => x.Id.Equals(Id,StringComparison.OrdinalIgnoreCase));

        public void AddCategory(Category category) => 
            _context.Categories.Add(category);

        public void UpdateCategory(Category category) => _context.Categories.Update(category);

        public void DeleteCategory(Category category) => _context.Categories.Remove(category);

        public async Task<bool> SaveChangesAsync()=> await _context.SaveChangesAsync() > 0 ? true : false;

        public bool VerifyName(string name)=> _context.Categories.Any(x => x.Name.Equals(name,StringComparison.OrdinalIgnoreCase));

        public IQueryable<Category> Search(string Query) =>
            _context.Categories.Where(x => x.Name.Contains(Query, StringComparison.OrdinalIgnoreCase));
    }
}
