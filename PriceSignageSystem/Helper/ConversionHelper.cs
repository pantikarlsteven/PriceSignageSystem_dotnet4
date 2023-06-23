using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
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

        // from decimal(6,0) to string date yyMMdd
        public static string ToDateTime(decimal value)
        {

            // Convert the decimal value to a string with 'yyMMdd' format
            string dateString = value.ToString("000000");

            // Parse the string as a DateTime object using the desired format
            DateTime date = DateTime.ParseExact(dateString, "yyMMdd", CultureInfo.InvariantCulture);

            // Format the DateTime object as 'yy-mm-dd'
            string formattedDate = date.ToString("yy-MM-dd");

            return formattedDate;
        }

        public static DataTable ConvertObjectToDataTable<T>(T obj)
        {
            DataTable dataTable = new DataTable();
            PropertyInfo[] properties = typeof(T).GetProperties();

            foreach (PropertyInfo property in properties)
            {
                dataTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }

            DataRow dataRow = dataTable.NewRow();

            foreach (PropertyInfo property in properties)
            {
                dataRow[property.Name] = property.GetValue(obj) ?? DBNull.Value;
            }

            dataTable.Rows.Add(dataRow);

            return dataTable;
        }

        public static DataTable ConvertListToDataTable<T>(IEnumerable<T> list)
        {
            var dataTable = new DataTable();

            // Get the properties of the type
            var properties = typeof(T).GetProperties();

            // Create columns in the DataTable based on the property names
            foreach (var prop in properties)
            {
                dataTable.Columns.Add(prop.Name, prop.PropertyType);
            }

            // Add rows to the DataTable with property values from the list
            foreach (var item in list)
            {
                var values = properties.Select(prop => prop.GetValue(item)).ToArray();
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }
    }
}