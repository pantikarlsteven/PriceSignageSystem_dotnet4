using PriceSignageSystem.Code.CustomValidations;
using PriceSignageSystem.Models;
using PriceSignageSystem.Models.Dto;
using PriceSignageSystem.Models.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PriceSignageSystem.Controllers
{
    [CustomAuthorize]
    public class AuditController : Controller
    {
        public readonly IAuditRepository _auditRepo;
        // GET: Audit
        public AuditController(IAuditRepository auditRepo)
        {
            _auditRepo = auditRepo;
        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> LoadAudit()
        {
            var latestDate = _auditRepo.GetLatestDate();
            var rawData = await _auditRepo.GetPCAbyLatestDate(latestDate);
            var auditList = new AuditDto();

            auditList.PrintedList = rawData.Where(a => a.IsPrinted == "True" && a.IsAudited == "N").ToList();
            auditList.NotPrintedList = rawData.Where(a => a.IsPrinted == "False" && a.IsAudited == "N").ToList();
            auditList.AuditedList = rawData.Where(a => a.IsAudited == "Y").ToList();
            
            foreach (var item in auditList.PrintedList)
            {
                item.TypeName = item.TypeId == 2 ? "Save"
                                : item.TypeId == 1 ? "Regular"
                                : "Save";
                item.SizeName = item.SizeId == 1 ? "Whole"
                                : item.SizeId == 2 ? "1/8"
                                : item.SizeId == 3 ? "Jewelry"
                                : "Whole";
                item.CategoryName = item.CategoryId == 1 ? "Appliance"
                                    : item.CategoryId == 2 ? "Non-Appliance"
                                    : "Non-Appliance";
                item.IsPrinted = item.IsPrinted == "True" ? "Yes" : "No";
                item.IsReverted = item.IsReverted == "Y" ? "Yes" : "No";
                item.IsExemp = item.IsExemp == "Y" ? "Yes" : "No";
                item.IsWrongPrice = item.IsWrongPrice == "Y" ? "Yes" : "No";

            }

            foreach (var item in auditList.NotPrintedList)
            {
                item.TypeName = item.TypeId == 2 ? "Save"
                                : item.TypeId == 1 ? "Regular"
                                : "Save";
                item.SizeName = item.SizeId == 1 ? "Whole"
                                : item.SizeId == 2 ? "1/8"
                                : item.SizeId == 3 ? "Jewelry"
                                : "Whole";
                item.CategoryName = item.CategoryId == 1 ? "Appliance"
                                    : item.CategoryId == 2 ? "Non-Appliance"
                                    : "Non-Appliance";
                item.IsPrinted = item.IsPrinted == "True" ? "Yes" : "No";
                item.IsReverted = item.IsReverted == "Y" ? "Yes" : "No";
                item.IsExemp = item.IsExemp == "Y" ? "Yes" : "No";
            }

            foreach (var item in auditList.AuditedList)
            {
                item.TypeName = item.TypeId == 2 ? "Save"
                                : item.TypeId == 1 ? "Regular"
                                : "Save";
                item.SizeName = item.SizeId == 1 ? "Whole"
                                : item.SizeId == 2 ? "1/8"
                                : item.SizeId == 3 ? "Jewelry"
                                : "Whole";
                item.CategoryName = item.CategoryId == 1 ? "Appliance"
                                    : item.CategoryId == 2 ? "Non-Appliance"
                                    : "Non-Appliance";
                item.IsPrinted = item.IsPrinted == "True" ? "Yes" : "No";
                item.IsReverted = item.IsReverted == "Y" ? "Yes" : "No";
                item.IsExemp = item.IsExemp == "Y" ? "Yes" : "No";
            }

            string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(auditList);

            // Compress the JSON data using Gzip compression
            byte[] compressedData;
            using (MemoryStream outputStream = new MemoryStream())
            {
                using (GZipStream gzipStream = new GZipStream(outputStream, CompressionMode.Compress))
                {
                    using (StreamWriter writer = new StreamWriter(gzipStream))
                    {
                        writer.Write(jsonData);
                    }
                }
                compressedData = outputStream.ToArray();
            }

            Response.AppendHeader("Content-Encoding", "gzip");
            Response.AppendHeader("Content-Length", compressedData.Length.ToString());

            return File(compressedData, "application/json");
        }

        [HttpPost]
        public ActionResult ScanBarcode(string code, string codeFormat)
        {
            var data = _auditRepo.ScanBarcode(code, codeFormat);
            return Json(data);
        }

        [HttpPost]
        public ActionResult Post(string sku)
        {
            var username = User.Identity.Name;
            var isSuccess = _auditRepo.Post(sku, username);

            return Json(isSuccess);
        }

        [HttpPost]
        public ActionResult NotRequireTagging(string sku, string isChecked)
        {
            var username = User.Identity.Name;
            var result = _auditRepo.NotRequireTagging(sku, isChecked, username);

            return Json(result);
        }

        [HttpPost]
        public ActionResult PostWithRemarks(string sku, string remarks)
        {
            var username = User.Identity.Name;
            var isSuccess = _auditRepo.PostWithRemarks(sku, username, remarks);

            return Json(isSuccess);
        }

        [HttpPost]
        public ActionResult GetAllRemarks()
        {
            //temporary 
            var list = new List<AuditRemark>();
            list.Add(new AuditRemark { Id = 1, Name = "NOF" });
            list.Add(new AuditRemark { Id = 2, Name = "Disposal" });
            list.Add(new AuditRemark { Id = 3, Name = "Mark Down" });

            return Json(list.ToArray());
        }

        [HttpPost]
        public ActionResult TagWrongPrice(string sku)
        {
            var username = User.Identity.Name;
            var result = _auditRepo.TagWrongPrice(sku, username);

            return Json(result);
        }
    }
}