using System;
using System.ComponentModel.DataAnnotations;

namespace PriceSignageSystem.Models
{
    public class STRPRC
    {
        public decimal O3LOC { get; set; }
        [Key]
        public decimal O3SKU { get; set; }
        public string O3SCCD { get; set; }
        public string O3IDSC { get; set; }
        public decimal O3UPC { get; set; }
        public decimal O3VNUM { get; set; }
        public string O3TYPE { get; set; }
        public decimal O3DEPT { get; set; }
        public decimal O3SDPT { get; set; }
        public decimal O3CLAS { get; set; }
        public decimal O3SCLS { get; set; }
        public decimal O3POS { get; set; }
        public decimal O3POSU { get; set; }
        public decimal O3REG { get; set; }
        public decimal O3REGU { get; set; }
        public decimal O3ORIG { get; set; }
        public decimal O3ORGU { get; set; }
        public decimal O3EVT { get; set; }
        public decimal O3PMMX { get; set; }
        public decimal O3PMTH { get; set; }
        public decimal O3PDQT { get; set; }
        public decimal O3PDPR { get; set; }
        public decimal O3SDT { get; set; }
        public decimal O3EDT { get; set; }
        public string O3TRB3 { get; set; }
        public decimal O3FGR { get; set; }
        public string O3FNAM { get; set; }
        public string O3MODL { get; set; }
        public string O3LONG { get; set; }
        public string O3SLUM { get; set; }
        public string O3DIV { get; set; }
        public string O3TUOM { get; set; }
        public decimal O3DATE { get; set; }
        public decimal O3CURD { get; set; }
        public string O3USER { get; set; }
        public DateTime DateUpdated { get; set; }
        public int TypeId { get; set; }
        public int SizeId { get; set; }
        public int CategoryId { get; set; }
        public bool IsPrinted { get; set; }
    }
}