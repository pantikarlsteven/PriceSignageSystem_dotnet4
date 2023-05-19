using PriceSignageSystem.Models.DatabaseContext;
using PriceSignageSystem.Models.Dto;
using PriceSignageSystem.Models.Interface;
using System.Collections.Generic;

namespace PriceSignageSystem.Models.Repository
{
    public class SizeRepository : ISizeRepository
    {
        private readonly ApplicationDbContext _db;

        public SizeRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public IEnumerable<Size> GetAllSizes()
        {
            var data = _db.Sizes;
            return data;
        }
    }
}