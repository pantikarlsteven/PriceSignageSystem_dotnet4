using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PriceSignageSystem.Models
{
    public class Country
    {
        [Key]
        public string iatrb3 { get; set; }
        public byte[] country_img { get; set; }
    }
}