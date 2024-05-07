using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PriceSignageSystem.Models.Dto
{
    public class SkuUpdatesAuditDto
    {
        public decimal Sku { get; set; }
        public string IsAudited { get; set; }
        public string AuditedBy { get; set; }
        public DateTime? DateAudited { get; set; }
        public string AuditedRemarks { get; set; }
        public string IsNotRequired { get; set; }
        public string IsWrongSign { get; set; }
        public string TaggedBy { get; set; }
        public DateTime? DateTagged { get; set; }
    }
}