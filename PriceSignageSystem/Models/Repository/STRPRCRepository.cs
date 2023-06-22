using PriceSignageSystem.Models.DatabaseContext;
using PriceSignageSystem.Models.Dto;
using PriceSignageSystem.Models.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace PriceSignageSystem.Models.Repository
{
    public class STRPRCRepository : ISTRPRCRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly string connectionString;
        private readonly int commandTimeoutInSeconds;

        public STRPRCRepository(ApplicationDbContext db)
        {
            _db = db;
            connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
            commandTimeoutInSeconds = 180;

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

        public STRPRCDto SearchString(string query)
        {

            var data = (from a in _db.STRPRCs
                        where a.O3SKU.ToString() == query || a.O3UPC.ToString() == query
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
                            SelectedTypeId = a.O3REGU == a.O3POS ? 1 : 2,
                            SelectedCategoryId = a.O3DEPT == 150 && (a.O3SDPT == 10 || a.O3SDPT == 12 || a.O3SDPT == 13 || a.O3SDPT == 14) ? 1 : 2
                        }).FirstOrDefault();
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

        public List<STRPRCDto> GetDataByStartDate(decimal startDate, bool withInventory)
        {
            var sp = "sp_GettmpData";
            var HasInv = withInventory == true ? 'Y' : 'N';
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
                command.Parameters.AddWithValue("@HasInv", HasInv);

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
                        TypeId = (int)reader["TypeId"],
                        SizeId = (int)reader["SizeId"],
                        CategoryId = (int)reader["CategoryId"],
                        DepartmentName = reader["DPTNAM"].ToString(),
                        IsReverted = reader["O3FLAG1"].ToString(),
                        IsPrinted = reader["IsPrinted"].ToString()
                    };

                    data.Add(record);
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
                        IsPrinted = (bool)reader["IsPrinted"] ? "Printed" : "",
                        O3IDSC = reader["O3IDSC"].ToString(),
                        ColumnName = reader["ColumnName"].ToString(),
                        DateUpdated = (DateTime)reader["DateUpdated"],
                        O3FNAM = reader["O3FNAM"].ToString(),
                        O3DEPT = (decimal)reader["O3DEPT"],
                        O3CLAS = (decimal)reader["O3CLAS"],
                        O3SCCD = reader["O3SCCD"].ToString(),
                        O3LONG = reader["O3LONG"].ToString(),
                        O3MODL = reader["O3MODL"].ToString(),
                        O3SDPT = (decimal)reader["O3SDPT"],
                        O3SCLS = (decimal)reader["O3SCLS"],
                        O3TRB3 = reader["O3TRB3"].ToString(),

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

            var data = (from a in _db.STRPRCLogs
                        join b in _db.STRPRCs on a.O3SKU equals b.O3SKU into ab
                        from c in ab.DefaultIfEmpty()
                        select new STRPRCLogDto
                        {
                            
                        }).Take(10).ToList();

            return data;
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

        public DateTime GetLatestUpdate()
        {
            var date = (from a in _db.STRPRCs
                        orderby a.DateUpdated descending
                        select a.DateUpdated).FirstOrDefault();

            return date;
        }

        public int UpdateSTRPRCTable(int storeId)
        {
            var count = 0;
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
                    count = (int)command.ExecuteScalar();

                    connection.Close();

                    // Use the result count as needed
                    //Console.WriteLine("Result Count: " + resultCount);
                }
            }
            return count;
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
    }
}