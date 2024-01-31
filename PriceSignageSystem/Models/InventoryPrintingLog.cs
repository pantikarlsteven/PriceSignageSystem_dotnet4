using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PriceSignageSystem.Models
{
    public class InventoryPrintingLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public decimal O3SKU { get; set; }
        public string PrintedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public decimal RegularPrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public string ItemDesc { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Divisor { get; set; }
        public string Remarks { get; set; }
    }
}