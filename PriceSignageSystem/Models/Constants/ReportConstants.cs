using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace PriceSignageSystem.Models.Constants
{
    public static class ReportConstants
    {
        public const string Dynamic_WholeReportPath = "~/Reports/CrystalReports/Dynamic_WholeReport.rpt";
        public const string Dynamic_HalfReportPath = "~/Reports/CrystalReports/Dynamic_HalfReport.rpt";
        public const string Dynamic_JewelryReportPath = "~/Reports/CrystalReports/Dynamic_JewelryReport.rpt";
        public const string Dynamic_SkinnyReportPath = "~/Reports/CrystalReports/Dynamic_SkinnyReport.rpt";

        public static class Size
        {
            public const int Whole = 1;
            public const int Half = 2;
            public const int Jewelry = 3;
            public const int Skinny = 4;
        }

        public static class Category
        {
            public const int Appliance = 1;
            public const int NonAppliance = 2;
        }

        public static class Status
        {
            public const string InQueue = "InQueue";
            public const string Printed = "Printed";
        }
    }
}