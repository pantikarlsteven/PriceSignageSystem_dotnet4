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
        List<STRPRCDto> GetStores();
        List<STRPRCDto> GetData(decimal O3SKU);
        IEnumerable<STRPRC> GetAllData();
        List<STRPRC> GetDataByDate(decimal startDate, decimal endDate);
        STRPRCDto GetDataBySKU(decimal O3SKU);
        DateTime GetLatestUpdate();
        int UpdateSTRPRCTable(int storeId);
        List<CountryDto> GetCountryImg(string country);
    }
}
