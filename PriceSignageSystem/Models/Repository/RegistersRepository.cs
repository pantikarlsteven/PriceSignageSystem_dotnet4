using PriceSignageSystem.Models.DatabaseContext;
using PriceSignageSystem.Models.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PriceSignageSystem.Models.Repository
{
    public class RegistersRepository : IRegistersRepository
    {
        public readonly ApplicationDbContext _db;

        public RegistersRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public IEnumerable<Country> GetCountries()
        {
            var data = _db.Countries.ToList();

            return data;
        }

        public void Upload(string countryName, byte[] fileInput)
        {
            var data = new Country();
            data.iatrb3 = countryName;
            data.country_img = fileInput;

            _db.Countries.Add(data);
            _db.SaveChanges();
        }

        public void DeleteSelectedCountry(string[] selectedRows)
        {
            foreach (var item in selectedRows)
            {
                var data = _db.Countries.Find(item);

                if(data != null)
                {
                    _db.Countries.Remove(data);
                }
            }

            _db.SaveChanges();
        }
    }
}