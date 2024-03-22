using PriceSignageSystem.Models.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace PriceSignageSystem.Models.Repository
{
    public class SQLBulk : ISQLBulk
    {
        private readonly string connectionString;

        public SQLBulk()
        {
            connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
        }

        public void InsertBulk(DataTable data, string tableName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                    {
                        bulkCopy.DestinationTableName = tableName; // Replace with your table name
                        bulkCopy.BulkCopyTimeout = 3600;
                        bulkCopy.WriteToServer(data);
                    }
                }
                //WriteToFile.Log(tableName + " Bulk insert completed.");
            }
            catch (Exception ex)
            {
                //WriteToFile.Log(ex.Message);
            }
        }

        public void RemoveBulk(string tableName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand($"DELETE FROM {tableName}", connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                //WriteToFile.Log(tableName + " Bulk remove completed.");
            }
            catch (Exception ex)
            {
                //WriteToFile.Log(ex.Message);
            }
        }
    }
}