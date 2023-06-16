using PriceSignageSystem.Models.Constants;
using PriceSignageSystem.Models.DatabaseContext;
using PriceSignageSystem.Models.Dto;
using PriceSignageSystem.Models.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PriceSignageSystem.Models.Repository
{
    public class QueueRepository : IQueueRepository
    {
        private readonly ApplicationDbContext _db;

        public QueueRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public ItemQueue AddItemQueue(STRPRCDto model)
        {
            var record = new ItemQueue();
            record.O3SKU = model.O3SKU;
            record.SizeId = model.SelectedSizeId;
            record.TypeId = model.SelectedTypeId;
            record.CategoryId = model.SelectedCategoryId;
            record.UserName = HttpContext.Current.User.Identity.Name;
            record.Status = ReportConstants.Status.InQueue;
            record.DateCreated = DateTime.Now;

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
                            SizeId = a.SizeId,
                            TypeId = a.TypeId,
                            CategoryId = a.CategoryId,
                            Status = a.Status,
                            iatrb3 = d.iatrb3,
                            country_img = d.country_img,
                            ItemQueueId = a.Id
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
    }
}