using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PriceSignageSystem.Models.Dto
{
    public class ReportDto
    {
        public decimal O3LOC { get; set; }
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
        public string UserName { get; set; }
        public int TypeId { get; set; }
        public int SizeId { get; set; }
        public int CategoryId { get; set; }
        public string Status { get; set; }
        public string iatrb3 { get; set; }
        public byte[] country_img { get; set; }
        public int ItemQueueId { get; set; }
        public bool IsSLBrand { get; set; }
        public bool IsSLDescription { get; set; }
        public bool IsBiggerFont { get; set; }
        public string O3SDSC { get; set; }
        public string qRemarks { get; set; }
        public string qBrand { get; set; }
        public string qModel { get; set; }
        public string qItemDesc { get; set; }
        public decimal qRegularPrice { get; set; }
        public decimal qCurrentPrice { get; set; }
        public string qDivisor { get; set; }
        public int qTypeId { get; set; }
        public string qTuom { get; set; }
    }
}