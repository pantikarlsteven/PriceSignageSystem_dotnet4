using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PriceSignageSystem.Models.Constants
{
    public static class ReportConstants
    {
        public const string ApplianceReportPath = "~/Reports/ApplianceReport.rdlc";
        public const string ApplianceReportPath_Half = "~/Reports/ApplianceReport_Half.rdlc";
        public const string ApplianceReportPath_Jewelry = "~/Reports/ApplianceReport_Jewelry.rdlc";
        public const string ApplianceReportPath_Skinny = "~/Reports/ApplianceReport_Skinny.rdlc";

        public const string DynamicQueueReportPath = "~/Reports/DynamicQueueReport_Whole.rdlc";
        public const string DynamicQueueReportPath_Half = "~/Reports/DynamicQueueReport_Half.rdlc";
        public const string DynamicQueueReportPath_Jewelry = "~/Reports/DynamicQueueReport_Jewelry.rdlc";

        public const string WholeReport_SLBrandAndSLDescPath = "~/Reports/CrystalReports/WholeReport/WholeReport_SLBrandAndSLDesc.rpt";

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