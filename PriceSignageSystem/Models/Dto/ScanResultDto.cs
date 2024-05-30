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
        public string IsPrinted { get; set; }
        public string IsItemExisting { get; set; }
        public string IsPCA { get; set; }
        public string IsAudited { get; set; }
        public string IsResolved { get; set; }
        public string ChangedOn { get; set; }
        public string IsSKUUpdate { get; set; }
    }
}