﻿using Org.BouncyCastle.Asn1.Pkcs;
using PriceSignageSystem.Helper;
using PriceSignageSystem.Models.Constants;
using PriceSignageSystem.Models.DatabaseContext;
using PriceSignageSystem.Models.Dto;
using PriceSignageSystem.Models.Interface;
using PriceSignageSystem.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinSCP;

namespace PriceSignageSystem.Models.Repository
{
    public class STRPRCRepository : ISTRPRCRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly string connectionString;
        private readonly string connectionString151;
        private readonly string connStringCentralizedExemptions;
        private readonly int commandTimeoutInSeconds;
        private readonly int _storeID;
        private readonly string _server;
        private readonly int _protocol;

        public STRPRCRepository(ApplicationDbContext db)
        {
            _db = db;
            connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
            connectionString151 = ConfigurationManager.ConnectionStrings["MyConnectionString151"].ConnectionString;
            connStringCentralizedExemptions = ConfigurationManager.ConnectionStrings["ConnStringCentralizedExemptions"].ConnectionString;
            commandTimeoutInSeconds = 3600;
            _storeID = int.Parse(ConfigurationManager.AppSettings["StoreID"]);
            _server = ConfigurationManager.AppSettings["Server"];
            _protocol = int.Parse(ConfigurationManager.AppSettings["Protocol"]);
        }

        public List<STRPRC> GetAll()
        {
            var records = new List<STRPRC>();
            // Set up the connection and command
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand("sp_FetchAllSTRPRC", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                // Open the connection and execute the command
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                // Process the result set
                while (reader.Read())
                {
                    var record = new STRPRC
                    {
                        O3LOC = (decimal)reader["O3LOC"],
                        O3CLAS = (decimal)reader["O3CLAS"],
                        O3IDSC = reader["O3IDSC"].ToString(),
                        O3SKU = (decimal)reader["O3SKU"],
                        O3SCCD = reader["O3SCCD"].ToString(),
                        O3UPC = (decimal)reader["O3UPC"],
                        O3VNUM = (decimal)reader["O3VNUM"],
                        O3TYPE = reader["O3TYPE"].ToString(),
                        O3DEPT = (decimal)reader["O3DEPT"],
                        O3SDPT = (decimal)reader["O3SDPT"],
                        O3SCLS = (decimal)reader["O3SCLS"],
                        O3POS = (decimal)reader["O3POS"],
                        O3POSU = (decimal)reader["O3POSU"],
                        O3REG = (decimal)reader["O3REG"],
                        O3REGU = (decimal)reader["O3REGU"],
                        O3ORIG = (decimal)reader["O3ORIG"],
                        O3ORGU = (decimal)reader["O3ORGU"],
                        O3EVT = (decimal)reader["O3EVT"],
                        O3PMMX = (decimal)reader["O3PMMX"],
                        O3PMTH = (decimal)reader["O3PMTH"],
                        O3PDQT = (decimal)reader["O3PDQT"],
                        O3PDPR = (decimal)reader["O3PDPR"],
                        O3SDT = (decimal)reader["O3SDT"],
                        O3EDT = (decimal)reader["O3EDT"],
                        O3TRB3 = reader["O3TRB3"].ToString(),
                        O3FGR = (decimal)reader["O3FGR"],
                        O3FNAM = reader["O3FNAM"].ToString(),
                        O3SLUM = reader["O3SLUM"].ToString(),
                        O3DIV = reader["O3DIV"].ToString(),
                        O3TUOM = reader["O3TUOM"].ToString(),
                        O3DATE = (decimal)reader["O3DATE"],
                        O3CURD = (decimal)reader["O3CURD"],
                        O3USER = reader["O3USER"].ToString(),
                        DateUpdated = (DateTime)reader["DateUpdated"]
                    };

                    records.Add(record);
                }

                // Close the reader and connection
                reader.Close();
                connection.Close();
            }

            return records;
        }

        public List<STRPRCDto> GetStores()
        {
            var stores = new List<STRPRCDto>();
            // Set up the connection and command
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand("sp_FetchDistinctStores", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                // Open the connection and execute the command
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                // Process the result set
                while (reader.Read())
                {
                    var store = new STRPRCDto
                    {
                        O3LOC = (decimal)reader["O3LOC"]
                    };

                    stores.Add(store);
                }

                // Close the reader and connection
                reader.Close();
                connection.Close();
            }

            return stores;
        }

