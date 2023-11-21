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
        Task<List<STRPRCDto>> GetDataByStartDate(decimal startDate);
        STRPRCDto GetSKUDetails(decimal O3SKU);
        List<STRPRCLogDto> GetUpdatedData(decimal sku = 0);
        STRPRCDto GetDataBySKU(decimal O3SKU);
        STRPRCDto GetLatestUpdate();
        decimal UpdateSTRPRCTable(int storeId);
        CountryDto GetCountryImg(string country);
        ReportDto GetReportData(decimal O3SKU);
        List<ReportDto> GetReportDataList(List<decimal> O3SKU);
        void UpdateSelection(decimal startDate, decimal endDate);
        void UpdateMultipleStatus(List<decimal> o3skus);
        void AddMultipleInventoryPrintingLog(List<decimal> o3skus, string user);
        void UpdateSingleStatus(decimal O3SKU);
        void AddInventoryPrintingLog(decimal O3SKU, string user);
        List<STRPRCDto> GetLatestPCAData();
        List<ExportPCADto> PCAToExport();
        string GetSubClassDescription(decimal O3SKU);
    }
}
