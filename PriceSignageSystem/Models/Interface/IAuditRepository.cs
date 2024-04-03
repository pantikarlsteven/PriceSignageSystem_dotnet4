using PriceSignageSystem.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceSignageSystem.Models.Interface
{
    public interface IAuditRepository
    {
        decimal GetLatestDate();
        Task<List<AuditDto>> GetPCAbyLatestDate(decimal latestDate);
        ScanResultDto ScanBarcode(string code, string codeFormat);
        bool Post(string sku, string username);
        int NotRequireTagging(string sku, string isChecked, string username);
        bool PostWithRemarks(string sku, string username, string remarks);
        int TagWrongSign(string sku, string username);

        Task<List<AuditDto>> GetSkuUpdates();
        List<AuditDto> GetAll();
    }
}