        public STRPRCDto SearchString(string query, string searchFilter, string codeFormat)
        {
            var data = new STRPRCDto();

            if (searchFilter == "SKU")
            {
                data = (from a in _db.STRPRCs
                        where a.O3SKU.ToString() == query
                        select new STRPRCDto
                        {
                            O3LOC = a.O3LOC,
                            O3CLAS = a.O3CLAS,
                            O3IDSC = a.O3IDSC,
                            O3SKU = a.O3SKU,
                            O3SCCD = String.IsNullOrEmpty(a.O3SCCD) ? "-" : a.O3SCCD,
                            O3UPC = a.O3UPC,
                            O3VNUM = a.O3VNUM,
                            O3TYPE = a.O3TYPE,
                            O3DEPT = a.O3DEPT,
                            O3SDPT = a.O3SDPT,
                            O3SCLS = a.O3SCLS,
                            O3POS = a.O3POS,
                            O3POSU = a.O3POSU,
                            O3REG = a.O3REG,
                            O3REGU = a.O3REGU,
                            O3ORIG = a.O3ORIG,
                            O3ORGU = a.O3ORGU,
                            O3EVT = a.O3EVT,
                            O3PMMX = a.O3PMMX,
                            O3PMTH = a.O3PMTH,
                            O3PDQT = a.O3PDQT,
                            O3PDPR = a.O3PDPR,
                            O3SDT = a.O3SDT,
                            O3EDT = a.O3EDT,
                            O3TRB3 = String.IsNullOrEmpty(a.O3TRB3) ? "-" : a.O3TRB3,
                            O3FGR = a.O3FGR,
                            O3FNAM = a.O3FNAM,
                            O3MODL = String.IsNullOrEmpty(a.O3MODL) ? "-" : a.O3MODL,
                            O3LONG = String.IsNullOrEmpty(a.O3LONG) ? "-" : a.O3LONG,
                            O3SLUM = a.O3SLUM,
                            O3DIV = a.O3DIV,
                            O3TUOM = a.O3TUOM,
                            O3DATE = a.O3DATE,
                            O3CURD = a.O3CURD,
                            O3USER = a.O3USER,
                            DateUpdated = a.DateUpdated,
                            SelectedTypeId = a.O3REGU == a.O3POS && a.O3EDT == 999999 ? 1 : 2,
                            SelectedCategoryId = (a.O3DEPT == 150 && (a.O3SDPT == 10 || a.O3SDPT == 12 || a.O3SDPT == 13 || a.O3SDPT == 14)) ||
                            (a.O3DEPT == 401 || a.O3DEPT == 402 || a.O3DEPT == 403 || a.O3DEPT == 404) ? 1 : 2,
                            IsExemp = (a.O3REG == a.O3POS && a.O3EDT != 999999) || (a.O3REG < a.O3POS && a.TypeId == 2) ? "Y" : "N",
                        }).FirstOrDefault();
            }
            else
            {
                var formattedCode = "";
                if (codeFormat == "UPC_A")
                {
                    formattedCode = BarcodeHelper.GetNormalizedUPC_A(query);
                }
                else if (codeFormat == "UPC_E")
                {
                    formattedCode = BarcodeHelper.GetNormalizedUPC_E(query);
                }

                formattedCode = formattedCode != "" ? formattedCode : query;

                var upc = _db.CompleteSTRPRCs.Where(f => f.IUPC.ToString() == formattedCode).FirstOrDefault();

                if (upc == null)
                    data = null;
                else
                {
                    data = (from a in _db.STRPRCs
                            where a.O3SKU == upc.INUMBR
                            select new STRPRCDto
                            {
                                O3LOC = a.O3LOC,
                                O3CLAS = a.O3CLAS,
                                O3IDSC = a.O3IDSC,
                                O3SKU = a.O3SKU,
                                O3SCCD = String.IsNullOrEmpty(a.O3SCCD) ? "-" : a.O3SCCD,
                                O3UPC = a.O3UPC,
                                O3VNUM = a.O3VNUM,
                                O3TYPE = a.O3TYPE,
                                O3DEPT = a.O3DEPT,
                                O3SDPT = a.O3SDPT,
                                O3SCLS = a.O3SCLS,
                                O3POS = a.O3POS,
                                O3POSU = a.O3POSU,
                                O3REG = a.O3REG,
                                O3REGU = a.O3REGU,
                                O3ORIG = a.O3ORIG,
                                O3ORGU = a.O3ORGU,
                                O3EVT = a.O3EVT,
                                O3PMMX = a.O3PMMX,
                                O3PMTH = a.O3PMTH,
                                O3PDQT = a.O3PDQT,
                                O3PDPR = a.O3PDPR,
                                O3SDT = a.O3SDT,
                                O3EDT = a.O3EDT,
                                O3TRB3 = String.IsNullOrEmpty(a.O3TRB3) ? "-" : a.O3TRB3,
                                O3FGR = a.O3FGR,
                                O3FNAM = a.O3FNAM,
                                O3MODL = String.IsNullOrEmpty(a.O3MODL) ? "-" : a.O3MODL,
                                O3LONG = String.IsNullOrEmpty(a.O3LONG) ? "-" : a.O3LONG,
                                O3SLUM = a.O3SLUM,
                                O3DIV = a.O3DIV,
                                O3TUOM = a.O3TUOM,
                                O3DATE = a.O3DATE,
                                O3CURD = a.O3CURD,
                                O3USER = a.O3USER,
                                DateUpdated = a.DateUpdated,
                                SelectedTypeId = a.O3REGU == a.O3POS && a.O3EDT == 999999 ? 1 : 2,
                                SelectedCategoryId = (a.O3DEPT == 150 && (a.O3SDPT == 10 || a.O3SDPT == 12 || a.O3SDPT == 13 || a.O3SDPT == 14)) ||
                                (a.O3DEPT == 401 || a.O3DEPT == 402 || a.O3DEPT == 403 || a.O3DEPT == 404) ? 1 : 2,
                                IsExemp = (a.O3REG == a.O3POS && a.O3EDT != 999999) || (a.O3REG < a.O3POS && a.TypeId == 2) ? "Y" : "N",
                            }).FirstOrDefault();
                }
                
            }

            return data;
        }

        public List<STRPRCDto> GetData(decimal O3SKU)
        {
            var data = new List<STRPRCDto>();

            // Set up the connection and command
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand("sp_GetDataBySKU", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                // Add parameters if required
                command.Parameters.AddWithValue("@O3SKU", O3SKU);

                // Open the connection and execute the command
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                // Process the result set
                while (reader.Read())
                {
                    var record = new STRPRCDto
                    {
                        O3LOC = (decimal)reader["O3LOC"],
                        O3CLAS = (decimal)reader["O3CLAS"],
                        O3IDSC = reader["O3IDSC"].ToString(),
                        O3SKU = (decimal)reader["O3SKU"],
                        O3SCCD = reader["O3SCCD"].ToString(),
                        O3UPC = (decimal)reader["O3UPC"],
                        O3VNUM = (decimal)reader["O3VNUM"],
                        O3TYPE = reader["O3TYPE"].ToString(),
                        O3DEPT = (decimal)reader["O3DEPT"],
                        O3SDPT = (decimal)reader["O3SDPT"],
                        O3SCLS = (decimal)reader["O3SCLS"],
                        O3POS = (decimal)reader["O3POS"],
                        O3POSU = (decimal)reader["O3POSU"],
                        O3REG = (decimal)reader["O3REG"],
                        O3REGU = (decimal)reader["O3REGU"],
                        O3ORIG = (decimal)reader["O3ORIG"],
                        O3ORGU = (decimal)reader["O3ORGU"],
                        O3EVT = (decimal)reader["O3EVT"],
                        O3PMMX = (decimal)reader["O3PMMX"],
                        O3PMTH = (decimal)reader["O3PMTH"],
                        O3PDQT = (decimal)reader["O3PDQT"],
                        O3PDPR = (decimal)reader["O3PDPR"],
                        O3SDT = (decimal)reader["O3SDT"],
                        O3EDT = (decimal)reader["O3EDT"],
                        O3TRB3 = reader["O3TRB3"].ToString(),
                        O3FGR = (decimal)reader["O3FGR"],
                        O3FNAM = reader["O3FNAM"].ToString(),
                        O3MODL = reader["O3MODL"].ToString(),
                        O3LONG = reader["O3LONG"].ToString(),
                        O3SLUM = reader["O3SLUM"].ToString(),
                        O3DIV = reader["O3DIV"].ToString(),
                        O3TUOM = reader["O3TUOM"].ToString(),
                        O3DATE = (decimal)reader["O3DATE"],
                        O3CURD = (decimal)reader["O3CURD"],
                        O3USER = reader["O3USER"].ToString(),
                        DateUpdated = (DateTime)reader["DateUpdated"]
                    };

                    data.Add(record);
                }

                // Close the reader and connection
                reader.Close();
                connection.Close();
            }

            return data;
        }

