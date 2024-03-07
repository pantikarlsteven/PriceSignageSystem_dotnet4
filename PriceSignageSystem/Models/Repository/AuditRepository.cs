using PriceSignageSystem.Helper;
using PriceSignageSystem.Models.Constants;
using PriceSignageSystem.Models.DatabaseContext;
using PriceSignageSystem.Models.Dto;
using PriceSignageSystem.Models.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace PriceSignageSystem.Models.Repository
{
    public class AuditRepository : IAuditRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly string connectionString;
        private readonly int commandTimeoutInSeconds;

        public AuditRepository(ApplicationDbContext db)
        {
            _db = db;
            connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
            commandTimeoutInSeconds = 180;
        }

        public decimal GetLatestDate()
        {
            return _db.STRPRCs.OrderByDescending(o => o.O3DATE).Select(s => s.O3DATE).FirstOrDefault();
        }

        public async Task<List<AuditDto>> GetPCAbyLatestDate(decimal startDate)
        {
            var sp = "sp_GettmpData";
            var data = new List<AuditDto>();
            var store = int.Parse(ConfigurationManager.AppSettings["StoreID"]);
            // Set up the connection and command
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(sp, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = commandTimeoutInSeconds;

                // Add parameters if required
                command.Parameters.AddWithValue("@O3SDT", startDate);
                command.Parameters.AddWithValue("@O3LOC", store);

                // Open the connection and execute the command
                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();

                // Process the result set
                while (reader.Read())
                {
                    var record = new AuditDto
                    {
                        IsPrinted = reader["IsPrinted"].ToString(),
                        O3SKU = (decimal)reader["O3SKU"],
                        O3UPC = (decimal)reader["O3UPC"],
                        O3IDSC = reader["O3IDSC"].ToString(),
                        O3REG = (decimal)reader["O3REG"],
                        O3POS = (decimal)reader["O3POS"],
                        O3SDT = (decimal)reader["O3SDT"],
                        O3EDT = (decimal)reader["O3EDT"],
                        O3RSDT = (decimal)reader["O3RSDT"],
                        O3REDT = (decimal)reader["O3REDT"],
                        TypeId = (int)reader["TypeId"],
                        SizeId = (int)reader["SizeId"],
                        CategoryId = (int)reader["CategoryId"],
                        DepartmentName = reader["DPTNAM"].ToString(),
                        IsReverted = reader["O3FLAG1"].ToString(),
                        HasInventory = reader["INV2"].ToString(),
                        IsExemp = reader["IsExemp"].ToString(),
                        NegativeSave = reader["NegativeSave"].ToString(),
                        O3TYPE = reader["O3TYPE"].ToString(),
                        IBHAND = (decimal)reader["IBHAND"],
                        ZeroInvDCOnHand = (decimal)reader["ZeroInvDCOnHand"],
                        ZeroInvInTransit = (decimal)reader["ZeroInvInTransit"],
                        IsAudited = reader["IsAudited"].ToString(),
                        IsResolved = reader["IsResolved"].ToString()
                    };

                    if ((decimal)reader["O3RSDT"] == startDate)
                    {
                        if ((int)reader["TypeId"] == ReportConstants.Type.Regular)
                        {
                            record.O3SDT = (decimal)reader["O3RSDT"];
                            record.O3EDT = (decimal)reader["O3REDT"];
                        }
                        else continue;
                    }

                    data.Add(record);
                }

                // Close the reader and connection
                reader.Close();
                connection.Close();
            }

            return data;

        }

        public ScanResultDto ScanBarcode(string code, string codeFormat)
        {
            var formattedCode = string.Empty;
            var result = new ScanResultDto();

            if (codeFormat == "UPC_A")
            {
                formattedCode = BarcodeHelper.GetNormalizedUPC_A(code);
            }
            else if (codeFormat == "UPC_E")
            {
                formattedCode = BarcodeHelper.GetNormalizedUPC_E(code);
            }

            formattedCode = formattedCode != string.Empty ? formattedCode : code;

            var upc = _db.CompleteSTRPRCs.Where(f => f.IUPC.ToString() == formattedCode).FirstOrDefault();

            if (upc != null)
            {
                var data = _db.Database.SqlQuery<ScanResultDto>("EXEC sp_CheckIfBelongToCurrentPCA @Sku", new SqlParameter("@Sku", upc.INUMBR)).FirstOrDefault(); // check if sku belongs to current pca 
               
                result.IsItemExisting = true;

                if (data != null)
                {
                    result.Sku = data.Sku;
                    result.CurrentPrice = data.CurrentPrice;
                    result.Desc = data.Desc;
                    result.DoesItemBelongToCurrentPCA = true;
                    result.IsPrinted = data.IsPrinted && data.IsPrinted;
                    result.IsAudited = data.IsAudited;
                }
                else
                {
                    result.DoesItemBelongToCurrentPCA = false;
                }

            }
            else
            {
                result.IsItemExisting = false;
            }
          
            return result;
        }

        public bool Post(string sku, string username)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("sp_Audit", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = commandTimeoutInSeconds;
                        // Add any required parameters to the command if needed
                        command.Parameters.AddWithValue("@Sku", sku);
                        command.Parameters.AddWithValue("@Username", username);

                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error executing stored procedure: " + ex.Message);
                return false;
            }
        }

        public int ResolveUnresolve(string sku, string isChecked, string username)
        {
            var IsResolved = isChecked == "true" ? "Y" : "N";
            var rowsAffected = _db.Database.SqlQuery<int>("EXEC sp_ResolveUnresolve @Sku, @IsResolved, @Username",
                new SqlParameter("@Sku", sku),
                new SqlParameter("@IsResolved", IsResolved),
                new SqlParameter("@Username", username))
                .FirstOrDefault();

            return rowsAffected;

        }
    }
}