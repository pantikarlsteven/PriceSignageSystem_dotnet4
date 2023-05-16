using PriceSignageSystem.Models.DatabaseContext;
using PriceSignageSystem.Models.Interface;
using System.Collections.Generic;

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
            var data = _db.Types;
            return data;
        }
    }
}