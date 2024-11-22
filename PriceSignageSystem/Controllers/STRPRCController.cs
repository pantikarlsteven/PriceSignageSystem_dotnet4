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
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private readonly IQueueRepository _queueRepository;
        private readonly IEditReasonRepository _reasonRepository;

        public STRPRCController(ISTRPRCRepository sTRPRCRepository, ITypeRepository typeRepository, ISizeRepository sizeRepository, ICategoryRepository categoryRepository, IQueueRepository queueRepository, IEditReasonRepository reasonRepository)
        {
            _sTRPRCRepository = sTRPRCRepository;
            _typeRepository = typeRepository;
            _sizeRepository = sizeRepository;
            _categoryRepository = categoryRepository;
            _queueRepository = queueRepository;
            _reasonRepository = reasonRepository;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetHistoryList()
        {
            var username = User.Identity.Name;
            var data = _queueRepository.GetHistory(username);
            return Json(data);
        }
        public ActionResult Search(string query, string searchFilter, string codeFormat)
        {
            try
            {
                var dto = _sTRPRCRepository.SearchString(query, searchFilter, codeFormat);
                
                if (dto != null/* && dto.IsExemp == "N"*/)
                {

                    var dateString = dto.O3SDT.ToString();
                    if (dateString.Length == 5) 
                    {
                        dateString = "0" + dateString;
                    }


                    DateTime startdateTimeValue = DateTime.ParseExact(dateString, "yyMMdd", CultureInfo.InvariantCulture);
                    dto.StartDateFormattedDate = startdateTimeValue.ToString("yy-MM-dd");
                    

                    if (dto.O3EDT == 999999)
                    {
                        dto.EndDateFormattedDate = "-";
                    }else
                    {
                        DateTime enddateTimeValue = DateTime.ParseExact(dto.O3EDT.ToString(), "yyMMdd", CultureInfo.InvariantCulture);
                        dto.EndDateFormattedDate = enddateTimeValue.ToString("yy-MM-dd");
                    }

                    if (dto.SelectedTypeId == 2)
                    {
                        dto.Sizes = _sizeRepository.GetAllSizes().Where(a => a.Id == 1 || a.Id == 2).Select(a => new SelectListItem
                        {
                            Value = a.Id.ToString(),
                            Text = a.Name
                        }).ToList();
                    }else
                    {
                        dto.Sizes = _sizeRepository.GetAllSizes().Select(a => new SelectListItem
                        {
                            Value = a.Id.ToString(),
                            Text = a.Name
                        }).ToList();
                    }

                    dto.O3REG = decimal.Parse(dto.O3REG.ToString("F2"));
                    dto.O3REGU = decimal.Parse(dto.O3REGU.ToString("F2"));
                    dto.O3POS = decimal.Parse(dto.O3POS.ToString("F2"));
                    dto.O3POSU = decimal.Parse(dto.O3POSU.ToString("F2"));

                    // PROMO ENGINE
                    var promo = _sTRPRCRepository.CheckIfSkuHasPromo(Convert.ToDecimal(query));
                    if (!String.IsNullOrEmpty(promo.PromoType))
                    {
                        dto.Types = _typeRepository.GetAllTypes().Where(a => a.Id == dto.SelectedTypeId || a.Name == promo.PromoType).Select(a => new SelectListItem
                        {
                            Value = a.Id.ToString(),
                            Text = a.Name
                        }).ToList();

                        if (dto.SelectedTypeId == ReportConstants.Type.Save)
                        {
                            if (dto.O3SDT >= promo.StartDate && dto.O3SDT <= promo.EndDate )
                            {
                                dto.O3SDT = promo.StartDate;
                                dto.O3EDT = promo.EndDate;
                                dto.SelectedTypeId = promo.TypeId;
                                dto.PromoVal = promo.PromoVal;
                            }

                            dto.IsExemp = "Y";
                        }
                        else
                        {
                            dto.Types = _typeRepository.GetAllTypes().Where(a => a.Name == promo.PromoType).Select(a => new SelectListItem
                            {
                                Value = a.Id.ToString(),
                                Text = a.Name
                            }).ToList();

                            dto.O3SDT = promo.StartDate;
                            dto.O3EDT = promo.EndDate;
                            dto.SelectedTypeId = promo.TypeId;
                            dto.PromoVal = promo.PromoVal;
                        }
                    }
                    else
                    {
                        dto.Types = _typeRepository.GetAllTypes().Where(a => a.Id == 1 || a.Id == 2).Select(a => new SelectListItem
                        {
                            Value = a.Id.ToString(),
                            Text = a.Name
                        }).ToList();
                    }

                    dto.Categories = _categoryRepository.GetAllCategories().Select(a => new SelectListItem
                    {
                        Value = a.Id.ToString(),
                        Text = a.Name
                    }).ToList();

                }
                else if (dto != null && dto.IsExemp == "Y")
                {
                    dto = new STRPRCDto();
                }
                else
                {
                    dto = dto ?? new STRPRCDto();
                }
                
                ViewBag.IsScannedDtoNull = dto.O3SKU == 0 ? true : false;

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

            if (date.DateUpdated.Date == DateTime.Now.Date)
            {
                ViewBag.IsDateLatest = true;
            }

            ViewBag.DateVersion = date.DateUpdated.ToString("MMM dd yyyy HH:mm:ss tt");
            ViewBag.WithInventory = withInventory;

            return View();
        }
       
        [HttpPost]
        public JsonResult GetDataBySKU(decimal id)
        {
            var dto = _sTRPRCRepository.GetDataBySKU(id);
            var promo = _sTRPRCRepository.CheckIfSkuHasPromo(id);

            dto.SizeArray = _sizeRepository.GetAllSizes().ToArray();
            dto.CategoryArray = _categoryRepository.GetAllCategories().ToArray();

            if (!String.IsNullOrEmpty(promo.PromoType))
            {
                dto.TypeArray = _typeRepository.GetAllTypes().Where(a => a.Id == dto.TypeId || a.Name == promo.PromoType).ToArray();

                if (dto.TypeId == ReportConstants.Type.Save)
                {
                    if (promo.StartDate >= dto.O3SDT)
                    {
                        dto.TypeId = promo.TypeId;
                    }
                }
                else // regular with promo
                {
                    dto.TypeArray = _typeRepository.GetAllTypes().Where(a => a.Name == promo.PromoType).ToArray();
                    dto.TypeId = promo.TypeId;
                }
            }
            else
            {
                if (dto.O3REG < dto.O3POS && dto.TypeId == 2) // Validation for Negative Save
                { 
                    dto.TypeId = 1;
                }
                
                dto.TypeArray = _typeRepository.GetAllTypes().Where(a => a.Id == dto.TypeId).ToArray();
                
            }

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

        [HttpPost]
        public JsonResult GetAllReasons()
        {
            var reasons = _reasonRepository.GetAllReasons().ToArray();

            return Json(reasons);
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
                //model.O3FNAM = "LUNA DA LUNA";
                //model.O3IDSC = "CABERNET MERLOT/RED BLEND 750MLL";
                model.SizeId = 1;
                model.CategoryId = 1;
                model.UserName = "Noel";
                textToImage.GetImageWidth(model.O3FNAM, model.O3IDSC, model.SizeId);
                model.IsSLBrand = textToImage.IsSLBrand;
                model.IsSLDescription = textToImage.IsSLDescription;

                report.SetDataSource(ConversionHelper.ConvertObjectToDataTable(model));
                ////report.PrintOptions.PrinterName = @"\\199.85.2.3\Canon LBP2900";
                //Logs.WriteToFile("test 1");
                report.PrintToPrinter(1, false, 0, 0);

                //System.Drawing.Printing.PrinterSettings printersettings = new System.Drawing.Printing.PrinterSettings();
                //printersettings.PrinterName = "\\\\SYSTEMSPC_8801\\Canon LBP2900";
                //printersettings.Copies = 1;
                //printersettings.Collate = false;
                //report.PrintToPrinter(printersettings, new System.Drawing.Printing.PageSettings(), false);

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

        public ActionResult UpdatedSTRPRC(string date)
        {
            ViewBag.DateFilter = date;

            var result = _sTRPRCRepository.GetLatestUpdate();

            if (result.DateUpdated.Date == DateTime.Now.Date)
            {
                ViewBag.IsDateLatest = true;
            }

            ViewBag.DateVersion = result.DateUpdated.ToString("MMM dd yyyy HH:mm:ss tt");

            return View();
        }


        [HttpGet]
        public async Task<ActionResult> GetUpdatedData(string dateFilter)
        {
            var result = _sTRPRCRepository.GetLatestUpdate();
            decimal dateFilterInDecimal = 0;

            if (!string.IsNullOrEmpty(dateFilter))
            {
                DateTime date = DateTime.Parse(dateFilter);
                dateFilterInDecimal = date.Year % 100 * 10000 + date.Month * 100 + date.Day;
            }

            List<STRPRCLogDto> data;
            if (string.IsNullOrEmpty(dateFilter) || dateFilterInDecimal == result.LatestDate)
            {
                data = await _sTRPRCRepository.GetSKUUpdates(result.DateUpdated.ToString("yyyy-MM-dd"));
            }
            else
            {
                data = await _sTRPRCRepository.GetSKUUpdatesFromHistory(dateFilter);
            }

            string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(data);

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

        [AllowAnonymous]
        [HttpGet]
        public async Task<JsonResult> CheckSTRPRCUpdates()
        {
            var result = _sTRPRCRepository.GetLatestUpdate();

            if (result != null)
            {
                if (result.DateUpdated.Date == DateTime.Now.Date)
                    return Json(true, JsonRequestBehavior.AllowGet);
            }

            var data151 = _sTRPRCRepository.CheckSTRPRCUpdates(int.Parse(ConfigurationManager.AppSettings["StoreID"]));
            if (DateTime.TryParseExact(data151.ToString(), "yyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
            {
                if (result == null)
                    return Json(false, JsonRequestBehavior.AllowGet);
                else
                {
                    // Now you can format the parsed date as needed
                    if (parsedDate.Date != DateTime.Now.Date || parsedDate.Date != result.DateUpdated.Date)
                        return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                Console.WriteLine("Invalid date format");
                await _sTRPRCRepository.UpdateSTRPRC151(int.Parse(ConfigurationManager.AppSettings["StoreID"]));
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            //if (date.DateUpdated.Date != DateTime.Now.Date)
            //    return Json(false, JsonRequestBehavior.AllowGet);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetSKUDetails(decimal O3SKU)
        {
            var data = _sTRPRCRepository.GetSKUDetails(O3SKU);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> UpdatePCA()
        {
            var result = _sTRPRCRepository.GetLatestUpdate();
            var decimalDateToday = ConversionHelper.ToDecimal(DateTime.Now);
            if (result != null)
            {
                //if STRPRCs in club is updated then update centralized exemptions in merchandise and get latest inventory
                if (result.LatestDate == decimalDateToday && result.O3SDT == decimalDateToday)
                {
                    await _sTRPRCRepository.UpdateCentralizedExemptions(result.LatestDate);
                    _sTRPRCRepository.GetLatestInventory(ConfigurationManager.AppSettings["StoreID"].ToString());
                }
                // if STRPRCs in club is not updated then check XXX_STRPRC in 0.151
                else
                {
                    var isUpdated = _sTRPRCRepository.Check151STRPRCChanges_LatestDate(int.Parse(ConfigurationManager.AppSettings["StoreID"]));
                    // if XXX_STRPRC is updated then run sp_GetLatestSTRPRCTable stored proc in club
                    if (isUpdated)
                    {
                        _sTRPRCRepository.UpdateSTRPRCTable(int.Parse(ConfigurationManager.AppSettings["StoreID"]));
                        result = _sTRPRCRepository.GetLatestUpdate();
                        await _sTRPRCRepository.UpdateCentralizedExemptions(result.LatestDate);
                        _sTRPRCRepository.GetLatestInventory(ConfigurationManager.AppSettings["StoreID"].ToString());
                    }
                    // if XXX_STRPRC is not updated then run sp_GetSTRPRC in 0.151 and sp_GetLatestSTRPRCTable stored proc in club
                    else
                    {
                        await _sTRPRCRepository.UpdateSTRPRC151(int.Parse(ConfigurationManager.AppSettings["StoreID"]));
                        _sTRPRCRepository.UpdateSTRPRCTable(int.Parse(ConfigurationManager.AppSettings["StoreID"]));
                        result = _sTRPRCRepository.GetLatestUpdate();
                        await _sTRPRCRepository.UpdateCentralizedExemptions(result.LatestDate);
                        _sTRPRCRepository.GetLatestInventory(ConfigurationManager.AppSettings["StoreID"].ToString());
                    }
                }
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult GetLatestInventory()
        {
            var result = _sTRPRCRepository.GetLatestInventory(ConfigurationManager.AppSettings["StoreID"].ToString());
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult CheckPCALatestDates()
        {
            var decimalDateToday = ConversionHelper.ToDecimal(DateTime.Now);
            var result = _sTRPRCRepository.GetLatestUpdate();
            if (result != null)
            {
                if (result.LatestDate == decimalDateToday && result.O3SDT == decimalDateToday)
                    return Json(true, JsonRequestBehavior.AllowGet);
            }

            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> LoadPCA(string dateFilter)
        {
            var dateToday = ConversionHelper.ToDecimal(DateTime.Now);
            DateTime currentTime = DateTime.Now;
            DateTime startTime = DateTime.Today.AddHours(4);
            DateTime endTime = DateTime.Today.AddHours(4).AddMinutes(15);

            if (currentTime >= startTime && currentTime < endTime)
            {
                return View("MaintenanceError");
            }

            var result = _sTRPRCRepository.GetLatestUpdate();

            var data = new STRPRCDto();
            decimal dateFilterInDecimal = 0;
            if (!string.IsNullOrEmpty(dateFilter))
            {
                DateTime date = DateTime.Parse(dateFilter);
                dateFilterInDecimal = date.Year % 100 * 10000 + date.Month * 100 + date.Day;
            }
            List<STRPRCDto> rawData;
            List<STRPRCDto> consignmentList;
            if (string.IsNullOrEmpty(dateFilter) || dateFilterInDecimal == result.LatestDate)
            {
                rawData = await _sTRPRCRepository.GetDataByStartDate(result.LatestDate);
                consignmentList = await _sTRPRCRepository.GetAllConsignment(result.LatestDate);
                rawData = rawData.Where(a => a.O3SDT == result.LatestDate).ToList();
            }
            else
            {
                result.LatestDate = dateFilterInDecimal;
                rawData = await _sTRPRCRepository.GetDataByPCAHistory(dateFilter, dateFilterInDecimal);
                consignmentList = await _sTRPRCRepository.GetDataByConsignmentHistory(dateFilter);
                rawData = rawData.Where(a => a.PCADate.ToString("yyyy-MM-dd") == dateFilter).ToList();
            }

            data.LatestDate = result.LatestDate;
            data.WithInventoryList = rawData
                .Where(a => (a.HasInventory == "Y" && a.IsExemp == "N" && a.O3TYPE != "CO")
                || (a.ExempType == "Negative Save" && a.IBHAND > 0) // Negative save with positive onhand
                || (a.IsDoublePromo == "Y")).ToList(); // Double promo
            data.ExcemptionList = rawData
                .Where(a => (a.HasInventory == "" || a.IsExemp == "Y")
                && a.O3SKU.ToString().Length != 4 // REMOVE FRESH SKUS
                && a.O3SDT == result.LatestDate).ToList(); 
            data.ConsignmentList = consignmentList.OrderByDescending(o => o.O3SDT).Where(f => f.O3FLAG3 == "Y").ToList();
            var commonIds = data.ExcemptionList.Select(x => x.O3SKU).Intersect(consignmentList.Select(x => x.O3SKU)).ToList();
            var coList = consignmentList.Where(x => !commonIds.Contains(x.O3SKU)).ToList();
            var ExempCoList = coList.Where(a => a.O3FLAG3 != "Y" || a.O3FLAG3 == null).ToList();
            data.ExcemptionList.AddRange(ExempCoList);

            string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(data);

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

        [HttpGet]
        public FileResult ExportDataTableToExcel(string tab, DateTime date, string filter, string selectedRows )
        {
            var selectedSkus = new decimal[] { };
            if (!string.IsNullOrEmpty(selectedRows))
            {
                string[] selectedRowsArray = selectedRows.Split(',');
                selectedSkus = selectedRowsArray.Select(decimal.Parse).ToArray();
            }


            var decimalDate = ConversionHelper.ToDecimal(date);
            var dataTable = new DataTable();
            var noccList = new List<ExportPCAExemptionDto>();
            if (tab == "NewExemptionInventory") { 
               var toExportRawData = _sTRPRCRepository.PCAToExportExemption().ToList();
                if (tab == "WithInventory")
                {
                    toExportRawData = toExportRawData.Where(a => a.WithInventory == "Yes" && a.IsExemp == "No").ToList();
                }
                else
                {

                    if (filter == "all")
                    {
                        if (string.IsNullOrEmpty(selectedRows))
                            toExportRawData = toExportRawData.Where(a => a.IsExemp == "Yes" || a.WithInventory == "No").ToList();
                        else
                            toExportRawData = toExportRawData.Where(item => selectedSkus.Contains(item.SKU)).ToList();
                    }

                    else if (filter == "saveZero")
                    {
                        if (string.IsNullOrEmpty(selectedRows))
                            toExportRawData = toExportRawData.Where(a => a.ExemptionType == "Zero Save").ToList();
                        else
                            toExportRawData = toExportRawData.Where(item => selectedSkus.Contains(item.SKU)).ToList();
                    }
                    else if (filter == "negative")
                    {
                        if (string.IsNullOrEmpty(selectedRows))
                            toExportRawData = toExportRawData.Where(a => a.ExemptionType == "Negative Inventory").ToList();
                        else
                            toExportRawData = toExportRawData.Where(item => selectedSkus.Contains(item.SKU)).ToList();
                    }
                    else if (filter == "zero")
                    {
                        if (string.IsNullOrEmpty(selectedRows))
                            toExportRawData = toExportRawData.Where(a => a.ExemptionType == "Zero Inventory").ToList();
                        else
                            toExportRawData = toExportRawData.Where(item => selectedSkus.Contains(item.SKU)).ToList();
                    }
                    else if (filter == "negativeSave")
                    {
                        if (string.IsNullOrEmpty(selectedRows))
                            toExportRawData = toExportRawData.Where(a => a.ExemptionType == "Negative Save").ToList();
                        else
                            toExportRawData = toExportRawData.Where(item => selectedSkus.Contains(item.SKU)).ToList();
                    }
                    else if (filter == "noCC")
                    {
                        if (string.IsNullOrEmpty(selectedRows))
                            noccList = _sTRPRCRepository.GetAllNoConsignmentContract();
                        else
                            noccList = _sTRPRCRepository.GetAllNoConsignmentContract().Where(item => selectedSkus.Contains(item.SKU)).ToList();

                    }
                    else if (filter == "doublePromo")
                    {
                        if (string.IsNullOrEmpty(selectedRows))
                            toExportRawData = toExportRawData.Where(a => a.ExemptionType == "Double Promo").ToList();
                        else
                            toExportRawData = toExportRawData.Where(item => selectedSkus.Contains(item.SKU)).ToList();

                    }
                    
                }

                if (filter != "noCC")
                    dataTable = ConversionHelper.ConvertListToDataTable(toExportRawData);
                else
                    dataTable = ConversionHelper.ConvertListToDataTable(noccList);

            }
            else
            {
                var toExportRawData = _sTRPRCRepository.PCAToExport().ToList();
                var consignmentData = new List<ExportPCADto>();
                if (tab == "WithInventory")
                {
                    if (string.IsNullOrEmpty(selectedRows))
                        toExportRawData = toExportRawData.Where(a => a.WithInventory == "Yes" || a.IsExemption == "No").ToList();
                    else
                        toExportRawData = toExportRawData.Where(item => selectedSkus.Contains(item.SKU)).ToList();

                }
                else if (tab == "Consignment")
                {
                    consignmentData = _sTRPRCRepository.GetConsignmentToExport(selectedSkus);
                }
                else
                {
                    toExportRawData = toExportRawData.Where(a => a.IsExemption == "Yes" || a.WithInventory == "No").ToList();

                }

                if (tab == "Consignment")
                    dataTable = ConversionHelper.ConvertListToDataTable(consignmentData);
                else
                    dataTable = ConversionHelper.ConvertListToDataTable(toExportRawData);
            }

           
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

                // HIDE COLUMNS
                worksheet.Columns("Q:T").Delete();

                // Save the Excel file to a memory stream
                using (var memoryStream = new System.IO.MemoryStream())
                {
                    workbook.SaveAs(memoryStream);


                    var fileContents = memoryStream.ToArray();
                    var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    var fileName = tab + "_" + DateTime.Today.ToShortDateString() + ".xlsx";

                    return File(fileContents, contentType, fileName);
                }
            }
        }

        public ActionResult PCA(string date)
        {
            ViewBag.DateFilter = date;
            return View();
        }

        public ActionResult Scan()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<JsonResult> CheckCentralizedExemptionStatus()
        {
            var result = await _sTRPRCRepository.CheckCentralizedExemptionStatus();
            var club = _sTRPRCRepository.GetLatestUpdate();

            if (result.Id > 0)
            {
                if (result.DateUpdated.Value.Date != DateTime.Now.Date
                    && club.DateUpdated.Date == DateTime.Now.Date)
                    return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(false, JsonRequestBehavior.AllowGet);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task UpdateCentralizedExemption()
        {
            var result = await _sTRPRCRepository.CheckCentralizedExemptionStatus();
            var data151 = _sTRPRCRepository.CheckSTRPRCUpdates(int.Parse(ConfigurationManager.AppSettings["StoreID"]));
            if (result.Id > 0)
            {
                if (!result.OngoingUpdate)
                {
                    _sTRPRCRepository.UpdateCentralizedExemptionStatus(result, true); //update ongoingupdate to true here;
                    await _sTRPRCRepository.UpdateCentralizedExemptions(data151);
                    _sTRPRCRepository.UpdateCentralizedExemptionStatus(result, false); //update ongoingupdate to false here and dateupdated to datetime.now;
                }
            }
            else
            {
                //add row to exemptionstatus here;
                _sTRPRCRepository.UpdateCentralizedExemptionStatus(result, true); //update ongoingupdate to true here;
                await _sTRPRCRepository.UpdateCentralizedExemptions(data151);
                result = await _sTRPRCRepository.CheckCentralizedExemptionStatus();
                _sTRPRCRepository.UpdateCentralizedExemptionStatus(result, false); //update ongoingupdate to false here and dateupdated to datetime.now;
            }
        }

        [HttpPost]
        public ActionResult Sync()
        {
            var result = _sTRPRCRepository.SyncFromNew();

            return Json(result);
        }

        [HttpPost]
        public ActionResult UpdateUPC()
        {
            var result = _sTRPRCRepository.UpdateUPC();

            return Json(result);
        }

        [HttpPost]
        public ActionResult OndemandTypeOnchange(decimal sku, int typeId)
        {
            var result = new STRPRCDto();

            if (typeId == ReportConstants.Type.Save || typeId == ReportConstants.Type.Regular)
            {
                result = _sTRPRCRepository.GetDataBySKU(sku);
                result.PromoVal = 0;
            }
            else
            {
                var promo = _sTRPRCRepository.CheckIfSkuHasPromo(sku);
                result.O3SKU = promo.Sku;
                result.O3SDT = promo.StartDate;
                result.O3EDT = promo.EndDate;
                result.PromoVal = promo.PromoVal;
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult GetPCAData(decimal id, string dateFilter)
        {
            string todayDate = DateTime.Now.ToString("yyyy-MM-dd");
            var dto = new STRPRCDto();
            var promo = new PromoEngineDto();

            if (String.IsNullOrEmpty(dateFilter) || dateFilter == todayDate)
            {
                dto = _sTRPRCRepository.GetSkuFromPCA(id);
                promo = _sTRPRCRepository.CheckIfSkuHasPromo(id);
            }
            else
            {
                dto = _sTRPRCRepository.GetSkuFromPCAHistory(id, dateFilter);
                promo = _sTRPRCRepository.CheckIfSkuHasPromoHistory(id, dateFilter);
            }

            dto.SizeArray = _sizeRepository.GetAllSizes().ToArray();
            dto.CategoryArray = _categoryRepository.GetAllCategories().ToArray();

            if (!String.IsNullOrEmpty(promo.PromoType))
            {
                dto.TypeArray = _typeRepository.GetAllTypes().Where(a => a.Id == dto.TypeId || a.Name == promo.PromoType).ToArray();

                if (dto.TypeId == ReportConstants.Type.Save)
                {
                    if (promo.StartDate >= dto.O3SDT)
                    {
                        dto.TypeId = promo.TypeId;
                    }
                }
                else // regular with promo
                {
                    dto.TypeArray = _typeRepository.GetAllTypes().Where(a => a.Name == promo.PromoType).ToArray();
                    dto.TypeId = promo.TypeId;
                }
            }
            else
            {
                if (dto.O3REGU < dto.O3POS && dto.TypeId == 2) // Validation for Negative Save
                {
                    dto.TypeId = 1;
                }

                dto.TypeArray = _typeRepository.GetAllTypes().Where(a => a.Id == dto.TypeId).ToArray();

            }

            return Json(dto);
        }
    }
}