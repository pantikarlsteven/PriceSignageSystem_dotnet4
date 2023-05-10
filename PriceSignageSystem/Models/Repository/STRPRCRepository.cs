using PriceSignageSystem.Models.DatabaseContext;
using PriceSignageSystem.Models.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PriceSignageSystem.Models.Repository
{
    public class STRPRCRepository : ISTRPRCRepository
    {
        private readonly ApplicationDbContext _db;

        public STRPRCRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public IQueryable<STRPRC> GetAll()
        {
            var data = _db.STRPRC;
            return data;
        }

        public STRPRC Fetch(string query)
        {
            var data = (from a in _db.STRPRC
                        where a.O3SKU.ToString() == query || a.O3UPC.ToString() == query
                        select a).FirstOrDefault();
            return data;
        }

        public IEnumerable<STRPRC> FilterByDate(decimal fromDate/*, decimal toDate*/)
        {
            var data = (from a in _db.STRPRC
                        where a.O3SDT >= fromDate// && a.O3EDT <= toDate
                        select a).ToList();

            return data;
        }
    }
}