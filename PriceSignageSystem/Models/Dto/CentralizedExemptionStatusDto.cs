using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PriceSignageSystem.Models.Dto
{
    public class CentralizedExemptionStatusDto
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public DateTime? DateUpdated { get; set; }
        public bool OngoingUpdate { get; set; }
    }
}