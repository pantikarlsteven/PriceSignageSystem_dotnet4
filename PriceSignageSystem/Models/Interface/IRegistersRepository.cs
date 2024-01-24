using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceSignageSystem.Models.Interface
{
    public interface IRegistersRepository
    {
        IEnumerable<Country> GetCountries();
        void Upload(string countryName, byte[] fileInput);
        void DeleteSelectedCountry(string[] selectedRows);
    }
}
