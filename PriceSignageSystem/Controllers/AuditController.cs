using ClosedXML.Excel;
using PriceSignageSystem.Code.CustomValidations;
using PriceSignageSystem.Helper;
using PriceSignageSystem.Models;
using PriceSignageSystem.Models.Dto;
using PriceSignageSystem.Models.Interface;
using System;
using System.Collections.Generic;
using System.Data;
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
        private readonly ISTRPRCRepository _sTRPRCRepository;

        // GET: Audit
        public AuditController(IAuditRepository auditRepo, ISTRPRCRepository sTRPRCRepository)
        {
            _auditRepo = auditRepo;
            _sTRPRCRepository = sTRPRCRepository;
        }
        public ActionResult Index(string date)
        {
            ViewBag.DateFilter = date;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> LoadAudit(string dateFilter)
        {
            var printedList = new List<AuditDto>();
            var unprintedList = new List<AuditDto>();
            var result = _sTRPRCRepository.GetLatestUpdate();
           
            if (!string.IsNullOrEmpty(dateFilter) && dateFilter != result.DateUpdated.ToString("yyyy-MM-dd"))
            {
                printedList = await _auditRepo.GetAllPrintedByHistory(dateFilter);
                unprintedList = await _auditRepo.GetAllUnPrintedByHistory(dateFilter);
            }
            else
            {
                printedList = await _auditRepo.GetAllPrinted();
                unprintedList = await _auditRepo.GetAllUnprinted(result.LatestDate);
            }
           
            var auditList = new AuditDto();
            var dateToday = ConversionHelper.ToDecimal(DateTime.Now);

            auditList.PrintedList = printedList.Where(a => a.IsPrinted == "Yes" && a.IsAudited == "No").ToList();
            auditList.NotPrintedList = unprintedList.OrderByDescending(o => o.IsNotRequired).ToList();
            auditList.AuditedList = printedList.Where(a => a.IsAudited == "Yes").ToList();
            
            foreach(var item in auditList.NotPrintedList)
            {
                if (item.IsReverted == "Yes" && item.O3EDT == 999999)
                    item.O3SDT = dateToday;
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
        public ActionResult ScanBarcode(string code, string codeFormat, bool isScaleTicket)
        {
            var data = _auditRepo.ScanBarcode(code, codeFormat, isScaleTicket);
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
            list.Add(new AuditRemark { Id = 2, Name = "Damaged" });
            list.Add(new AuditRemark { Id = 3, Name = "Marked Down" });
            list.Add(new AuditRemark { Id = 4, Name = "Expired" });

            return Json(list.ToArray());
        }

        [HttpPost]
        public ActionResult TagWrongSign(string sku)
        {
            var username = User.Identity.Name;
            var result = _auditRepo.TagWrongSign(sku, username);

            return Json(result);
        }

        [HttpGet]
        public FileResult ExportDataTableToExcel(string filter)
        {
            var dataTable = new DataTable();
            var auditedList = _auditRepo.GetAllAuditToExport().Where(a => a.IsAudited == "Y");

            if (filter == "all")
                auditedList = auditedList.ToList();
            else if (filter == "nof")
                auditedList = auditedList.Where(a => a.Remarks == "NOF").ToList();
            else if (filter == "damaged")
                auditedList = auditedList.Where(a => a.Remarks == "Damaged").ToList();
            else if (filter == "markeddown")
                auditedList = auditedList.Where(a => a.Remarks == "Marked Down").ToList();
            else if (filter == "expired")
                auditedList = auditedList.Where(a => a.Remarks == "Expired").ToList();

            dataTable = ConversionHelper.ConvertListToDataTable(auditedList);

            using (XLWorkbook workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");

                // Set the column headers
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    var cell = worksheet.Cell(1, i + 1);
                    cell.Value = dataTable.Columns[i].ColumnName;

                    // Set the cell style to bold
                    var style = cell.Style;
                    style.Font.Bold = true;
                }

                // Populate the data rows
                for (int row = 0; row < dataTable.Rows.Count; row++)
                {
                    for (int col = 0; col < dataTable.Columns.Count; col++)
                    {
                        worksheet.Cell(row + 2, col + 1).Value = dataTable.Rows[row][col].ToString();
                    }
                }

                // Apply table styles for striped rows
                var tableRange = worksheet.Range(1, 1, dataTable.Rows.Count + 1, dataTable.Columns.Count);
                tableRange.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                tableRange.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                tableRange.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                tableRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                tableRange.Style.Border.InsideBorderColor = XLColor.Gray;
                tableRange.Style.Border.OutsideBorderColor = XLColor.Gray;
                tableRange.Style.Fill.SetBackgroundColor(XLColor.White);
                tableRange.Style.Fill.SetPatternType(XLFillPatternValues.Solid);

                // Apply striped row style
                var rows = tableRange.RowsUsed();
                for (int i = 1; i <= rows.Count(); i++)
                {
                    if (i % 2 == 0)
                    {
                        rows.ElementAt(i - 1).Style.Fill.SetBackgroundColor(XLColor.LightGray);
                    }
                }

                // Save the Excel file to a memory stream
                using (var memoryStream = new System.IO.MemoryStream())
                {
                    workbook.SaveAs(memoryStream);


                    var fileContents = memoryStream.ToArray();
                    var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    var fileName =  filter + "_" + DateTime.Today.ToShortDateString() + ".xlsx";

                    return File(fileContents, contentType, fileName);
                }
            }
        }
    }
}