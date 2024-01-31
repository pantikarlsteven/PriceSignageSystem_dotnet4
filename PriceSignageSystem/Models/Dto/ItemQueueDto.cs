using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PriceSignageSystem.Models.Dto
{
    public class ItemQueueDto
    {
        public int Id { get; set; }
        public decimal O3SKU { get; set; }
        public int TypeId { get; set; }
        public int SizeId { get; set; }
        public int CategoryId { get; set; }
        public string UserName { get; set; }
        public string Status { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string TypeName { get; set; }
        public string SizeName { get; set; }
        public string CategoryName { get; set; }
        public string Remarks { get; set; }

    }
}