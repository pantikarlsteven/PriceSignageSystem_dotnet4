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

        public List<Size> GetAllSizes()
        {
            var data = (from a in _db.Sizes
                       select a).ToList();
            return data;
        }
    }
}