using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace PriceSignageSystem.Helper
{
    public static class ConversionHelper
    {
        // from datetime to decimal(6,0)
        public static decimal ToDecimal(DateTime date)
        {
            return decimal.Parse(date.ToString("yyMMdd"));
        }

        // from decimal(6,0) to datetime yyMMdd
        public static DateTime ToDateTime(decimal value)
        {
            string stringValue = value.ToString("000000");
            DateTime dateTimeValue = DateTime.ParseExact(stringValue, "yyMMdd", CultureInfo.InvariantCulture);

            return dateTimeValue;
        }
    }
}