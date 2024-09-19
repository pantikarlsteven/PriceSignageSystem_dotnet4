using PriceSignageSystem.Models.Constants;
using PriceSignageSystem.Models.DatabaseContext;
using PriceSignageSystem.Models.Dto;
using PriceSignageSystem.Models.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace PriceSignageSystem.Models.Repository
{
    public class QueueRepository : IQueueRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly string connectionString;

        public QueueRepository(ApplicationDbContext db)
        {
            _db = db;
            connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
        }
        public ItemQueue AddItemQueue(STRPRCDto model)
        {
            var record = new ItemQueue();
            record.O3SKU = model.O3SKU;
            record.TypeId = model.TypeId;
            record.SizeId = model.SizeId;
            record.UserName = HttpContext.Current.User.Identity.Name;
            record.Status = ReportConstants.Status.InQueue;
            record.ItemDesc = model.O3IDSC;
            record.Brand = model.O3FNAM;
            record.Model = model.O3MODL;
            record.Divisor = model.O3DIV;
            record.Remarks = model.Remarks;
            record.DateCreated = DateTime.Now;
            record.RegularPrice = model.O3REGU;
            record.CurrentPrice = model.O3POS;
            record.Tuom = model.O3TUOM;
            record.ExpDateCER = model.ExpDateCER;
            record.IsEdited = model.IsEdited;
            
            var data =  _db.ItemQueues.Add(record);
            _db.SaveChanges();

            return data;

        }
        public List<ReportDto> GetQueueListPerUser(string username)
        {
            var status = ReportConstants.Status.InQueue;
            var records = new  List<ReportDto>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                try
                {
                    string query = "SELECT b.*, c.country_img," +
                        "a.Id, " +
                        "a.UserName, " +
                        "a.TypeId as QueueTypeId, " +
                        "a.SizeId as QueueSizeId, " +
                        "a.Status, " +
                        "a.CurrentPrice, " +
                        "a.RegularPrice, " +
                        "a.Brand, " +
                        "a.Model, " +
                        "a.Divisor, " +
                        "a.ItemDesc, " +
                        "a.Remarks, " +
                        "a.Tuom, " +
                        "a.IsEdited, " +
                        "a.ExpDateCER, " +
                        "d.O1VAL, " +
                        "d.O1PTYP " +
                        "FROM ItemQueues a " +
                        "LEFT JOIN STRPRCs b ON a.O3SKU = b.O3SKU " +
                        "LEFT JOIN Countries c ON b.O3TRB3 = c.iatrb3 " +
                        "LEFT JOIN DailyPromos d ON a.O3SKU = d.O1SKU " +
                        "WHERE a.UserName = @username AND a.Status = @status ";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@status", status);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var record = new ReportDto
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
                                    O3SDT = (decimal)reader["O3SDT"],
                                    O3EDT = (decimal)reader["O3EDT"],
                                    O3TRB3 = reader["O3TRB3"].ToString(),
                                    O3FNAM = reader["O3FNAM"].ToString(),
                                    O3MODL = reader["O3MODL"].ToString(),
                                    O3LONG = reader["O3LONG"].ToString(),
                                    O3SLUM = reader["O3SLUM"].ToString(),
                                    O3DIV = reader["O3DIV"].ToString(),
                                    O3TUOM = reader["O3TUOM"].ToString(),
                                    O3DATE = (decimal)reader["O3DATE"],
                                    O3CURD = (decimal)reader["O3CURD"],
                                    O3USER = reader["O3USER"].ToString(),
                                    UserName = reader["UserName"].ToString(),
                                    TypeId = (int)reader["QueueTypeId"],
                                    SizeId = (int)reader["QueueSizeId"],
                                    Status = reader["Status"].ToString(),
                                    country_img = reader["country_img"] != DBNull.Value ? (byte[])reader["country_img"] : null,
                                    ItemQueueId = (int)reader["Id"],
                                    qCurrentPrice = (decimal)reader["CurrentPrice"],
                                    qRegularPrice = (decimal)reader["RegularPrice"],
                                    qBrand = reader["Brand"].ToString(),
                                    qModel = reader["Model"].ToString(),
                                    qDivisor = reader["Divisor"].ToString(),
                                    qItemDesc = reader["ItemDesc"].ToString(),
                                    qRemarks = reader["Remarks"].ToString(),
                                    qTypeId = (int)reader["QueueTypeId"],
                                    CategoryId = (int)reader["CategoryId"],
                                    qTuom = reader["Tuom"].ToString(),
                                    IsEdited = reader["IsEdited"].ToString(),
                                    ExpDateCER = reader["ExpDateCER"].ToString(),
                                    PromoVal = reader["O1VAL"] != DBNull.Value ? (decimal)reader["O1VAL"] : 0,
                                    PromoType = reader["O1PTYP"].ToString()
                                };
                                records.Add(record);
                            }
                            reader.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                }

                conn.Close();
            }

            return records;
            
        }

        public void UpdateStatus(IEnumerable<ReportDto> data)
        {
            foreach(var item in data)
            {
                var model = _db.ItemQueues.Where(a => a.Id == item.ItemQueueId).FirstOrDefault();
                model.Status = ReportConstants.Status.Printed;
                model.DateUpdated = DateTime.Now;
            }
            _db.SaveChanges();
        }

        public void QueueMultipleItems(int sizeId, decimal[] skus)
        {
            var toQueueList = new List<STRPRC>();
            var promoList = new List<PromoEngineDto>();
            string skuString;

            if (skus.Length > 1)
                skuString = string.Join(",", skus);
            else
                skuString = skus.First().ToString();

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
                        $"FROM DailyPromos WHERE O1SKU in ({skuString})";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var record = new PromoEngineDto
                                {
                                    Sku = (decimal)reader["O1SKU"],
                                    StartDate = (decimal)reader["O1SDT"],
                                    EndDate = (decimal)reader["O1EDT"],
                                    PromoType = reader["O1PTYP"].ToString(),
                                    PromoVal = (decimal)reader["O1VAL"],
                                    TypeId = (int)reader["TypeId"]
                                };
                                promoList.Add(record);
                            }
                            reader.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                }

                conn.Close();
            }

            toQueueList = _db.STRPRCs.Where(a => skus.Contains(a.O3SKU)).ToList();

            foreach (var item in toQueueList)
            {
                var skuPromo = promoList.Where(a => a.Sku == item.O3SKU).FirstOrDefault();

                var model = new ItemQueue();
                model.O3SKU = item.O3SKU;
                model.SizeId = sizeId;
                model.UserName = HttpContext.Current.User.Identity.Name;
                model.Status = ReportConstants.Status.InQueue;
                model.DateCreated = DateTime.Now;
                model.IsEdited = "N";

                if (skuPromo != null)
                {
                    if (skuPromo.StartDate > item.O3SDT)
                        model.TypeId = skuPromo.TypeId;
                }
                else
                {
                    model.TypeId = item.TypeId;
                }

                _db.ItemQueues.Add(model);
                _db.SaveChanges();
            }
        }

        public List<ItemQueueDto> GetHistory(string username)
        {
            var dateToday = DateTime.Today;
            var result = (from a in _db.ItemQueues
                          join b in _db.Sizes on a.SizeId equals b.Id
                          join c in _db.Types on a.TypeId equals c.Id
                          where a.UserName == username && DbFunctions.TruncateTime(a.DateCreated) == dateToday
                          orderby a.DateCreated descending
                          select new ItemQueueDto
                          {
                              Id = a.Id,
                              O3SKU = a.O3SKU,
                              TypeName = c.Name,
                              SizeName = b.Name,
                              Status = a.Status,
                              DateCreated = a.DateCreated,
                              DateUpdated = a.DateUpdated,
                              Remarks = a.Remarks
                          }).ToList();

            return result;
        }
        public int RequeueItem(int id, string username)
        {

            var data = _db.ItemQueues.Where(a => a.Id == id && a.UserName == username).FirstOrDefault();

            if (data != null)
            {
                data.Status = ReportConstants.Status.InQueue;
                data.DateUpdated = DateTime.Now;
            }
           
            var count = _db.SaveChanges();

            return count;
        }

        public Array GetInQueueSizePerUser(string username)
        {
            var result = (from a in _db.ItemQueues
                             join b in _db.Sizes on a.SizeId equals b.Id
                             where a.UserName == username && a.Status == ReportConstants.Status.InQueue
                             select new { Id = b.Id, Name = b.Name }).Distinct();



            return result.ToArray();
        }
    }
}