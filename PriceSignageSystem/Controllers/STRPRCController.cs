using ClosedXML.Excel;
using CrystalDecisions.CrystalReports.Engine;
using Newtonsoft.Json;
using PriceSignageSystem.Code;
using PriceSignageSystem.Code.CustomValidations;
using PriceSignageSystem.Helper;
using PriceSignageSystem.Models.Constants;
using PriceSignageSystem.Models.Dto;
using PriceSignageSystem.Models.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace PriceSignageSystem.Controllers
{
    [CustomAuthorize]
    public class STRPRCController : Controller
    {
        private readonly ISTRPRCRepository _sTRPRCRepository;
        private readonly ITypeRepository _typeRepository;
        private readonly ISizeRepository _sizeRepository;
        private readonly ICategoryRepository _categoryRepository;

        public STRPRCController(ISTRPRCRepository sTRPRCRepository, ITypeRepository typeRepository, ISizeRepository sizeRepository, ICategoryRepository categoryRepository)
        {
            _sTRPRCRepository = sTRPRCRepository;
            _typeRepository = typeRepository;
            _sizeRepository = sizeRepository;
            _categoryRepository = categoryRepository;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Search(string query)
        {
            try
            {
                var dto = _sTRPRCRepository.SearchString(query);

                if (dto != null)
                {
                    DateTime startdateTimeValue = DateTime.ParseExact(dto.O3SDT.ToString(), "yyMMdd", CultureInfo.InvariantCulture);
                    dto.StartDateFormattedDate = startdateTimeValue.ToString("yy-MM-dd");

                    if (dto.O3EDT == 999999)
                    {
                        dto.EndDateFormattedDate = "-";
                    }

                    else
                    {
                        DateTime enddateTimeValue = DateTime.ParseExact(dto.O3EDT.ToString(), "yyMMdd", CultureInfo.InvariantCulture);
                        dto.EndDateFormattedDate = enddateTimeValue.ToString("yy-MM-dd");
                    }

                    dto.Sizes = _sizeRepository.GetAllSizes().Select(a => new SelectListItem
                    {
                        Value = a.Id.ToString(),
                        Text = a.Name
                    }).ToList();

                    dto.Types = _typeRepository.GetAllTypes().Select(a => new SelectListItem
                    {
                        Value = a.Id.ToString(),
                        Text = a.Name
                    }).ToList();

                    dto.Categories = _categoryRepository.GetAllCategories().Select(a => new SelectListItem
                    {
                        Value = a.Id.ToString(),
                        Text = a.Name
                    }).ToList();


                }
                else
                {
                    dto = dto ?? new STRPRCDto();
                }
                return PartialView("~/Views/STRPRC/_SearchResultPartialView.cshtml", dto);

            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception occurred: " + ex.Message);
                Console.WriteLine("InnerException: " + ex.InnerException?.Message);
                return View();
            }
        }

        public ActionResult SearchByDate(bool withInventory)
        {
            var date = _sTRPRCRepository.GetLatestUpdate();

            if (date.Date == DateTime.Now.Date)
            {
                ViewBag.IsDateLatest = true;
            }

            ViewBag.DateVersion = date.ToString("MMM dd yyyy HH:mm:ss tt");
            ViewBag.WithInventory = withInventory;

            return View();
        }
       
        [HttpPost]
        public ActionResult GetPCAByDate(DateTime startDate)
        {
            var startDateFormatted = ConversionHelper.ToDecimal(startDate);
            var data = new STRPRCDto();
            var rawData = _sTRPRCRepository.GetDataByStartDate(startDateFormatted).ToList();
            data.WithInventoryList = rawData.Where(a => a.HasInventory == "Y").ToList();
            data.WithoutInventoryList = rawData.Where(a => a.HasInventory == string.Empty).ToList();
            data.ExcemptionList = rawData.Where(a => a.O3SDT == a.O3EDT).ToList();

            foreach (var item in data.WithInventoryList)
            {
                item.TypeName = startDateFormatted == item.O3SDT && item.O3EDT != 999999 ? "Save"
                                : startDateFormatted == item.O3SDT && item.O3EDT == 999999 ? "Regular"
                                : "Save";
                item.SizeName = item.SizeId == 1 ? "Whole"
                                : item.SizeId == 2 ? "Skinny"
                                : item.SizeId == 3 ? "1/8"
                                : item.SizeId == 4 ? "Jewelry"
                                : "Whole";
                item.CategoryName = item.CategoryId == 1 ? "Appliance"
                                    : item.CategoryId == 2 ? "Non-Appliance"
                                    : "Non-Appliance";
                item.IsPrinted = item.IsPrinted == "1" ? "Yes" : "No";
                item.IsReverted = item.IsReverted == "Y" ? "Yes" : "No";
            }

            foreach (var item in data.WithoutInventoryList)
            {
                item.TypeName = startDateFormatted == item.O3SDT && item.O3EDT != 999999 ? "Save"
                                : startDateFormatted == item.O3SDT && item.O3EDT == 999999 ? "Regular"
                                : "Save";
                item.SizeName = item.SizeId == 1 ? "Whole"
                                : item.SizeId == 2 ? "Skinny"
                                : item.SizeId == 3 ? "1/8"
                                : item.SizeId == 4 ? "Jewelry"
                                : "Whole";
                item.CategoryName = item.CategoryId == 1 ? "Appliance"
                                    : item.CategoryId == 2 ? "Non-Appliance"
                                    : "Non-Appliance";
                item.IsPrinted = item.IsPrinted == "1" ? "Yes" : "No";
                item.IsReverted = item.IsReverted == "Y" ? "Yes" : "No";
            }

            foreach (var item in data.ExcemptionList)
            {
                item.TypeName = startDateFormatted == item.O3SDT && item.O3EDT != 999999 ? "Save"
                                : startDateFormatted == item.O3SDT && item.O3EDT == 999999 ? "Regular"
                                : "Save";
                item.SizeName = item.SizeId == 1 ? "Whole"
                                : item.SizeId == 2 ? "Skinny"
                                : item.SizeId == 3 ? "1/8"
                                : item.SizeId == 4 ? "Jewelry"
                                : "Whole";
                item.CategoryName = item.CategoryId == 1 ? "Appliance"
                                    : item.CategoryId == 2 ? "Non-Appliance"
                                    : "Non-Appliance";
                item.IsPrinted = item.IsPrinted == "1" ? "Yes" : "No";
                item.IsReverted = item.IsReverted == "Y" ? "Yes" : "No";
            }

            return Json(data);
        }

        [HttpPost]
        public JsonResult GetDataBySKU(decimal id)
        {
            var dto = _sTRPRCRepository.GetDataBySKU(id);

            dto.SizeArray = _sizeRepository.GetAllSizes().ToArray();
            dto.TypeArray = _typeRepository.GetAllTypes().ToArray();
            dto.CategoryArray = _categoryRepository.GetAllCategories().ToArray();

            return Json(dto);
        }
        [HttpPost]
        public ActionResult UpdateSTRPRCData()
        {
            var storeId = int.Parse(ConfigurationManager.AppSettings["StoreID"]);
            var startdate = _sTRPRCRepository.UpdateSTRPRCTable(storeId);
            var dateString = startdate.ToString();
            //var dateString = (230620).ToString();

            DateTime date;
            DateTime.TryParseExact(dateString, "yyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);

            return Json(date.Date);
        }

        [HttpPost]
        public JsonResult GetAllSizes()
        {
            var sizes = _sizeRepository.GetAllSizes().ToArray();

            return Json(sizes);
        }

        [HttpGet]
        public void PrintPCASTRPRCTest(decimal id)
        {
            Logs.WriteToFile("test 0");

            try
            {
                var model = _sTRPRCRepository.GetReportData(id);
                ReportDocument report = new ReportDocument();
                var strReport = string.Empty;

                Logs.WriteToFile("test 0");

                var path = Server.MapPath(ReportConstants.Dynamic_WholeReportPath);
                report.Load(path);

                var textToImage = new TextToImage();
                textToImage.GetImageWidth(model.O3FNAM, model.O3IDSC, model.SizeId);
                model.IsSLBrand = textToImage.IsSLBrand;
                model.IsSLDescription = textToImage.IsSLDescription;

                report.SetDataSource(ConversionHelper.ConvertObjectToDataTable(model));
                //report.PrintOptions.PrinterName = @"\\199.85.2.3\Canon LBP2900";
                Logs.WriteToFile("test 1");
                //report.PrintToPrinter(1, false, 0, 0);

                System.Drawing.Printing.PrinterSettings printersettings = new System.Drawing.Printing.PrinterSettings();
                printersettings.PrinterName = "\\\\SYSTEMSPC_8801\\Canon LBP2900";
                printersettings.Copies = 1;
                printersettings.Collate = false;
                report.PrintToPrinter(printersettings, new System.Drawing.Printing.PageSettings(), false);

                Logs.WriteToFile("test 2");
                report.Close();
                report.Dispose();
                //Response.AppendHeader("Content-Disposition", "inline; filename=test.pdf");
                //return File(pdfBytes, "application/pdf");

            }
            catch (Exception ex)
            {
                Logs.WriteToFile(ex.Message);
                Logs.WriteToFile(ex.InnerException.Message);
                Logs.WriteToFile(ex.InnerException.StackTrace);
                Logs.WriteToFile(ex.InnerException.Source);



                //return Content("<h2>Error: " + ex.Message + "</h2>", "text/html");
            }
        }

        public ActionResult UpdatedSTRPRC()
        {
            var date = _sTRPRCRepository.GetLatestUpdate();

            if (date.Date == DateTime.Now.Date)
            {
                ViewBag.IsDateLatest = true;
            }

            ViewBag.DateVersion = date.ToString("MMM dd yyyy HH:mm:ss tt");

            return View();
        }


        [HttpGet]
        public ActionResult GetUpdatedData()
        {
            var data = _sTRPRCRepository.GetUpdatedData();
            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpGet]
        public JsonResult CheckSTRPRCUpdates()
        {
            var date = _sTRPRCRepository.GetLatestUpdate();
            if (date.Date != DateTime.Now.Date)
                return Json(false, JsonRequestBehavior.AllowGet);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult LoadPCA()
        {
            var date = _sTRPRCRepository.GetLatestUpdate();
            if (date.Date != DateTime.Now.Date)
                _sTRPRCRepository.UpdateSTRPRCTable(int.Parse(ConfigurationManager.AppSettings["StoreID"]));

            var data = new STRPRCDto();
            var LatestPCAData = _sTRPRCRepository.GetLatestPCAData().ToList();
            
            data.WithInventoryList = LatestPCAData.Where(a => a.HasInventory == "Y").ToList();
            data.WithoutInventoryList = LatestPCAData.Where(a => a.HasInventory == string.Empty).ToList();
            data.ExcemptionList = LatestPCAData.Where(a => a.O3SDT == a.O3EDT).ToList();
            foreach (var item in data.WithInventoryList)
            {
                item.TypeName =  item.TypeId == 2 ? "Save"
                                : item.TypeId == 1 ? "Regular"
                                : "Save";
                item.SizeName = item.SizeId == 1 ? "Whole"
                                : item.SizeId == 2 ? "Skinny"
                                : item.SizeId == 3 ? "1/8"
                                : item.SizeId == 4 ? "Jewelry"
                                : "Whole";
                item.CategoryName = item.CategoryId == 1 ? "Appliance"
                                    : item.CategoryId == 2 ? "Non-Appliance"
                                    : "Non-Appliance";
                item.IsPrinted = item.IsPrinted == "True" ? "Yes" : "No";
                item.IsReverted = item.IsReverted == "Y" ? "Yes" : "No";
            }

            foreach (var item in data.WithoutInventoryList)
            {
                item.TypeName = item.TypeId == 2 ? "Save"
                                : item.TypeId == 1 ? "Regular"
                                : "Save";
                item.SizeName = item.SizeId == 1 ? "Whole"
                                : item.SizeId == 2 ? "Skinny"
                                : item.SizeId == 3 ? "1/8"
                                : item.SizeId == 4 ? "Jewelry"
                                : "Whole";
                item.CategoryName = item.CategoryId == 1 ? "Appliance"
                                    : item.CategoryId == 2 ? "Non-Appliance"
                                    : "Non-Appliance";
                item.IsPrinted = item.IsPrinted == "True" ? "Yes" : "No";
                item.IsReverted = item.IsReverted == "Y" ? "Yes" : "No";
            }

            foreach (var item in data.ExcemptionList)
            {
                item.TypeName = item.TypeId == 2 ? "Save"
                                : item.TypeId == 1 ? "Regular"
                                : "Save";
                item.SizeName = item.SizeId == 1 ? "Whole"
                                : item.SizeId == 2 ? "Skinny"
                                : item.SizeId == 3 ? "1/8"
                                : item.SizeId == 4 ? "Jewelry"
                                : "Whole";
                item.CategoryName = item.CategoryId == 1 ? "Appliance"
                                    : item.CategoryId == 2 ? "Non-Appliance"
                                    : "Non-Appliance";
                item.IsPrinted = item.IsPrinted == "True" ? "Yes" : "No";
                item.IsReverted = item.IsReverted == "Y" ? "Yes" : "No";
            }

            return Json(data);
        }

        [HttpGet]
        public FileResult ExportDataTableToExcel(bool withInventory)
        {
            var toExport = _sTRPRCRepository.PCAToExport(withInventory).ToList();
            foreach (var item in toExport)
            {
                item.IsPrinted = item.IsPrinted == "True" ? "Yes" : "No";
                item.IsReverted = item.IsReverted == "Y" ? "Yes" : "No";
            }

            var dataTable = ConversionHelper.ConvertListToDataTable(toExport);
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

                    // Return the Excel file as a downloadable response
                    var fileContents = memoryStream.ToArray();
                    var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    var fileName = string.Empty;
                    if (withInventory)
                    {
                         fileName = "PCA_With_Inventory.xlsx"; // Default filename

                    }
                    else
                    {
                         fileName = "PCA_Without_Inventory.xlsx"; // Default filename

                    }

                    return File(fileContents, contentType, fileName);
                }
            }
        }

        public ActionResult PCA()
        {
            return View();
        }
    }
}