using PriceSignageSystem.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceSignageSystem.Models.Interface
{
    public interface ISTRPRCRepository
    {
        STRPRCDto SearchString(string query);
        List<STRPRC> GetAll();
        IEnumerable<STRPRC> FilterByDate(decimal fromDate/*, decimal toDate*/);
        List<STRPRCDto> GetStores();
        List<STRPRC> GetData(decimal O3SKU);
        IEnumerable<STRPRC> GetAllData();
        IEnumerable<STRPRC> GetDataByDate(decimal startDate, decimal endDate);


    }
}
