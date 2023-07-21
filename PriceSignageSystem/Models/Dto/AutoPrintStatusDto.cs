using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PriceSignageSystem.Models.Dto
{
    public class AutoPrintStatusDto
    {
        public bool IsPrinted { get; set; }
        public decimal O3DATE { get; set; }
    }
}