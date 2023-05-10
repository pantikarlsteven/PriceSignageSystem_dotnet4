using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceSignageSystem.Models.Interface
{
    public interface ISTRPRCRepository
    {
        IQueryable<STRPRC> GetAll();
        STRPRC Fetch(string query);
        IEnumerable<STRPRC> FilterByDate(decimal fromDate/*, decimal toDate*/);
    }
}
