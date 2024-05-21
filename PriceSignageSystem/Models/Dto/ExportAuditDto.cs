using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PriceSignageSystem.Models.Dto
{
    public class ExportAuditDto
    {
        public decimal O3SKU { get; set; }
        public decimal UPC { get; set; }
        public string Brand { get; set; }
        public string Description { get; set; }
        public decimal RegPrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal DateStart { get; set; }
        public decimal DateEnd { get; set; }
        public string TypeName { get; set; }
        public string SizeName { get; set; }
        public string CategoryName { get; set; }
        public string Type { get; set; }
        public string Reverted { get; set; }
        public string Exemption { get; set; }
        public string Remarks { get; set; }
        public string IsAudited { get; set; }
    }
}