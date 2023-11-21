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
        public const string Dynamic_OneEightReportPath = "~/Reports/CrystalReports/Dynamic_OneEightReport.rpt";
        public const string Dynamic_JewelryReportPath = "~/Reports/CrystalReports/Dynamic_JewelryReport.rpt";

        public static class Size
        {
            public const int Whole = 1;
            public const int OneEight = 2;
            public const int Jewelry = 3;
        }

        public static class Category
        {
            public const int Appliance = 1;
            public const int NonAppliance = 2;
        }

        public static class Type
        {
            public const int Regular = 1;
            public const int Save = 2;
        }

        public static class Status
        {
            public const string InQueue = "InQueue";
            public const string Printed = "Printed";
        }
    }
}