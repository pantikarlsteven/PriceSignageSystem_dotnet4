using PriceSignageSystem.Models.DatabaseContext;
using PriceSignageSystem.Models.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PriceSignageSystem.Models.Repository
{
    public class TypeRepository : ITypeRepository
    {
        private readonly ApplicationDbContext _db;

        public TypeRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public IEnumerable<Type> GetAllTypes()
        {
            var data = _db.Type;
            return data;
        }
    }
}