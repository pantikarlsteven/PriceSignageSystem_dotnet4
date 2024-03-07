using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PriceSignageSystem.Models.Dto
{
    public class AuditDto
    {
        [DisplayFormat(DataFormatString = "{0:0}")]
        public decimal O3LOC { get; set; }
        [Display(Name = "SKU")]
        [DisplayFormat(DataFormatString = "{0:0}")]
        public decimal O3SKU { get; set; }
        [Display(Name = "STATUS CODE")]
        public string O3SCCD { get; set; }
        [Display(Name = "DESC")]
        public string O3IDSC { get; set; }
        [Display(Name = "UPC")]
        [DisplayFormat(DataFormatString = "{0:0}")]
        public decimal O3UPC { get; set; }
        public decimal O3VNUM { get; set; }
        public string O3TYPE { get; set; }
        [Display(Name = "DEPT")]
        [DisplayFormat(DataFormatString = "{0:0}")]
        public decimal O3DEPT { get; set; }
        [Display(Name = "SUB-DEPT")]
        [DisplayFormat(DataFormatString = "{0:0}")]
        public decimal O3SDPT { get; set; }
        [Display(Name = "CLASS")]
        [DisplayFormat(DataFormatString = "{0:0}")]
        public decimal O3CLAS { get; set; }
        [Display(Name = "SUB-CLASS")]
        [DisplayFormat(DataFormatString = "{0:0}")]
        public decimal O3SCLS { get; set; }
        [Display(Name = "CURRENT PRICE")]
        public decimal O3POS { get; set; }
        public decimal O3POSU { get; set; }
        public decimal O3REG { get; set; }
        [Display(Name = "REG PRICE")]
        public decimal O3REGU { get; set; }
        [Display(Name = "ORIGINAL PRICE")]
        public decimal O3ORIG { get; set; }
        public decimal O3ORGU { get; set; }
        public decimal O3EVT { get; set; }
        public decimal O3PMMX { get; set; }
        public decimal O3PMTH { get; set; }
        public decimal O3PDQT { get; set; }
        public decimal O3PDPR { get; set; }
        [Display(Name = "START DATE")]
        public decimal O3SDT { get; set; }
        public decimal O3RSDT { get; set; }
        [Display(Name = "END DATE")]
        public decimal O3EDT { get; set; }
        public decimal O3REDT { get; set; }
        [Display(Name = "COUNTRY")]
        public string O3TRB3 { get; set; }
        [Display(Name = "BRAND NO.")]
        public decimal O3FGR { get; set; }
        [Display(Name = "BRAND NAME")]
        public string O3FNAM { get; set; }
        [Display(Name = "MODEL")]
        public string O3MODL { get; set; }
        [Display(Name = "LONG DESC")]
        public string O3LONG { get; set; }
        [Display(Name = "FROM U/M")]
        public string O3SLUM { get; set; }
        [Display(Name = "DIVISOR")]
        public string O3DIV { get; set; }
        [Display(Name = "TO UOM")]
        public string O3TUOM { get; set; }
        public decimal O3DATE { get; set; }
        public decimal O3CURD { get; set; }
        public string O3USER { get; set; }
        public DateTime DateUpdated { get; set; }
        [Display(Name = "Size")]
        public int SelectedSizeId { get; set; } = 1;
        [Display(Name = "Type")]
        public int SelectedTypeId { get; set; } = 1;
        [Display(Name = "Category")]
        public int SelectedCategoryId { get; set; } = 2;
        public string SizeName { get; set; }
        public string TypeName { get; set; }
        public string CategoryName { get; set; }
        public string StartDateFormattedDate { get; set; }
        public string EndDateFormattedDate { get; set; }
        public int TypeId { get; set; }
        public int SizeId { get; set; }
        public int CategoryId { get; set; }
        public string DepartmentName { get; set; }
        public string IsReverted { get; set; }
        public string IsPrinted { get; set; }
        public string IsAudited { get; set; } = "No";
        public string IsResolved { get; set; }
        public decimal LatestDate { get; set; }
        public string HasInventory { get; set; }
        public string IsExemp { get; set; }
        public string NegativeSave { get; set; }
        public decimal IBHAND { get; set; }
        public List<AuditDto> PrintedList { get; set; }
        public List<AuditDto> NotPrintedList { get; set; }
        public List<AuditDto> AuditedList { get; set; }
        public string Remarks { get; set; }
        public decimal ZeroInvDCOnHand { get; set; }
        public decimal ZeroInvInTransit { get; set; }
    }

}