using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PriceSignageSystem.Models.Dto
{
    public class ExportPCADto
    {
        public decimal O3SKU { get; set; }
        public decimal O3UPC { get; set; }
        public string O3FNAM { get; set; }
        public string O3MODL { get; set; }
        public string O3IDSC { get; set; }
        public string O3LONG { get; set; }
        public decimal O3REGU { get; set; }
        public decimal O3POS { get; set; }
        public decimal O3SDT { get; set; }
        public decimal O3EDT { get; set; }
        public string DepartmentName { get; set; }
        public string IsReverted { get; set; }
        public string IsPrinted { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }
        public string Category { get; set; }
    }
}