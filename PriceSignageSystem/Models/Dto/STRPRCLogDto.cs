using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PriceSignageSystem.Models.Dto
{
    public class STRPRCLogDto
    {
        [DisplayFormat(DataFormatString = "{0:0}")]
        public decimal O3LOC { get; set; }
        [Display(Name = "SKU")]
        [DisplayFormat(DataFormatString = "{0:0}")]
        public decimal O3SKU { get; set; }
        [Display(Name = "STATUS CODE")]
        public string O3SCCD { get; set; }

        [Display(Name = "Is Printed")]
        public string IsPrinted { get; set; }

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
        [Display(Name = "END DATE")]
        public decimal O3EDT { get; set; }
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
        public List<SelectListItem> Sizes { get; set; }
        public List<SelectListItem> Types { get; set; }
        public List<SelectListItem> Categories { get; set; }
        public string SizeName { get; set; }
        public string TypeName { get; set; }
        public string CategoryName { get; set; }
        public Size[] SizeArray { get; set; }
        public Type[] TypeArray { get; set; }
        public Category[] CategoryArray { get; set; }
        public string StartDateFormattedDate { get; set; }
        public string EndDateFormattedDate { get; set; }
        public int TypeId { get; set; }
        public int SizeId { get; set; }
        public int CategoryId { get; set; }
        public string DepartmentName { get; set; }
        public string IsReverted { get; set; }

        public int Id { get; set; }
        private string _text;
        public string ColumnName { get { return _text; }
            set {
                // Perform filtering logic here
                var listText = new List<string>();

                foreach (var item in value.Split(',').ToList())
                {
                    switch (item)
                    {
                        case "O3IDSC":
                            listText.Add("Description");
                            break;
                        case "O3FNAM":
                            listText.Add("Brand");
                            break;
                        case "O3MODL":
                            listText.Add("Model");
                            break;
                        case "O3TUOM":
                            listText.Add("To UOM");
                            break;
                        case "O3SCCD":
                            listText.Add("Item Status");
                            break;
                        case "O3UPC":
                            listText.Add("UPC");
                            break;
                        case "O3TRB3":
                            listText.Add("FLAG");
                            break;
                        case "O3LONG":
                            listText.Add("Long Description");
                            break;
                        case "O3DEPT":
                            listText.Add("Hierarchy");
                            break;
                    }
                }

                _text = string.Join(",", listText);
            } 
        }
        public string FromValue { get; set; }
        public string ToValue { get; set; }
        public string Inv { get; set; }
    }
}