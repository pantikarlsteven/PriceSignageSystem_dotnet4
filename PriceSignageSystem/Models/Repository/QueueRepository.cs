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
            var data = (from a in _db.ItemQueues
                        join b in _db.STRPRCs on a.O3SKU equals b.O3SKU
                        join c in _db.Countries on b.O3TRB3 equals c.iatrb3 into bc
                        from d in bc.DefaultIfEmpty()
                        where a.UserName == username && a.Status == ReportConstants.Status.InQueue
                        select new ReportDto
                        {
                            O3LOC = b.O3LOC,
                            O3CLAS = b.O3CLAS,
                            O3IDSC = b.O3IDSC,
                            O3SKU = b.O3SKU,
                            O3SCCD = b.O3SCCD,
                            O3UPC = b.O3UPC,
                            O3VNUM = b.O3VNUM,
                            O3TYPE = b.O3TYPE,
                            O3DEPT = b.O3DEPT,
                            O3SDPT = b.O3SDPT,
                            O3SCLS = b.O3SCLS,
                            O3POS = b.O3POS,
                            O3POSU = b.O3POSU,
                            O3REG = b.O3REG,
                            O3REGU = b.O3REGU,
                            O3ORIG = b.O3ORIG,
                            O3ORGU = b.O3ORGU,
                            O3EVT = b.O3EVT,
                            O3PMMX = b.O3PMMX,
                            O3PMTH = b.O3PMTH,
                            O3PDQT = b.O3PDQT,
                            O3PDPR = b.O3PDPR,
                            O3SDT = b.O3SDT,
                            O3EDT = b.O3EDT,
                            O3TRB3 = b.O3TRB3,
                            O3FGR = b.O3FGR,
                            O3FNAM = b.O3FNAM,
                            O3MODL = b.O3MODL,
                            O3LONG = b.O3LONG,
                            O3SLUM = b.O3SLUM,
                            O3DIV = b.O3DIV,
                            O3TUOM = b.O3TUOM,
                            O3DATE = b.O3DATE,
                            O3CURD = b.O3CURD,
                            O3USER = b.O3USER,
                            DateUpdated = b.DateUpdated,
                            UserName = a.UserName,
                            TypeId = a.TypeId,
                            SizeId = a.SizeId,
                            Status = a.Status,
                            iatrb3 = d.iatrb3,
                            country_img = d.country_img,
                            ItemQueueId = a.Id,
                            qCurrentPrice = a.CurrentPrice,
                            qRegularPrice = a.RegularPrice,
                            qBrand = a.Brand,
                            qModel = a.Model,
                            qDivisor = a.Divisor,
                            qItemDesc = a.ItemDesc,
                            qRemarks = a.Remarks,
                            qTypeId = a.TypeId,
                            CategoryId = b.CategoryId,
                            qTuom = a.Tuom,
                            IsEdited = a.IsEdited,
                            ExpDateCER = a.ExpDateCER
                        }).ToList();
            return data;
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