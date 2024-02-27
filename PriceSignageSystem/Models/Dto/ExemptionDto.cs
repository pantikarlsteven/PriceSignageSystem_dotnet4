using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PriceSignageSystem.Models.Dto
{
    public class ExemptionDto
    {
        public int Id { get; set; }
        public decimal O3SKU { get; set; }
        public string O3IDSC { get; set; }
        public decimal O3UPC { get; set; }
        public int StoreID { get; set; }
        public DateTime DateExemption { get; set; }
    }
}