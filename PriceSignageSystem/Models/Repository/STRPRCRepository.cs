﻿using Org.BouncyCastle.Asn1.Pkcs;
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

namespace PriceSignageSystem.Models.Repository
{
    public class STRPRCRepository : ISTRPRCRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly string connectionString;
        private readonly string connectionString151;
        private readonly string connStringCentralizedExemptions;
        private readonly int commandTimeoutInSeconds;

        public STRPRCRepository(ApplicationDbContext db)
        {
            _db = db;
            connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
            connectionString151 = ConfigurationManager.ConnectionStrings["MyConnectionString151"].ConnectionString;
            connStringCentralizedExemptions = ConfigurationManager.ConnectionStrings["ConnStringCentralizedExemptions"].ConnectionString;
            commandTimeoutInSeconds = 3600;

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
                var formattedCode = string.Empty;
                decimal parsedCode = default(decimal);
                if (query.Contains("\r\n"))
                {
                    query = query.Replace("\r\n", "");
                }
                if (codeFormat == "UPC_A")
                {
                    formattedCode = BarcodeHelper.GetNormalizedUPC_A(query);
                }
                else if (codeFormat == "UPC_E")
                {
                    formattedCode = BarcodeHelper.ConvertUPCEToUPCA(query);
                }
                parsedCode = ((formattedCode != string.Empty) ? decimal.Parse(formattedCode) : decimal.Parse(query));
                var upc = _db.CompleteSTRPRCs.Where(f => f.IUPC == parsedCode).FirstOrDefault();

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
                        ExempType = reader["ExempType"].ToString(),
                        O3TYPE = reader["O3TYPE"].ToString(),
                        IBHAND = (decimal)reader["IBHAND"],
                        ZeroInvDCOnHand = (decimal)reader["ZeroInvDCOnHand"],
                        ZeroInvInTransit = (decimal)reader["ZeroInvInTransit"],
                        IsNotRequired = reader["IsNotRequired"].ToString(),
                        SizeName = reader["SizeName"].ToString(),
                        TypeName = reader["TypeName"].ToString(),
                        CategoryName = reader["CategoryName"].ToString(),
                        IsDoublePromo = reader["IsDoublePromo"].ToString()
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

        public async Task<bool> UpdateCentralizedExemptions(decimal startDate)
        {
            try
            {
                var sp = "sp_GettmpData";
                var data = new List<CentralizedSTRPRCDto>();
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
                            HasCoContract = reader["O3FLAG3"].ToString(),
                            HasInventory = reader["INV2"].ToString(),
                            IsExemp = reader["IsExemp"].ToString(),
                            NegativeSave = reader["NegativeSave"].ToString(),
                            O3TYPE = reader["O3TYPE"].ToString(),
                            IBHAND = (decimal)reader["IBHAND"],
                            ZeroInvDCOnHand = (decimal)reader["ZeroInvDCOnHand"],
                            ZeroInvInTransit = (decimal)reader["ZeroInvInTransit"],
                            StoreID = store
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

                var exemptions = data.Where(w => w.IsExemp == "Y" || ((w.HasCoContract != "Y" || w.HasCoContract == null) && w.O3TYPE == "CO")).ToList();

                var dataExemp = new List<ExemptionDto>();
                using (var connection = new SqlConnection(connStringCentralizedExemptions))
                using (var command = new SqlCommand("sp_GetExemptionsRaw", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = commandTimeoutInSeconds;
                    command.Parameters.AddWithValue("@O3LOC", store);
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
                                    " O3POS, O3SDT, O3EDT, O3RSDT, O3REDT, TypeId, SizeId, CategoryId, DPTNAM, O3FLAG1, INV2, IsExemp, NegativeSave, IBHAND, StoreID, HasCoContract, DateExemption) " +
                                     "VALUES (@IsPrinted, @O3SKU, @O3UPC, @O3IDSC, @o3type, @O3REG," +
                                    " @O3POS, @O3SDT, @O3EDT, @O3RSDT, @O3REDT, @TypeId, @SizeId, @CategoryId, @DPTNAM, @O3FLAG1, @INV2, @IsExemp, @NegativeSave, @IBHAND, @StoreID, @HasCoContract, @DateExemption)";

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
                            command.Parameters.AddWithValue("@HasCoContract", exemption.HasCoContract);
                            command.Parameters.AddWithValue("@DateExemption", DateTime.Now);

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

        public List<STRPRCLogDto> GetUpdatedData(string latestDate)
        {
            var records = new List<STRPRCLogDto>();
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand("sp_GetUpdatedData", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = commandTimeoutInSeconds;
                command.Parameters.AddWithValue("@Date", latestDate);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

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
                        TypeName = reader["TypeName"].ToString(),
                        SizeName = reader["SizeName"].ToString(),
                        CategoryName = reader["CategoryName"].ToString(),
                        O3TYPE = reader["O3TYPE"].ToString()
                    };

                    records.Add(record);
                }

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
                        orderby a.O3SDT descending
                        select new STRPRCDto
                        {
                            DateUpdated = a.DateUpdated,
                            LatestDate = a.O3DATE,
                            O3SDT = a.O3SDT
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

        public decimal UpdateSTRPRCTable(int storeId)
        {
            decimal date = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("sp_GetLatestSTRPRCTable", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = commandTimeoutInSeconds;
                        // Add any required parameters to the command if needed
                        command.Parameters.AddWithValue("@O3LOC", storeId);

                        connection.Open();

                        // Execute the command and retrieve the result count
                        date = (decimal)command.ExecuteScalar();

                        connection.Close();

                        // Use the result count as needed
                        //Console.WriteLine("Result Count: " + resultCount);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error executing stored procedure: " + ex.Message);
            }
            return date;
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
            var record = new ReportDto();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                try
                {
                    string query = "SELECT * FROM STRPRCs A " +
                        "LEFT JOIN Countries B ON A.O3TRB3 = B.iatrb3 " +
                        "LEFT JOIN DailyPromos C ON A.O3SKU = C.O1SKU " +
                        "WHERE A.O3SKU = @sku";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@sku", O3SKU);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                record = new ReportDto
                                {
                                    O3SKU = (decimal)reader["O3SKU"],
                                    O3CLAS = (decimal)reader["O3CLAS"],
                                    O3IDSC = reader["O3IDSC"].ToString(),
                                    O3SCCD = reader["O3SCCD"].ToString(),
                                    O3UPC = (decimal)reader["O3UPC"],
                                    O3TYPE = reader["O3TYPE"].ToString(),
                                    O3DEPT = (decimal)reader["O3DEPT"],
                                    O3SDPT = (decimal)reader["O3SDPT"],
                                    O3SCLS = (decimal)reader["O3SCLS"],
                                    O3POS = (decimal)reader["O3POS"],
                                    O3POSU = (decimal)reader["O3POSU"],
                                    O3REG = (decimal)reader["O3REG"],
                                    O3REGU = (decimal)reader["O3REGU"],
                                    O3SDT = (decimal)reader["O3SDT"],
                                    O3EDT = (decimal)reader["O3EDT"],
                                    O3TRB3 = reader["O3TRB3"].ToString(),
                                    O3FNAM = reader["O3FNAM"].ToString(),
                                    O3MODL = reader["O3MODL"].ToString(),
                                    O3LONG = reader["O3LONG"].ToString(),
                                    O3DIV = reader["O3DIV"].ToString(),
                                    O3TUOM = reader["O3TUOM"].ToString(),
                                    O3DATE = (decimal)reader["O3DATE"],
                                    TypeId = (int)reader["TypeId"],
                                    SizeId = (int)reader["SizeId"],
                                    CategoryId = (int)reader["CategoryId"],
                                    country_img = reader["country_img"] != DBNull.Value ? (byte[])reader["country_img"] : null,
                                    PromoVal = reader["O1VAL"] != DBNull.Value ? (decimal)reader["O1VAL"] : 0,
                                    PromoType = reader["O1PTYP"].ToString(),
                                };
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
            return record;
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

        public void AddMultipleInventoryPrintingLog(List<decimal> o3skus, string user, int sizeId, string printedOn)
        {
            foreach (var item in o3skus)
            {
                _db.InventoryPrintingLogs.Add(new InventoryPrintingLog()
                {
                    O3SKU = item,
                    PrintedBy = user,
                    DateCreated = DateTime.Now,
                    SizeId = sizeId,
                    PrintedOn = printedOn
                });
            }
            _db.SaveChanges();
        }

        public void AddMultipleQueuedPrintingLog(IEnumerable<ReportDto> data, string user, int sizeId)
        {
            foreach (var item in data)
            {
                _db.InventoryPrintingLogs.Add(new InventoryPrintingLog()
                {
                    O3SKU = item.O3SKU,
                    PrintedBy = user,
                    DateCreated = DateTime.Now,
                    RegularPrice = item.O3REGU,
                    CurrentPrice = item.O3POS,
                    Remarks = item.qRemarks,
                    ItemDesc = item.O3IDSC,
                    Brand = item.O3FNAM,
                    Model = item.O3MODL,
                    Divisor = item.O3DIV,
                    SizeId = sizeId,
                    TypeId = item.TypeId,
                    PrintedOn = "OnDemand - Queue"
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
                Divisor = model.O3DIV,
                SizeId = model.SizeId,
                TypeId = model.TypeId,
                PrintedOn = model.PrintedOn
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
                        IsExemp = reader["IsExemp"].ToString(),
                        ExemptionType = reader["ExemptionType"].ToString(),
                        IBHAND = (decimal)reader["IBHAND"]
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
            var storeId = int.Parse(ConfigurationManager.AppSettings["StoreID"]);
            try
            {
                using (SqlConnection connection = new SqlConnection(connStringCentralizedExemptions))
                {
                    using (SqlCommand command = new SqlCommand("sp_GetExemptionStatus", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = commandTimeoutInSeconds;
                        command.Parameters.AddWithValue("@StoreId", storeId);
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
            var storeId = int.Parse(ConfigurationManager.AppSettings["StoreID"]);
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
                            command.Parameters.AddWithValue("@StoreId", storeId);
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
                            command.Parameters.AddWithValue("@StoreId", storeId);
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
        public ReportDto GetPrintedLogPerSku(string sku)
        {
            var _sku = decimal.Parse(sku);
            var data = _db.InventoryPrintingLogs.Where(a => a.O3SKU == _sku).OrderByDescending(o => o.DateCreated).FirstOrDefault();

            if (data != null)
            {
                var log = new ReportDto();
                log.O3SKU = data.O3SKU;
                log.O3IDSC = data.ItemDesc;
                log.O3REGU = data.RegularPrice;
                log.O3POS = data.CurrentPrice;
                log.qRemarks = data.Remarks;
                log.O3FNAM = data.Brand;
                log.O3MODL = data.Model;
                log.O3DIV = data.Divisor;
                log.SizeId = data.SizeId == 0 ? 2 : data.SizeId;
                log.TypeId = data.TypeId;

                return log;
            }

            return null;
        }

        public bool Check151STRPRCChanges_LatestDate(int o3loc)
        {
            var dateToday = ConversionHelper.ToDecimal(DateTime.Now);
            using (var connection = new SqlConnection(connectionString151))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand($"SELECT TOP 1 O3DATE,O3SDT FROM [{o3loc}_STRPRC] order by O3SDT desc", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader.GetDecimal(0) == dateToday && reader.GetDecimal(1) == dateToday)
                            {
                                return true;
                            }
                            return false;
                        }
                        else
                            return false;
                    }
                }
            }
        }

        public bool Check151STRPRCNew_LatestDate(int o3loc)
        {
            var dateToday = ConversionHelper.ToDecimal(DateTime.Now);
            using (var connection = new SqlConnection(connectionString151))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand($"SELECT TOP 1 O3DATE,O3SDT FROM [{o3loc}_STRPRC_NEW] order by O3SDT desc", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader.GetDecimal(0) == dateToday && reader.GetDecimal(1) == dateToday)
                            {
                                return true;
                            }
                            return false;
                        }
                        else
                            return false;
                    }
                }
            }
        }

        public int SyncFromNew()
        {
            var store = int.Parse(ConfigurationManager.AppSettings["StoreID"]);
            var result = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("sp_SyncPCA", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = commandTimeoutInSeconds;
                        // Add any required parameters to the command if needed
                        command.Parameters.AddWithValue("@Store", store);

                        connection.Open();

                        result = (int)command.ExecuteScalar();

                        connection.Close();

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error executing stored procedure: " + ex.Message);
            }

            return result;
        }

        public int UpdateUPC()
        {
            var result = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("sp_UpdateUPC", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = commandTimeoutInSeconds;

                        connection.Open();

                        result = (int)command.ExecuteScalar();

                        connection.Close();

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error executing stored procedure: " + ex.Message);
            }

            return result;
        }

        public async Task<List<STRPRCDto>> GetAllConsignment(decimal startDate)
        {
            List<STRPRCDto> records = new List<STRPRCDto>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                try
                {
                    string query = @"SELECT [O3LOC]
                                  ,[O3SKU]
                                  ,[O3SCCD]
                                  ,[O3IDSC]
                                  ,[O3UPC]
                                  ,[O3VNUM]
                                  ,[O3TYPE]
                                  ,[O3DEPT]
                                  ,[O3SDPT]
                                  ,[O3CLAS]
                                  ,[O3SCLS]
                                  ,[O3SDSC]
                                  ,[O3POS]
                                  ,[O3POSU]
                                  ,[O3REG]
                                  ,[O3REGU]
                                  ,[O3ORIG]
                                  ,[O3ORGU]
                                  ,[O3EVT]
                                  ,[O3REVT]
                                  ,[O3PMMX]
                                  ,[O3PMTH]
                                  ,[O3PDQT]
                                  ,[O3PDPR]
                                  ,[O3SDT]
                                  ,[O3EDT]
                                  ,[O3RSDT]
                                  ,[O3REDT]
                                  ,[O3TRB3]
                                  ,[O3FGR]
                                  ,[O3FNAM]
                                  ,[O3MSRP]
                                  ,[O3MODL]
                                  ,[O3LONG]
                                  ,[O3SLUM]
                                  ,[O3DIV]
                                  ,[O3TUOM]
                                  ,[O3DATE]
                                  ,[O3CURD]
                                  ,[O3USER]
                                  ,[O3FLAG1]
                                  ,[O3FLAG2]
                                  ,[O3FLAG3]
                                  ,[DateUpdated]
                                  ,[TypeId]
                                  ,[SizeId]
                                  ,[CategoryId]
                                  ,[TypeName] = (case 
                            					when TypeId = 1 then 'Regular' 
                            					when TypeId = 2 then 'Save' 
                            					when TypeId = 3 then 'B1T1' 
                            					when TypeId = 4 then 'B1T1_M' 
                            					when TypeId = 5 then 'B1_A' 
                            					when TypeId = 6 then 'B1_P' 
                            				  end)
                            	  ,[SizeName] = (case when SizeId = 1 then 'Whole'
                            					   when SizeId = 2 then '1/8'
                            					   else 'Jewelry' 
                            				  end)
                            	  ,[CategoryName] = (case when CategoryId = 1 then 'Appliance' else 'Non-Appliance' end)
                                  ,[IsPrinted] = (case when IsPrinted = 1 then 'Yes' else 'No' end)
                            	  ,[DepartmentName] = (SELECT D.DPTNAM FROM INVDPT D
                            							WHERE D.IDEPT = O3DEPT
                            							)
                            	  ,[NegativeSave] = (case when O3REG < O3POS AND TypeId = 2 then 'Y' else 'N' end)
                            	  ,[IsExemp] = (case when (O3REG = O3POS AND O3EDT != 999999) OR (O3REG < O3POS AND TypeId = 2) OR (O3FLAG3 <> 'Y' OR O3FLAG3 IS NULL AND O3TYPE= 'CO') then 'Y' else 'N' end)

                              FROM [dbo].[STRPRCs]
                            WHERE O3TYPE = 'CO' 
                            AND O3SDT = @dateFilter 
                            AND Len(O3SKU) <> 4";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@dateFilter", startDate);

                        // Execute your command here
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                STRPRCDto record = new STRPRCDto
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
                                    DepartmentName = reader["DepartmentName"].ToString(),
                                    IsReverted = reader["O3FLAG1"].ToString(),
                                    IsExemp = reader["IsExemp"].ToString(),
                                    O3TYPE = reader["O3TYPE"].ToString(),
                                    SizeName = reader["SizeName"].ToString(),
                                    TypeName = reader["TypeName"].ToString(),
                                    CategoryName = reader["CategoryName"].ToString()
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

        public List<ExportPCAExemptionDto> GetAllNoConsignmentContract()
        {
            var result = _db.Database.SqlQuery<ExportPCAExemptionDto>("EXEC sp_GetAllConsignmentToExport")
               .Where(a => a.O3FLAG3 != "Y" || a.O3FLAG3 == null)
               .ToList();

            return result;
        }

        public List<ExportPCADto> GetConsignmentToExport(decimal[] selectedSkus)
        {
            #region -- TABLE VALUED PARAMETER
            var table = new DataTable();
            table.Columns.Add("Value", typeof(decimal));

            foreach (var sku in selectedSkus)
            {
                table.Rows.Add(sku);
            }

            var parameter = new SqlParameter
            {
                ParameterName = "@SelectedSkus",
                SqlDbType = SqlDbType.Structured,
                TypeName = "dbo.DecimalArray",
                Value = table
            };

            #endregion

            var result = _db.Database.SqlQuery<ExportPCADto>("EXEC sp_GetAllConsignmentToExport @SelectedSkus", parameter)
                .Where(a => a.O3FLAG3 == "Y")
               .ToList();

            return result;
        }

        public async Task<List<STRPRCDto>> GetDataByPCAHistory(string dateFilter)
        {
            List<STRPRCDto> records = new List<STRPRCDto>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                try
                {
                    string query = @"SELECT
                        s.*,
                        [IsExemp] = (case when (s.O3REG = s.O3POS AND s.O3EDT != 999999) OR (s.O3REG < s.O3POS AND s.TypeId = 2) OR (s.O3FLAG3 !='Y' AND s.O3TYPE= 'CO') then 'Y' else 'N' end),
                        [NegativeSave] = (case when s.O3REG < s.O3POS AND s.TypeId = 2 then 'Y' else 'N' end),
                        [ZeroInvDCOnHand] = (case when zib.sum_dconhand IS NULL then 0 else zib.sum_dconhand end),
                        [ZeroInvInTransit] = (case when zit.intransit IS NULL then 0 else zit.intransit end),
                        [IsAudited] = (case when aud.IsAudited IS NULL then 'N' else 'Y' end),
                        [IsNotRequired] = (case when aud.IsNotRequired IS NULL then 'N' else 'Y' end),                           
                        [TypeName] = (case when s.TypeId = 1 then 'Regular' when s.TypeId = 2 then 'Save' when s.TypeId = 3 then 'B1T1' when s.TypeId = 4 then 'B1T1_M' when s.TypeId = 5 then 'B1_A' when s.TypeId = 6 then 'B1_P' end),
                        [SizeName] = (case when s.SizeId = 1 then 'Whole' when s.SizeId = 2 then '1/8' else 'Jewelry' end),
                        [CategoryName] = (case when s.CategoryId = 1 then 'Appliance' else 'Non-Appliance' end), 
                        s.IBHAND,
                        [IsPrintedYN] = (case when s.IsPrinted = 1 then 'Yes' else 'No' end)
                        FROM PCAHistory s 
                        LEFT JOIN INVBAL ib ON ib.INUMBR = s.O3SKU 
                        LEFT JOIN ZEROINV_DCONHAND zib ON s.O3SKU = zib.INUMBR 
                        LEFT JOIN ZEROINV_INTRANSIT zit ON s.O3SKU = zit.INUMBR
                        LEFT JOIN Audit_PrintedHistory aud ON s.O3SKU = aud.Sku AND s.PCADate = aud.DatePrinted WHERE s.PCADate = @dateFilter";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@dateFilter", dateFilter);

                        // Execute your command here
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                STRPRCDto record = new STRPRCDto
                                {
                                    IsPrinted = reader["IsPrintedYN"].ToString(),
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
                                    HasInventory = reader["INV"].ToString(),
                                    IsExemp = reader["IsExemp"].ToString(),
                                    O3TYPE = reader["O3TYPE"].ToString(),
                                    IBHAND = (decimal)reader["IBHAND"],
                                    ZeroInvDCOnHand = (decimal)reader["ZeroInvDCOnHand"],
                                    ZeroInvInTransit = (decimal)reader["ZeroInvInTransit"],
                                    IsNotRequired = reader["IsNotRequired"].ToString(),
                                    SizeName = reader["SizeName"].ToString(),
                                    TypeName = reader["TypeName"].ToString(),
                                    CategoryName = reader["CategoryName"].ToString()
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

        public async Task<List<STRPRCDto>> GetDataByConsignmentHistory(string dateFilter)
        {
            List<STRPRCDto> records = new List<STRPRCDto>();
            DateTime date = DateTime.Parse(dateFilter);
            decimal dateFilterInDecimal = date.Year % 100 * 10000 + date.Month * 100 + date.Day;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                try
                {
                    string query = "SELECT" +
                        "*" +
                        ",[TypeName] = (case when TypeId = 1 then 'Regular' when TypeId = 2 then 'Save' when TypeId = 3 then 'B1T1' when TypeId = 4 then 'B1T1_M' when TypeId = 5 then 'B1_A' when TypeId = 6 then 'B1_P'end)," +
                        "[SizeName] = (case when SizeId = 1 then 'Whole' when SizeId = 2 then '1/8' else 'Jewelry' end)," +
                        "[CategoryName] = (case when CategoryId = 1 then 'Appliance' else 'Non-Appliance' end)," +
                        "[IsPrinted] = (case when IsPrinted = 1 then 'Yes' else 'No' end)," +
                        "[DepartmentName] = (SELECT D.DPTNAM FROM INVDPT D WHERE D.IDEPT = O3DEPT)," +
                        "[NegativeSave] = (case when O3REG < O3POS AND TypeId = 2 then 'Y' else 'N' end)," +
                        "[IsExemp] = (case when (O3REG = O3POS AND O3EDT != 999999) OR (O3REG < O3POS AND TypeId = 2) OR (O3FLAG3 <> 'Y' OR O3FLAG3 IS NULL AND O3TYPE= 'CO') then 'Y' else 'N' end)" +
                        "FROM ConsignmentHistory WHERE HistoryDate = @dateFilter AND O3SDT = @dateFilterInDecimal";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@dateFilter", dateFilter);
                        cmd.Parameters.AddWithValue("@dateFilterInDecimal", dateFilterInDecimal);

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                STRPRCDto record = new STRPRCDto
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
                                    IsExemp = reader["IsExemp"].ToString(),
                                    O3TYPE = reader["O3TYPE"].ToString(),
                                    SizeName = reader["SizeName"].ToString(),
                                    TypeName = reader["TypeName"].ToString(),
                                    CategoryName = reader["CategoryName"].ToString(),
                                    O3FLAG3 = reader["O3FLAG3"].ToString()
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

        public PromoEngineDto CheckIfSkuHasPromo(decimal sku)
        {
            var record = new PromoEngineDto();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                try
                {
                    string query = "SELECT " +
                        "*," +
                        "[TypeId] = (CASE WHEN O1PTYP = 'B1T1' THEN 3 " +
                        "WHEN O1PTYP = 'B1T1M' THEN 4 " +
                        "WHEN O1PTYP = 'B1_A' THEN 5 " +
                        "WHEN O1PTYP = 'B1_P' THEN 6 END) " +
                        "FROM DailyPromos WHERE O1SKU = @sku";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@sku", sku);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                record = new PromoEngineDto
                                {
                                    Sku = (decimal)reader["O1SKU"],
                                    StartDate = (decimal)reader["O1SDT"],
                                    EndDate = (decimal)reader["O1EDT"],
                                    PromoType = reader["O1PTYP"].ToString(),
                                    PromoVal = (decimal)reader["O1VAL"],
                                    TypeId = (int)reader["TypeId"]
                                };
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
            return record;
        }
    }
}