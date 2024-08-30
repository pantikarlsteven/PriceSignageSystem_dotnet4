using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PriceSignageSystem.Models.Dto
{
    public class PromoEngineDto
    {
        public decimal Sku { get; set; }
        public decimal StartDate { get; set; }
        public decimal EndDate { get; set; }
        public string PromoType { get; set; }
        public decimal PromoVal { get; set; }
        public int TypeId { get; set; }
    }
}