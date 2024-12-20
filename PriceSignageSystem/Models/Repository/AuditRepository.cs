﻿using PriceSignageSystem.Helper;
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
            var sp = "sp_GetAllUnprintedForAudit";

            var data = new List<AuditDto>();

            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(sp, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = commandTimeoutInSeconds;

                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    var record = new AuditDto
                    {
                        O3SKU = (decimal)reader["O3SKU"],
                        O3UPC = (decimal)reader["O3UPC"],
                        O3POS = (decimal)reader["O3POS"],
                        O3REG = (decimal)reader["O3REG"],
                        O3SDT = (decimal)reader["O3SDT"],
                        O3EDT = (decimal)reader["O3EDT"],
                        O3FNAM = reader["O3FNAM"].ToString(),
                        O3IDSC = reader["O3IDSC"].ToString(),
                        O3TYPE = reader["O3TYPE"].ToString(),
                        O3DEPT = (decimal)reader["O3DEPT"],
                        O3SDPT = (decimal)reader["O3SDPT"],
                        O3CLAS = (decimal)reader["O3CLAS"],
                        O3SCLS = (decimal)reader["O3SCLS"],
                        TypeName = reader["TypeName"].ToString(),
                        SizeName = reader["SizeName"].ToString(),
                        CategoryName = reader["CategoryName"].ToString(),
                        DepartmentName = reader["DepartmentName"].ToString(),
                        IsPrinted = reader["IsPrinted"].ToString(),
                        IsReverted = reader["IsReverted"].ToString(),
                        IsWrongSign = reader["IsWrongSign"].ToString(),
                        IsNotRequired = reader["IsNotRequired"].ToString(),
                        IsAudited = reader["IsAudited"].ToString(),
                        HasInventory = reader["HasInventory"].ToString(),
                        AuditedRemarks = reader["AuditedRemarks"].ToString(),
                        IsExemp = reader["IsExemp"].ToString()
                    };

                    data.Add(record);
                }

                // Close the reader and connection
                reader.Close();
                connection.Close();
            }

            return data;
        }

        public async Task<List<AuditDto>> GetAllPrintedByHistory(string dateFilter)
        {
            List<AuditDto> records = new List<AuditDto>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                try
                {
                    string query = "SELECT " +
                        "* " +
                        "FROM Audit_PrintedHistory " +
                        "WHERE PrintedDate = @dateFilter " +
                        "ORDER BY O3SDT DESC ,O3DEPT, O3SDPT, O3CLAS, O3SCLS ASC ";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@dateFilter", dateFilter);

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                AuditDto record = new AuditDto
                                {
                                    O3SKU = (decimal)reader["O3SKU"],
                                    O3UPC = (decimal)reader["O3UPC"],
                                    O3FNAM = reader["O3FNAM"].ToString(),
                                    O3IDSC = reader["O3IDSC"].ToString(),
                                    O3REG = (decimal)reader["O3REG"],
                                    O3POS = (decimal)reader["O3POS"],
                                    O3SDT = (decimal)reader["O3SDT"],
                                    O3EDT = (decimal)reader["O3EDT"],
                                    O3TYPE = reader["O3TYPE"].ToString(),
                                    DepartmentName = reader["DepartmentName"].ToString(),
                                    TypeName = reader["TypeName"].ToString(),
                                    SizeName = reader["SizeName"].ToString(),
                                    CategoryName = reader["CategoryName"].ToString(),
                                    IsExemp = reader["IsExemp"].ToString(),
                                    IsReverted = reader["IsReverted"].ToString(),
                                    IsPrinted = reader["IsPrinted"].ToString(),
                                    IsWrongSign = reader["IsWrongSign"].ToString(),
                                    IsNotRequired = reader["IsNotRequired"].ToString(),
                                    IsAudited = reader["IsAudited"].ToString(),
                                    HasInventory = reader["HasInventory"].ToString(),
                                    AuditedRemarks = reader["AuditedRemarks"].ToString()
                                };
                                records.Add(record);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
            return records;
        }

        public async Task<List<AuditDto>> GetAllUnPrintedByHistory(string dateFilter)
        {
            DateTime date = DateTime.Parse(dateFilter);
            decimal dateFilterInDecimal = date.Year % 100 * 10000 + date.Month * 100 + date.Day;

            List<AuditDto> records = new List<AuditDto>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                try
                {
                    string query = "SELECT " +
                       "a.* " +
                       "FROM PCAHistory x " +
                       "INNER JOIN Audit_UnprintedHistory a on x.O3SKU = a.O3SKU and x.PCADate = a.UnprintedDate " +
                       "WHERE x.PCADate = @dateFilter AND x.O3SDT = @startDate AND x.IsPrinted = 'No' " +
                       "ORDER BY x.O3SDT DESC ,x.O3DEPT, x.O3SDPT, x.O3CLAS, x.O3SCLS ASC ";
                   
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@dateFilter", dateFilter);
                        cmd.Parameters.AddWithValue("@startDate", dateFilterInDecimal);

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                AuditDto record = new AuditDto
                                {
                                    O3SKU = (decimal)reader["O3SKU"],
                                    O3UPC = (decimal)reader["O3UPC"],
                                    O3FNAM = reader["O3FNAM"].ToString(),
                                    O3IDSC = reader["O3IDSC"].ToString(),
                                    O3REG = (decimal)reader["O3REG"],
                                    O3POS = (decimal)reader["O3POS"],
                                    O3SDT = (decimal)reader["O3SDT"],
                                    O3EDT = (decimal)reader["O3EDT"],
                                    O3TYPE = reader["O3TYPE"].ToString(),
                                    DepartmentName = reader["DepartmentName"].ToString(),
                                    TypeName = reader["TypeName"].ToString(),
                                    SizeName = reader["SizeName"].ToString(),
                                    CategoryName = reader["CategoryName"].ToString(),
                                    IsExemp = reader["IsExemp"].ToString(),
                                    IsReverted = reader["IsReverted"].ToString(),
                                    IsPrinted = reader["IsPrinted"].ToString(),
                                    IsWrongSign = reader["IsWrongSign"].ToString(),
                                    IsNotRequired = reader["IsNotRequired"].ToString(),
                                    IsAudited = reader["IsAudited"].ToString(),
                                    HasInventory = reader["HasInventory"].ToString(),
                                    AuditedRemarks = reader["AuditedRemarks"].ToString()
                                };
                                records.Add(record);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
            return records;
        }
    }
}