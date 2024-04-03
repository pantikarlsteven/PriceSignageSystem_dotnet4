using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PriceSignageSystem.Models.Dto
{
    public class ScanResultDto
    {
        public decimal Sku { get; set; }
        public decimal CurrentPrice { get; set; }
        public string Desc { get; set; }
        public bool IsPrinted { get; set; }
        public bool IsItemExisting { get; set; }
        public bool DoesItemBelongToCurrentPCA { get; set; }
        public string IsAudited { get; set; }
        public string IsResolved { get; set; }
        public string NewUPC { get; set; }
        public string NewModel { get; set; }
        public string NewBrand { get; set; }
        public string NewDept { get; set; }
        public string NewFlag { get; set; }
        public string NewDesc { get; set; }
    }
}