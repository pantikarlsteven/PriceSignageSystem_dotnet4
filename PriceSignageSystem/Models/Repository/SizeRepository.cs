using PriceSignageSystem.Models.DatabaseContext;
using PriceSignageSystem.Models.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
            var data = _db.Size;
            return data;
        }
    }
}