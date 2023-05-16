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

        public STRPRCRepository(ApplicationDbContext db)
        {
            _db = db;
            connectionString = ConfigurationManager.ConnectionStrings["PriceSignageDbConnectionString"].ConnectionString;
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

        public IEnumerable<STRPRC> FilterByDate(decimal fromDate/*, decimal toDate*/)
        {
            var data = (from a in _db.STRPRCs
                        where a.O3SDT >= fromDate// && a.O3EDT <= toDate
                        select a).ToList();

            return data;
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
            STRPRCDto data = null;
            // Set up the connection and command
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand("sp_SearchProduct", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                // Add parameters if required
                command.Parameters.AddWithValue("@O3SKU", query);
                command.Parameters.AddWithValue("@O3UPC", query);

                // Open the connection and execute the command
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                // Process the result set
                if (reader.Read())
                {
                    data = new STRPRCDto();
                    data.O3LOC = (decimal)reader["O3LOC"];
                    data.O3CLAS = (decimal)reader["O3CLAS"];
                    data.O3IDSC = reader["O3IDSC"].ToString();
                    data.O3SKU = (decimal)reader["O3SKU"];
                    data.O3SCCD = reader["O3SCCD"].ToString();
                    data.O3UPC = (decimal)reader["O3UPC"];
                    data.O3VNUM = (decimal)reader["O3VNUM"];
                    data.O3TYPE = reader["O3TYPE"].ToString();
                    data.O3DEPT = (decimal)reader["O3DEPT"];
                    data.O3SDPT = (decimal)reader["O3SDPT"];
                    data.O3SCLS = (decimal)reader["O3SCLS"];
                    data.O3POS = (decimal)reader["O3POS"];
                    data.O3POSU = (decimal)reader["O3POSU"];
                    data.O3REG = (decimal)reader["O3REG"];
                    data.O3REGU = (decimal)reader["O3REGU"];
                    data.O3ORIG = (decimal)reader["O3ORIG"];
                    data.O3ORGU = (decimal)reader["O3ORGU"];
                    data.O3EVT = (decimal)reader["O3EVT"];
                    data.O3PMMX = (decimal)reader["O3PMMX"];
                    data.O3PMTH = (decimal)reader["O3PMTH"];
                    data.O3PDQT = (decimal)reader["O3PDQT"];
                    data.O3PDPR = (decimal)reader["O3PDPR"];
                    data.O3SDT = (decimal)reader["O3SDT"];
                    data.O3EDT = (decimal)reader["O3EDT"];
                    data.O3TRB3 = reader["O3TRB3"].ToString();
                    data.O3FGR = (decimal)reader["O3FGR"];
                    data.O3FNAM = reader["O3FNAM"].ToString();
                    data.O3SLUM = reader["O3SLUM"].ToString();
                    data.O3DIV = reader["O3DIV"].ToString();
                    data.O3TUOM = reader["O3TUOM"].ToString();
                    data.O3DATE = (decimal)reader["O3DATE"];
                    data.O3CURD = (decimal)reader["O3CURD"];
                    data.O3USER = reader["O3USER"].ToString();
                    data.DateUpdated = (DateTime)reader["DateUpdated"];
                }

                // Close the reader and connection
                reader.Close();
                connection.Close();
            }

            return data;
        }

        public List<STRPRC> GetData(decimal O3SKU)
        {
            var data = new List<STRPRC>();

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

        public IEnumerable<STRPRC> GetDataByDate(decimal startDate, decimal endDate)
        {
            var data = _db.STRPRCs.Where(a => a.O3SDT >= startDate && a.O3EDT <= endDate);

            return data;
        }
    }
}