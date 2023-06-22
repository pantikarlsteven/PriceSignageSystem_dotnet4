using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PriceSignageSystem.Models
{
    public class STRPRCLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public decimal O3SKU { get; set; }
        public string ColumnName { get; set; }
        public string FromValue { get; set; }
        public string ToValue { get; set;}
        public DateTime DateUpdated { get; set; }
        public bool IsPrinted { get; set; }
    }
}