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
                        IsNotRequired = reader["IsNotRequired"].ToString(),
                        AuditedRemarks = reader["AuditedRemarks"].ToString(),
                        IsWrongSign = reader["IsWrongSign"].ToString(),
                        O3FNAM = reader["O3FNAM"].ToString()

                    };

                    //if ((decimal)reader["O3RSDT"] == startDate)
                    //{
                    //    if ((int)reader["TypeId"] == ReportConstants.Type.Regular)
                    //    {
                    //        record.O3SDT = (decimal)reader["O3RSDT"];
                    //        record.O3EDT = (decimal)reader["O3REDT"];
                    //    }
                    //    else continue;
                    //}

                    data.Add(record);
                }

                // Close the reader and connection
                reader.Close();
                connection.Close();
            }

            return data;

        }

        public ScanResultDto ScanBarcode(string code, string codeFormat, bool isScaleTicket)
        {
            string formattedCode = string.Empty;
            ScanResultDto result = new ScanResultDto();
            decimal parsedCode = default(decimal);

            if (code.Contains("\r\n"))
            {
                code = code.Replace("\r\n", "");
            }
            if (!isScaleTicket)
            {
                if (codeFormat == "UPC_A")
                {
                    formattedCode = BarcodeHelper.GetNormalizedUPC_A(code);
                }
                else if (codeFormat == "UPC_E")
                {
                    formattedCode = BarcodeHelper.ConvertUPCEToUPCA(code);
                }
                parsedCode = ((formattedCode != string.Empty) ? decimal.Parse(formattedCode) : decimal.Parse(code));
                var upc = _db.CompleteSTRPRCs.Where(f => f.IUPC == parsedCode).FirstOrDefault();
                if (upc != null)
                {
                    result = _db.Database.SqlQuery<ScanResultDto>("EXEC sp_GetSkuForScanning @Sku", new SqlParameter("@Sku", upc.INUMBR)).FirstOrDefault();
                    result.IsItemExisting = "Yes";
                }
                else
                {
                    result.IsItemExisting = "No";
                }
            }
            else
            {
                formattedCode = BarcodeHelper.InHouseUPC(code);
                parsedCode = decimal.Parse(formattedCode);
                result = _db.Database.SqlQuery<ScanResultDto>("EXEC sp_GetSkuForScanning @Sku", new SqlParameter("@Sku", parsedCode)).FirstOrDefault();
                if (result != null)
                {
                    result.IsItemExisting = "Yes";
                }
                else
                {
                    result.IsItemExisting = "No";
                }
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

        public int NotRequireTagging(string sku, string isChecked, string username)
        {
            var isNotRequired = isChecked == "true" ? "Y" : "N";
            var rowsAffected = _db.Database.SqlQuery<int>("EXEC sp_NotRequireTagging @Sku, @IsNotRequired, @Username",
                new SqlParameter("@Sku", sku),
                new SqlParameter("@IsNotRequired", isNotRequired),
                new SqlParameter("@Username", username))
                .FirstOrDefault();

            return rowsAffected;

        }

        public bool PostWithRemarks(string sku, string username, string remarks)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("sp_AuditWithRemarks", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = commandTimeoutInSeconds;
                        // Add any required parameters to the command if needed
                        command.Parameters.AddWithValue("@Sku", sku);
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Remarks", remarks);

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

        public int TagWrongSign(string sku, string username)
        {
            var rowsAffected = _db.Database.SqlQuery<int>("EXEC sp_TagWrongSign @Sku, @Username",
                new SqlParameter("@Sku", sku),
                new SqlParameter("@Username", username))
                .FirstOrDefault();

            return rowsAffected;

        }

        public async Task<List<AuditDto>> GetSkuUpdates()
        {
            var result = await _db.Database.SqlQuery<AuditDto>("EXEC sp_GetSkuUpdates")
                .ToListAsync();

            return result;
        }

        public List<AuditDto> GetAll()
        {
            var result = new List<AuditDto>();
            try
            {
                result = _db.Database.SqlQuery<AuditDto>("EXEC sp_GetAllAudits")
                            .ToList();
            }
            catch (Exception ex)
            {

            }

            return result;
        }

        public SkuUpdatesAuditDto CheckIfExisting(decimal sku)
        {
            var result = _db.Database.SqlQuery<SkuUpdatesAuditDto>("EXEC sp_CheckIfExistingInAudit @Sku",
                new SqlParameter("@Sku", sku))
                .FirstOrDefault();

            return result;
        }

        public List<ExportAuditDto> GetAllAuditToExport()
        {
            var result = new List<ExportAuditDto>();
            try
            {
                result = _db.Database.SqlQuery<ExportAuditDto>("EXEC sp_GetAllAudits")
                            .ToList();
            }
            catch (Exception ex)
            {

            }

            return result;
        }

        public async Task<List<AuditDto>> GetAllPrinted()
        {
            var result = await _db.Database.SqlQuery<AuditDto>("EXEC sp_GetAllPrintedForAudit")
                         .ToListAsync();

            return result;
        }
        public async Task<List<AuditDto>> GetAllUnprinted()
        {
            var result = await _db.Database.SqlQuery<AuditDto>("EXEC sp_GetAllUnprintedForAudit")
                         .ToListAsync();

            return result;
        }
    }
}