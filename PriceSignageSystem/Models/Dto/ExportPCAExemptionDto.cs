﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PriceSignageSystem.Models.Dto
{
    public class ExportPCAExemptionDto
    {
        public decimal SKU { get; set; }
        public decimal UPC { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string ItemDesc { get; set; }
        public string LongDesc { get; set; }
        public decimal RegularPrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal StartDate { get; set; }
        public decimal EndDate { get; set; }
        public string DepartmentName { get; set; }
        public string IsPrinted { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }
        public string Category { get; set; }
        public decimal Onhand { get; set; }
        public string WithInventory { get; set; }
        public string IsExemp { get; set; }
        public string ExemptionType { get; set; }
        public string O3FLAG3 { get; set; }
    }
}