        public IEnumerable<STRPRC> GetAllData()
        {
            return _db.STRPRCs;
        }

        public async Task<List<STRPRCDto>> GetDataByStartDate(decimal startDate)
        {
            var sp = "sp_GettmpData";
            var data = new List<STRPRCDto>();
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
                    var record = new STRPRCDto
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
                        ZeroInvInTransit = (decimal)reader["ZeroInvInTransit"]
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

        public async Task<bool> UpdateCentralizedExemptions(decimal startDate)
        {
            try
            {
                var sp = "sp_GettmpData";
                var data = new List<CentralizedSTRPRCDto>();
                // Set up the connection and command
                using (var connection = new SqlConnection(connectionString))
                using (var command = new SqlCommand(sp, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = commandTimeoutInSeconds;

                    // Add parameters if required
                    command.Parameters.AddWithValue("@O3SDT", startDate);
                    command.Parameters.AddWithValue("@O3LOC", _storeID);

                    // Open the connection and execute the command
                    connection.Open();
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    // Process the result set
                    while (reader.Read())
                    {
                        var record = new CentralizedSTRPRCDto
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
                            StoreID = _storeID
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

                var exemptions = data.Where(w => w.IsExemp == "Y").ToList();

                var dataExemp = new List<ExemptionDto>();
                using (var connection = new SqlConnection(connStringCentralizedExemptions))
                using (var command = new SqlCommand("sp_GetExemptionsRaw", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = commandTimeoutInSeconds;
                    command.Parameters.AddWithValue("@O3LOC", _storeID);
                    connection.Open();
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    while (reader.Read())
                    {
                        var record = new ExemptionDto
                        {
                            Id = (int)reader["Id"],
                            O3SKU = (decimal)reader["O3SKU"],
                            O3UPC = (decimal)reader["O3UPC"],
                            O3IDSC = reader["O3IDSC"].ToString(),
                            StoreID = (int)reader["StoreID"],
                            DateExemption = Convert.ToDateTime(reader["DateExemption"]),
                        };

                        dataExemp.Add(record);
                    }
                    reader.Close();
                    connection.Close();
                }
                //pca with exemptions that are in merchandise but not in new pca
                var result = dataExemp.Where(exemp => !exemptions.Select(s => s.O3SKU).Contains(exemp.O3SKU)).ToList();

                //delete pca with exemptions that are in merchandise but not in new pca
                if (result.Count > 0)
                {
                    using (SqlConnection connection = new SqlConnection(connStringCentralizedExemptions))
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand($"DELETE FROM Exemptions WHERE Id IN ({string.Join(",", result.Select((_, i) => $"@id{i}"))})", connection))
                        {
                            for (int i = 0; i < result.Count; i++)
                            {
                                command.Parameters.AddWithValue($"@id{i}", result[i].Id);
                            }

                            int rowsAffected = command.ExecuteNonQuery();
                            Console.WriteLine($"Rows affected: {rowsAffected}");
                        }
                    }
                }

                string insertQuery = "INSERT INTO Exemptions (IsPrinted, O3SKU, O3UPC, O3IDSC, o3type, O3REG," +
                                    " O3POS, O3SDT, O3EDT, O3RSDT, O3REDT, TypeId, SizeId, CategoryId, DPTNAM, O3FLAG1, INV2, IsExemp, NegativeSave, IBHAND, StoreID) " +
                                     "VALUES (@IsPrinted, @O3SKU, @O3UPC, @O3IDSC, @o3type, @O3REG," +
                                    " @O3POS, @O3SDT, @O3EDT, @O3RSDT, @O3REDT, @TypeId, @SizeId, @CategoryId, @DPTNAM, @O3FLAG1, @INV2, @IsExemp, @NegativeSave, @IBHAND, @StoreID)";

                var listOfIds = new List<int>();

                using (SqlConnection connection = new SqlConnection(connStringCentralizedExemptions))
                {
                    connection.Open();

                    foreach (var exemption in exemptions)
                    {
                        if (dataExemp.Where(s => s.O3SKU == exemption.O3SKU).Count() > 0)
                            continue;

                        using (SqlCommand command = new SqlCommand(insertQuery, connection))
                        {
                            // Add parameters to the SQL command
                            command.Parameters.AddWithValue("@IsPrinted", 0);
                            command.Parameters.AddWithValue("@O3SKU", exemption.O3SKU);
                            command.Parameters.AddWithValue("@O3UPC", exemption.O3UPC);
                            command.Parameters.AddWithValue("@O3IDSC", exemption.O3IDSC);
                            command.Parameters.AddWithValue("@o3type", exemption.O3TYPE);
                            command.Parameters.AddWithValue("@O3REG", exemption.O3REG);
                            command.Parameters.AddWithValue("@O3POS", exemption.O3POS);
                            command.Parameters.AddWithValue("@O3SDT", exemption.O3SDT);
                            command.Parameters.AddWithValue("@O3EDT", exemption.O3EDT);
                            command.Parameters.AddWithValue("@O3RSDT", exemption.O3RSDT);
                            command.Parameters.AddWithValue("@O3REDT", exemption.O3REDT);
                            command.Parameters.AddWithValue("@TypeId", exemption.TypeId);
                            command.Parameters.AddWithValue("@SizeId", exemption.SizeId);
                            command.Parameters.AddWithValue("@CategoryId", exemption.CategoryId);
                            command.Parameters.AddWithValue("@DPTNAM", exemption.DepartmentName);
                            command.Parameters.AddWithValue("@O3FLAG1", exemption.IsReverted);
                            command.Parameters.AddWithValue("@INV2", exemption.HasInventory);
                            command.Parameters.AddWithValue("@IsExemp", exemption.IsExemp);
                            command.Parameters.AddWithValue("@NegativeSave", exemption.NegativeSave);
                            command.Parameters.AddWithValue("@IBHAND", exemption.IBHAND);
                            command.Parameters.AddWithValue("@StoreID", exemption.StoreID);

                            // Execute the SQL command
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public STRPRCDto GetSKUDetails(decimal O3SKU)
        {
            var sp = "sp_GetSKUDetails";
            var data = new STRPRCDto();
            // Set up the connection and command
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(sp, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = commandTimeoutInSeconds;

                // Add parameters if required
                command.Parameters.AddWithValue("@O3SKU", O3SKU);

                // Open the connection and execute the command
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                // Process the result set
                if (reader.Read())
                {
                    var record = new STRPRCDto
                    {
                        O3SKU = (decimal)reader["O3SKU"],
                        O3FNAM = reader["O3FNAM"].ToString(),
                        O3MODL = reader["O3MODL"].ToString(),
                        O3DEPT = (decimal)reader["O3DEPT"],
                        O3SDPT = (decimal)reader["O3SDPT"],
                        O3CLAS = (decimal)reader["O3CLAS"],
                        O3SCLS = (decimal)reader["O3SCLS"],
                        O3SCCD = reader["O3SCCD"].ToString(),
                        O3TRB3 = reader["O3TRB3"].ToString(),
                        O3LONG = reader["O3LONG"].ToString()
                    };

                    data = record;
                }

                // Close the reader and connection
                reader.Close();
                connection.Close();
            }

            return data;

        }

        public List<STRPRCLogDto> GetUpdatedData(decimal o3sku = 0)
        {
            var records = new List<STRPRCLogDto>();
            // Set up the connection and command
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand("sp_GetUpdatedData", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = commandTimeoutInSeconds;

                command.Parameters.Add("@o3loc", SqlDbType.Decimal).Value = ConfigurationManager.AppSettings["StoreID"];
                if (o3sku > 0)
                    command.Parameters.Add("@o3sku", SqlDbType.Decimal).Value = o3sku;

                // Open the connection and execute the command
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                // Process the result set
                while (reader.Read())
                {
                    var record = new STRPRCLogDto
                    {
                        O3SKU = (decimal)reader["O3SKU"],
                        IsPrinted = (bool)reader["IsPrinted"] ? "Yes" : "No",
                        O3IDSC = reader["O3IDSC"].ToString(),
                        ColumnName = reader["ColumnName"].ToString(),
                        DateUpdated = (DateTime)reader["DateUpdated"],
                        O3FNAM = reader["O3FNAM"].ToString(),
                        O3DEPT = (decimal)reader["O3DEPT"],
                        DepartmentName = reader["DepartmentName"].ToString(),
                        O3CLAS = (decimal)reader["O3CLAS"],
                        O3SCCD = reader["O3SCCD"].ToString(),
                        O3LONG = reader["O3LONG"].ToString(),
                        O3MODL = reader["O3MODL"].ToString(),
                        O3SDPT = (decimal)reader["O3SDPT"],
                        O3SCLS = (decimal)reader["O3SCLS"],
                        O3TRB3 = reader["O3TRB3"].ToString(),
                        Inv = reader["INV2"].ToString(),

                        //O3LOC = (decimal)reader["O3LOC"],
                        //O3UPC = (decimal)reader["O3UPC"],
                        //O3VNUM = (decimal)reader["O3VNUM"],
                        //O3TYPE = reader["O3TYPE"].ToString(),
                        //O3POS = (decimal)reader["O3POS"],
                        //O3POSU = (decimal)reader["O3POSU"],
                        //O3REG = (decimal)reader["O3REG"],
                        //O3REGU = (decimal)reader["O3REGU"],
                        //O3ORIG = (decimal)reader["O3ORIG"],
                        //O3ORGU = (decimal)reader["O3ORGU"],
                        //O3EVT = (decimal)reader["O3EVT"],
                        //O3PMMX = (decimal)reader["O3PMMX"],
                        //O3PMTH = (decimal)reader["O3PMTH"],
                        //O3PDQT = (decimal)reader["O3PDQT"],
                        //O3PDPR = (decimal)reader["O3PDPR"],
                        //O3SDT = (decimal)reader["O3SDT"],
                        //O3EDT = (decimal)reader["O3EDT"],
                        //O3TRB3 = reader["O3TRB3"].ToString(),
                        //O3FGR = (decimal)reader["O3FGR"],
                        //O3SLUM = reader["O3SLUM"].ToString(),
                        //O3DIV = reader["O3DIV"].ToString(),
                        //O3TUOM = reader["O3TUOM"].ToString(),
                        //O3DATE = (decimal)reader["O3DATE"],
                        //O3CURD = (decimal)reader["O3CURD"],
                        //O3USER = reader["O3USER"].ToString(),

                        TypeName = reader["TypeName"].ToString(),
                        SizeName = reader["SizeName"].ToString(),
                        CategoryName = reader["CategoryName"].ToString()
                        //Id = (int)reader["Id"],
                        //FromValue = reader["FromValue"].ToString(),
                        //ToValue = reader["IdToValue"].ToString()
                    };

                    records.Add(record);
                }

                // Close the reader and connection
                reader.Close();
                connection.Close();
            }

            return records;
        }

        public STRPRCDto GetDataBySKU(decimal O3SKU)
        {
            var data = (from a in _db.STRPRCs
                        where a.O3SKU == O3SKU
                        select new STRPRCDto
                        {
                            O3LOC = a.O3LOC,
                            O3CLAS = a.O3CLAS,
                            O3IDSC = a.O3IDSC,
                            O3SKU = a.O3SKU,
                            O3SCCD = a.O3SCCD,
                            O3UPC = a.O3UPC,
                            O3VNUM = a.O3VNUM,
                            O3TYPE = a.O3TYPE,
                            O3DEPT = a.O3DEPT,
                            O3SDPT = a.O3SDPT,
                            O3SCLS = a.O3SCLS,
                            O3POS = a.O3POS,
                            O3POSU = a.O3POSU,
                            O3REG = a.O3REG,
                            O3REGU = a.O3REGU,
                            O3ORIG = a.O3ORIG,
                            O3ORGU = a.O3ORGU,
                            O3EVT = a.O3EVT,
                            O3PMMX = a.O3PMMX,
                            O3PMTH = a.O3PMTH,
                            O3PDQT = a.O3PDQT,
                            O3PDPR = a.O3PDPR,
                            O3SDT = a.O3SDT,
                            O3EDT = a.O3EDT,
                            O3TRB3 = a.O3TRB3,
                            O3FGR = a.O3FGR,
                            O3FNAM = a.O3FNAM,
                            O3MODL = a.O3MODL,
                            O3LONG = a.O3LONG,
                            O3SLUM = a.O3SLUM,
                            O3DIV = a.O3DIV,
                            O3TUOM = a.O3TUOM,
                            O3DATE = a.O3DATE,
                            O3CURD = a.O3CURD,
                            O3USER = a.O3USER,
                            DateUpdated = a.DateUpdated,
                            TypeId = a.TypeId,
                            SizeId = a.SizeId,
                            CategoryId = a.CategoryId
                        }).FirstOrDefault();

            return data;
        }

        public STRPRCDto GetLatestUpdate()
        {
            var data = (from a in _db.STRPRCs
                        orderby a.DateUpdated descending
                        select new STRPRCDto
                        {
                            DateUpdated = a.DateUpdated,
                            LatestDate = a.O3DATE
                        }).FirstOrDefault();

            return data;
        }

        public bool GetLatestInventory(string storeId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("sp_GetLatestInventory", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = commandTimeoutInSeconds;
                        // Add any required parameters to the command if needed
                        command.Parameters.AddWithValue("@Store", storeId);

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

        public decimal CheckSTRPRCUpdates(int storeId)
        {
            decimal date = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString151))
                {
                    using (SqlCommand command = new SqlCommand("sp_GetLatestUpdate", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = commandTimeoutInSeconds;
                        // Add any required parameters to the command if needed
                        command.Parameters.AddWithValue("@Store", storeId);

                        connection.Open();

                        // Execute the command and retrieve the result count
                        date = (decimal)command.ExecuteScalar();
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error executing stored procedure: " + ex.Message);
            }
            return date;
        }

        public async Task UpdateSTRPRC151(int storeId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString151))
                {
                    using (SqlCommand command = new SqlCommand("sp_GetSTRPRC", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = commandTimeoutInSeconds;
                        // Add any required parameters to the command if needed
                        command.Parameters.AddWithValue("@Store", storeId);

                        connection.Open();

                        // Execute the command and retrieve the result count
                        await command.ExecuteNonQueryAsync();
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error executing stored procedure: " + ex.Message);
            }
        }

        public async Task PreSTRPRCUpdate()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("sp_PreSTRPRCUpdate", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = commandTimeoutInSeconds;
                        command.Parameters.AddWithValue("@O3LOC", _storeID);
                        connection.Open();
                        await command.ExecuteNonQueryAsync();
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error executing stored procedure: " + ex.Message);
            }
        }

        public async Task PostSTRPRCUpdate()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("sp_PostSTRPRCUpdate", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = commandTimeoutInSeconds;
                        // Add any required parameters to the command if needed
                        command.Parameters.AddWithValue("@O3LOC", _storeID);
                        connection.Open();
                        await command.ExecuteScalarAsync();
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error executing stored procedure: " + ex.Message);
            }
        }

        [Obsolete]
        public bool CheckSTRPRCFromRemote()
        {
            try
            {
                using (Session session = new Session())
                {
                    session.Open(FTPService.GetSessionOptions(_server, _protocol));
                    string remoteFilePath = ConfigurationManager.AppSettings["DataConRemotePath"];

                    string formattedDate = DateTime.Now.ToString("MMddyy");
                    //string fileName = _storeID.ToString() + $"_STRPRC_{formattedDate}.zip";
                    string fileName = _storeID.ToString() + "_STRPRC_031924.zip";
                    string localFilePath = LocalPath.CreateLocalFilePath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "exported_strprc\\"));
                    string extractedFilePath = LocalPath.CreateLocalFilePath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "exported_strprc\\extracted\\"));

                    string[] zipFiles = Directory.GetFiles(localFilePath);
                    foreach (string file in zipFiles)
                    {
                        if (Path.GetExtension(file).Equals(".zip", StringComparison.OrdinalIgnoreCase))
                            File.Delete(file);
                    }

                    string[] textFiles = Directory.GetFiles(extractedFilePath);
                    foreach (string file in textFiles)
                    {
                        if (Path.GetExtension(file).Equals(".txt", StringComparison.OrdinalIgnoreCase))
                            File.Delete(file);
                    }

                    TransferOperationResult transferResult;
                    transferResult = session.GetFiles(Path.Combine(remoteFilePath, fileName), localFilePath); //Copy 201_STRPRC_031924.zip file to local file path

                    if (transferResult.IsSuccess)
                    {
                        ZipFile.ExtractToDirectory(localFilePath + fileName, extractedFilePath);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        [Obsolete]
        public List<STRPRCBulkDto> GetSTRPRCList()
        {
            var list = new List<STRPRCBulkDto>();
            string extractedFilePath = LocalPath.CreateLocalFilePath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "exported_strprc\\extracted\\"));

            // Read all lines from the file
            string[] lines = System.IO.File.ReadAllLines(Path.Combine(extractedFilePath, _storeID.ToString() + "_STRPRC.txt"));

            // Convert each string to a list of strings
            List<List<string>> strprcList = new List<List<string>>();

            foreach (string str in lines)
            {
                string[] splitStrings = str.Split('|');

                if (splitStrings.Length > 43)
                    continue;

                strprcList.Add(new List<string>(splitStrings));
            }

            foreach (var strprc in strprcList)
            {
                var data = new STRPRCBulkDto();
                data.O3LOC = decimal.Parse(strprc[0]);
                data.O3SKU = decimal.Parse(strprc[1]);
                data.O3SCCD = strprc[2];
                data.O3IDSC = strprc[3];
                data.O3UPC = decimal.Parse(strprc[4]);
                data.O3VNUM = decimal.Parse(strprc[5]);
                data.O3TYPE = strprc[6];
                data.O3DEPT = decimal.Parse(strprc[7]);
                data.O3SDPT = decimal.Parse(strprc[8]);
                data.O3CLAS = decimal.Parse(strprc[9]);
                data.O3SCLS = decimal.Parse(strprc[10]);
                data.O3SDSC = strprc[11];
                data.O3POS = decimal.Parse(strprc[12]);
                data.O3POSU = decimal.Parse(strprc[13]);
                data.O3REG = decimal.Parse(strprc[14]);
                data.O3REGU = decimal.Parse(strprc[15]);
                data.O3ORIG = decimal.Parse(strprc[16]);
                data.O3ORGU = decimal.Parse(strprc[17]);
                data.O3EVT = decimal.Parse(strprc[18]);
                data.O3REVT = decimal.Parse(strprc[19]);
                data.O3PMMX = decimal.Parse(strprc[20]);
                data.O3PMTH = decimal.Parse(strprc[21]);
                data.O3PDQT = decimal.Parse(strprc[22]);
                data.O3PDPR = decimal.Parse(strprc[23]);
                data.O3SDT = decimal.Parse(strprc[24]);
                data.O3EDT = decimal.Parse(strprc[25]);
                data.O3RSDT = decimal.Parse(strprc[26]);
                data.O3REDT = decimal.Parse(strprc[27]);
                data.O3TRB3 = strprc[28];
                data.O3FGR = decimal.Parse(strprc[29]);
                data.O3FNAM = strprc[30];
                data.O3MSRP = decimal.Parse(strprc[31]);
                data.O3MODL = strprc[32];
                data.O3LONG = strprc[33];
                data.O3SLUM = strprc[34];
                data.O3DIV = strprc[35];
                data.O3TUOM = strprc[36];
                data.O3DATE = decimal.Parse(strprc[37]);
                data.O3CURD = decimal.Parse(strprc[38]);
                data.O3USER = strprc[39];
                data.O3FLAG1 = strprc[40];
                data.O3FLAG2 = strprc[41];
                list.Add(data);
            }

            return list;
        }

        public CountryDto GetCountryImg(string country)
        {
            var data = (from a in _db.Countries
                        where a.iatrb3 == country
                        select new CountryDto
                        {
                            iatrb3 = a.iatrb3,
                            country_img = a.country_img
                        }).FirstOrDefault();

            return data;
        }

        public ReportDto GetReportData(decimal O3SKU)
        {
            var data = (from a in _db.STRPRCs
                        join b in _db.Countries on a.O3TRB3 equals b.iatrb3 into ab
                        from c in ab.DefaultIfEmpty()
                        where a.O3SKU == O3SKU
                        select new ReportDto
                        {
                            O3LOC = a.O3LOC,
                            O3CLAS = a.O3CLAS,
                            O3IDSC = a.O3IDSC,
                            O3SKU = a.O3SKU,
                            O3SCCD = a.O3SCCD,
                            O3UPC = a.O3UPC,
                            O3VNUM = a.O3VNUM,
                            O3TYPE = a.O3TYPE,
                            O3DEPT = a.O3DEPT,
                            O3SDPT = a.O3SDPT,
                            O3SCLS = a.O3SCLS,
                            O3POS = a.O3POS,
                            O3POSU = a.O3POSU,
                            O3REG = a.O3REG,
                            O3REGU = a.O3REGU,
                            O3ORIG = a.O3ORIG,
                            O3ORGU = a.O3ORGU,
                            O3EVT = a.O3EVT,
                            O3PMMX = a.O3PMMX,
                            O3PMTH = a.O3PMTH,
                            O3PDQT = a.O3PDQT,
                            O3PDPR = a.O3PDPR,
                            O3SDT = a.O3SDT,
                            O3EDT = a.O3EDT,
                            O3TRB3 = a.O3TRB3,
                            O3FGR = a.O3FGR,
                            O3FNAM = a.O3FNAM,
                            O3MODL = a.O3MODL,
                            O3LONG = a.O3LONG,
                            O3SLUM = a.O3SLUM,
                            O3DIV = a.O3DIV,
                            O3TUOM = a.O3TUOM,
                            O3DATE = a.O3DATE,
                            O3CURD = a.O3CURD,
                            O3USER = a.O3USER,
                            DateUpdated = a.DateUpdated,
                            TypeId = a.TypeId,
                            SizeId = a.SizeId,
                            CategoryId = a.CategoryId,
                            country_img = c.country_img
                        }).FirstOrDefault();

            return data;
        }

        public List<ReportDto> GetReportDataList(List<decimal> O3SKUs)
        {
            var data = (from a in _db.STRPRCs
                        join b in _db.Countries on a.O3TRB3 equals b.iatrb3 into ab
                        from c in ab.DefaultIfEmpty()
                        where O3SKUs.Contains(a.O3SKU)
                        select new ReportDto
                        {
                            O3LOC = a.O3LOC,
                            O3CLAS = a.O3CLAS,
                            O3IDSC = a.O3IDSC,
                            O3SKU = a.O3SKU,
                            O3SCCD = a.O3SCCD,
                            O3UPC = a.O3UPC,
                            O3VNUM = a.O3VNUM,
                            O3TYPE = a.O3TYPE,
                            O3DEPT = a.O3DEPT,
                            O3SDPT = a.O3SDPT,
                            O3SCLS = a.O3SCLS,
                            O3POS = a.O3POS,
                            O3POSU = a.O3POSU,
                            O3REG = a.O3REG,
                            O3REGU = a.O3REGU,
                            O3ORIG = a.O3ORIG,
                            O3ORGU = a.O3ORGU,
                            O3EVT = a.O3EVT,
                            O3PMMX = a.O3PMMX,
                            O3PMTH = a.O3PMTH,
                            O3PDQT = a.O3PDQT,
                            O3PDPR = a.O3PDPR,
                            O3SDT = a.O3SDT,
                            O3EDT = a.O3EDT,
                            O3TRB3 = a.O3TRB3,
                            O3FGR = a.O3FGR,
                            O3FNAM = a.O3FNAM,
                            O3MODL = a.O3MODL,
                            O3LONG = a.O3LONG,
                            O3SLUM = a.O3SLUM,
                            O3DIV = a.O3DIV,
                            O3TUOM = a.O3TUOM,
                            O3DATE = a.O3DATE,
                            O3CURD = a.O3CURD,
                            O3USER = a.O3USER,
                            DateUpdated = a.DateUpdated,
                            TypeId = a.TypeId,
                            SizeId = a.SizeId,
                            CategoryId = a.CategoryId,
                            country_img = c.country_img
                        }).ToList();

            return data;
        }

        public void UpdateSelection(decimal startDate, decimal endDate)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string storedProcedureName = "sp_UpdateSTRPRCSelection";
                using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@O3SDT", startDate);
                    command.Parameters.AddWithValue("@O3EDT", endDate);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateMultipleStatus(List<decimal> o3skus)
        {
            foreach (var item in o3skus)
            {
                var data = _db.STRPRCs.Where(a => a.O3SKU == item).FirstOrDefault();
                data.IsPrinted = true;

                var logs = _db.STRPRCLogs.Where(a => a.O3SKU == item).ToList();

                foreach (var log in logs)
                {
                    log.IsPrinted = true;
                }
            }

            _db.SaveChanges();

        }

        public void AddMultipleInventoryPrintingLog(List<decimal> o3skus, string user)
        {
            foreach (var item in o3skus)
            {
                _db.InventoryPrintingLogs.Add(new InventoryPrintingLog()
                {
                    O3SKU = item,
                    PrintedBy = user,
                    DateCreated = DateTime.Now
                });
            }
            _db.SaveChanges();
        }

        public void UpdateSingleStatus(decimal O3SKU)
        {
            var data = _db.STRPRCs.Where(a => a.O3SKU == O3SKU).FirstOrDefault();
            data.IsPrinted = true;

            var logs = _db.STRPRCLogs.Where(a => a.O3SKU == O3SKU).ToList();

            foreach (var log in logs)
            {
                log.IsPrinted = true;
            }

            _db.SaveChanges();
        }

        public void AddInventoryPrintingLog(ReportDto model, string user)
        {

            var data = new InventoryPrintingLog()
            {
                O3SKU = model.O3SKU,
                PrintedBy = user,
                DateCreated = DateTime.Now,
                RegularPrice = model.O3REGU,
                CurrentPrice = model.O3POS,
                Remarks = model.qRemarks,
                ItemDesc = model.O3IDSC,
                Brand = model.O3FNAM,
                Model = model.O3MODL,
                Divisor = model.O3DIV
            };
            _db.InventoryPrintingLogs.Add(data);
            _db.SaveChanges();
        }

        public List<STRPRCDto> GetLatestPCAData()
        {
            var sp = "sp_GetLatestPCAData";
            var data = new List<STRPRCDto>();
            // Set up the connection and command
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(sp, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = commandTimeoutInSeconds;

                // Add parameters if required
                //command.Parameters.AddWithValue("@Filter", filter);

                // Open the connection and execute the command
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                // Process the result set
                while (reader.Read())
                {
                    var record = new STRPRCDto
                    {
                        O3LOC = (decimal)reader["O3LOC"],
                        O3CLAS = (decimal)reader["O3CLAS"],
                        O3IDSC = reader["O3IDSC"].ToString(),
                        O3SKU = (decimal)reader["O3SKU"],
                        O3SCCD = reader["O3SCCD"].ToString(),
                        O3UPC = (decimal)reader["O3UPC"],
                        O3VNUM = (decimal)reader["O3VNUM"],
                        O3TYPE = reader["O3TYPE"].ToString(),
                        O3DEPT = (decimal)reader["O3DEPT"],
                        O3SDPT = (decimal)reader["O3SDPT"],
                        O3SCLS = (decimal)reader["O3SCLS"],
                        O3POS = (decimal)reader["O3POS"],
                        O3POSU = (decimal)reader["O3POSU"],
                        O3REG = (decimal)reader["O3REG"],
                        O3REGU = (decimal)reader["O3REGU"],
                        O3ORIG = (decimal)reader["O3ORIG"],
                        O3ORGU = (decimal)reader["O3ORGU"],
                        O3EVT = (decimal)reader["O3EVT"],
                        O3PMMX = (decimal)reader["O3PMMX"],
                        O3PMTH = (decimal)reader["O3PMTH"],
                        O3PDQT = (decimal)reader["O3PDQT"],
                        O3PDPR = (decimal)reader["O3PDPR"],
                        O3SDT = (decimal)reader["O3SDT"],
                        O3EDT = (decimal)reader["O3EDT"],
                        O3TRB3 = reader["O3TRB3"].ToString(),
                        O3FGR = (decimal)reader["O3FGR"],
                        O3FNAM = reader["O3FNAM"].ToString(),
                        O3MODL = reader["O3MODL"].ToString(),
                        O3LONG = reader["O3LONG"].ToString(),
                        O3SLUM = reader["O3SLUM"].ToString(),
                        O3DIV = reader["O3DIV"].ToString(),
                        O3TUOM = reader["O3TUOM"].ToString(),
                        O3DATE = (decimal)reader["O3DATE"],
                        O3CURD = (decimal)reader["O3CURD"],
                        O3USER = reader["O3USER"].ToString(),
                        DateUpdated = (DateTime)reader["DateUpdated"],
                        TypeId = (int)reader["Type_STRPRC"],
                        SizeId = (int)reader["Size_STRPRC"],
                        CategoryId = (int)reader["Category_STRPRC"],
                        DepartmentName = reader["DPTNAM"].ToString(),
                        IsReverted = reader["O3FLAG1"].ToString(),
                        IsPrinted = reader["IsPrinted_STRPRC"].ToString(),
                        LatestDate = (decimal)reader["LatestDate"],
                        HasInventory = reader["INV2"].ToString(),
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

        public List<ExportPCAExemptionDto> PCAToExportExemption()
        {
            var sp = "sp_NewExportPCAData";

            var data = new List<ExportPCAExemptionDto>();
            // Set up the connection and command
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(sp, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = commandTimeoutInSeconds;

                // Add parameters if required
                //command.Parameters.AddWithValue("@SearchDate", date);

                // Open the connection and execute the command
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                // Process the result set
                while (reader.Read())
                {
                    var record = new ExportPCAExemptionDto
                    {
                        SKU = (decimal)reader["O3SKU"],
                        UPC = (decimal)reader["O3UPC"],
                        CurrentPrice = (decimal)reader["O3POS"],
                        RegularPrice = (decimal)reader["O3REGU"],
                        StartDate = (decimal)reader["O3SDT"],
                        EndDate = (decimal)reader["O3EDT"],
                        Brand = reader["O3FNAM"].ToString(),
                        Model = reader["O3MODL"].ToString(),
                        LongDesc = reader["O3LONG"].ToString(),
                        ItemDesc = reader["O3IDSC"].ToString(),
                        Type = reader["Type"].ToString(),
                        Size = reader["Size"].ToString(),
                        Category = reader["Category"].ToString(),
                        DepartmentName = reader["DPTNAM"].ToString(),
                        IsPrinted = reader["IsPrintedYN"].ToString(),
                        WithInventory = reader["INVYN"].ToString(),
                        IsExemption = reader["IsExemp"].ToString(),
                        ExemptionType = reader["ExemptionType"].ToString()
                    };

                    data.Add(record);
                }

                // Close the reader and connection
                reader.Close();
                connection.Close();
            }

            return data;
        }

        public List<ExportPCADto> PCAToExport()
        {
            var sp = "sp_NewExportPCAData";

            var data = new List<ExportPCADto>();
            // Set up the connection and command
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(sp, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = commandTimeoutInSeconds;

                // Add parameters if required
                //command.Parameters.AddWithValue("@SearchDate", date);

                // Open the connection and execute the command
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                // Process the result set
                while (reader.Read())
                {
                    var record = new ExportPCADto
                    {
                        SKU = (decimal)reader["O3SKU"],
                        UPC = (decimal)reader["O3UPC"],
                        CurrentPrice = (decimal)reader["O3POS"],
                        RegularPrice = (decimal)reader["O3REGU"],
                        StartDate = (decimal)reader["O3SDT"],
                        EndDate = (decimal)reader["O3EDT"],
                        Brand = reader["O3FNAM"].ToString(),
                        Model = reader["O3MODL"].ToString(),
                        LongDesc = reader["O3LONG"].ToString(),
                        ItemDesc = reader["O3IDSC"].ToString(),
                        Type = reader["Type"].ToString(),
                        Size = reader["Size"].ToString(),
                        Category = reader["Category"].ToString(),
                        DepartmentName = reader["DPTNAM"].ToString(),
                        IsPrinted = reader["IsPrintedYN"].ToString(),
                        WithInventory = reader["INVYN"].ToString(),
                        IsExemption = reader["IsExemp"].ToString(),
                        O3TYPE = reader["O3TYPE"].ToString()
                    };

                    data.Add(record);
                }

                // Close the reader and connection
                reader.Close();
                connection.Close();
            }

            return data;
        }



        public string GetSubClassDescription(decimal O3SKU)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand($"SELECT TOP 1 O3SDSC FROM STRPRCs WHERE O3SKU = {O3SKU}", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                            return reader.GetString(0);
                        else
                            return null;
                    }
                }
            }
        }

        public async Task<CentralizedExemptionStatusDto> CheckCentralizedExemptionStatus()
        {
            var centralizedExDto = new CentralizedExemptionStatusDto();
            try
            {
                using (SqlConnection connection = new SqlConnection(connStringCentralizedExemptions))
                {
                    using (SqlCommand command = new SqlCommand("sp_GetExemptionStatus", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = commandTimeoutInSeconds;
                        command.Parameters.AddWithValue("@StoreId", _storeID);
                        connection.Open();
                        SqlDataReader reader = await command.ExecuteReaderAsync();

                        while (reader.Read())
                        {
                            var record = new CentralizedExemptionStatusDto
                            {
                                Id = (int)reader["Id"],
                                StoreId = (int)reader["StoreId"],
                                DateUpdated = !reader.IsDBNull(reader.GetOrdinal("DateUpdated")) ? Convert.ToDateTime(reader["DateUpdated"]) : DateTime.Now.AddDays(-1),
                                OngoingUpdate = (bool)reader["OngoingUpdate"],
                            };

                            centralizedExDto = record;
                        }
                        reader.Close();
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error executing stored procedure: " + ex.Message);
            }
            return centralizedExDto;
        }

        public void UpdateCentralizedExemptionStatus(CentralizedExemptionStatusDto data, bool onGoingUpdate)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connStringCentralizedExemptions))
                {
                    connection.Open();
                    if (data.Id > 0)
                    {
                        var query = "UPDATE ExemptionStatus SET DateUpdated = @DateUpdated," +
                                        " OngoingUpdate = @OngoingUpdate WHERE StoreId = @StoreId";

                        if (onGoingUpdate)
                            query = "UPDATE ExemptionStatus SET" +
                                        " OngoingUpdate = @OngoingUpdate WHERE StoreId = @StoreId";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@StoreId", _storeID);
                            command.Parameters.AddWithValue("@DateUpdated", DateTime.Now);
                            command.Parameters.AddWithValue("@OngoingUpdate", onGoingUpdate);
                            int rowsAffected = command.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        using (SqlCommand command = new SqlCommand("INSERT INTO ExemptionStatus (StoreId,DateUpdated,OngoingUpdate)" +
                                                                    " VALUES (@StoreId, @DateUpdated, @OngoingUpdate)", connection))
                        {
                            command.Parameters.AddWithValue("@StoreId", _storeID);
                            command.Parameters.AddWithValue("@DateUpdated", DateTime.Now.AddDays(-1));
                            command.Parameters.AddWithValue("@OngoingUpdate", onGoingUpdate);
                            int rowsAffected = command.ExecuteNonQuery();
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error executing stored procedure: " + ex.Message);
            }
        }
    }
}