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
        STRPRCDto SearchString(string query, string searchFilter, string codeFormat);
        List<STRPRC> GetAll();
        List<STRPRCDto> GetStores();
        List<STRPRCDto> GetData(decimal O3SKU);
        IEnumerable<STRPRC> GetAllData();
        Task<List<STRPRCDto>> GetDataByStartDate(decimal startDate);
        STRPRCDto GetSKUDetails(decimal O3SKU);
        List<STRPRCLogDto> GetUpdatedData(string latestDate);
        STRPRCDto GetDataBySKU(decimal O3SKU);
        STRPRCDto GetLatestUpdate();
        bool GetLatestInventory(string storeId);
        Task UpdateSTRPRC151(int storeId);
        decimal CheckSTRPRCUpdates(int storeId);

        decimal UpdateSTRPRCTable(int storeId);
        bool Check151STRPRCChanges_LatestDate(int o3loc);
        bool Check151STRPRCNew_LatestDate(int o3loc);
        Task<bool> UpdateCentralizedExemptions(decimal startDate);
        CountryDto GetCountryImg(string country);
        ReportDto GetReportData(decimal O3SKU);
        List<ReportDto> GetReportDataList(List<decimal> O3SKU);
        void UpdateSelection(decimal startDate, decimal endDate);
        void UpdateMultipleStatus(List<decimal> o3skus);
        void AddMultipleInventoryPrintingLog(List<decimal> o3skus, string user, int sizeId, string printedOn);
        void AddMultipleQueuedPrintingLog(IEnumerable<ReportDto> data, string user, int sizeId);
        void UpdateSingleStatus(decimal O3SKU);
        void AddInventoryPrintingLog(ReportDto model, string user);
        List<STRPRCDto> GetLatestPCAData();
        List<ExportPCAExemptionDto> PCAToExportExemption();
        List<ExportPCADto> PCAToExport();
        string GetSubClassDescription(decimal O3SKU);
        Task<CentralizedExemptionStatusDto> CheckCentralizedExemptionStatus();
        void UpdateCentralizedExemptionStatus(CentralizedExemptionStatusDto data, bool onGoingUpdate);
        ReportDto GetPrintedLogPerSku(string sku);
        int SyncFromNew();
        int UpdateUPC();
        Task<List<STRPRCDto>> GetAllConsignment(decimal startDate);
        List<ExportPCAExemptionDto> GetAllNoConsignmentContract();
        List<ExportPCADto> GetConsignmentToExport(decimal[] selectedSkus);
        Task<List<STRPRCDto>> GetDataByPCAHistory(string dateFilter);
        Task<List<STRPRCDto>> GetDataByConsignmentHistory(string dateFilter);
        PromoEngineDto CheckIfSkuHasPromo(decimal sku);

    }
}
