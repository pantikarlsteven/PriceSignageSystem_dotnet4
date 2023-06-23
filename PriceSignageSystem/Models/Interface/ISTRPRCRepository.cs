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
        List<STRPRCDto> GetDataByStartDate(decimal startDate, bool withInventory);
        List<STRPRCLogDto> GetUpdatedData(decimal sku = 0);
        STRPRCDto GetDataBySKU(decimal O3SKU);
        DateTime GetLatestUpdate();
        decimal UpdateSTRPRCTable(int storeId);
        CountryDto GetCountryImg(string country);
        ReportDto GetReportData(decimal O3SKU);
        List<ReportDto> GetReportDataList(List<decimal> O3SKU);
        void UpdateSelection(decimal startDate, decimal endDate);
        void UpdateMultipleStatus(List<decimal> o3skus);
        void UpdateSingleStatus(decimal O3SKU);
    }
}
