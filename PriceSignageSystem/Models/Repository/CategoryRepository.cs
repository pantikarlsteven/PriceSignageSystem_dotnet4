using PriceSignageSystem.Models.DatabaseContext;
using PriceSignageSystem.Models.Interface;
using System.Collections.Generic;

namespace PriceSignageSystem.Models.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _db;

        public CategoryRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public IEnumerable<Category> GetAllCategories()
        {
            var data = _db.Categories;
            return data;
        }
    }
}