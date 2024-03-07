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
    }
}