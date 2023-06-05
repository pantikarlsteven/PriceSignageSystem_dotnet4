using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using PriceSignageSystem.Code;
using PriceSignageSystem.Helper;
using PriceSignageSystem.Models.DatabaseContext;
using PriceSignageSystem.Models.Dto;
using PriceSignageSystem.Models.Interface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;


namespace PriceSignageSystem.Controllers
{
    [SessionExpiration]
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
            var date = _sTRPRCRepository.GetLatestUpdate();

            if (date.Date == DateTime.Now.Date)
            {
                ViewBag.IsDateLatest = true;
            }

            ViewBag.DateVersion = date.Date.ToShortDateString();
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

        public ActionResult SearchByDate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetDataByDate(DateTime startDate, DateTime endDate)
        {
            var startDateDecimal = ConversionHelper.ToDecimal(startDate);
            var endDateDecimal = ConversionHelper.ToDecimal(endDate);

            var data = _sTRPRCRepository.GetDataByDate(startDateDecimal, endDateDecimal).ToList();
            var startDateFormatted = ConversionHelper.ToDecimal(startDate);
            var endDateFormatted = ConversionHelper.ToDecimal(endDate);
            foreach (var item in data) // TEMPORARY -- SOON TO BE DEFINED IN DB
            {
                item.TypeName =   startDateFormatted == item.O3SDT ? "Save"
                                : endDateFormatted == item.O3EDT ? "Regular"
                                : "Save";
                item.SizeName =   item.SelectedSizeId == 1 ? "Whole"
                                : item.SelectedSizeId == 2 ? "Half"
                                : item.SelectedSizeId == 3 ? "Jewelry"
                                : item.SelectedSizeId == 4 ? "Skinny"
                                : "Whole";
                item.CategoryName =   item.SelectedCategoryId == 1 ? "Appliance"
                                    : item.SelectedCategoryId == 2 ? "Non-Appliance"
                                    : "Non-Appliance";
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

        public ActionResult UpdateSTRPRCData()
        {
            var storeId = 201; // temporary
            var count = _sTRPRCRepository.UpdateSTRPRCTable(storeId);
            var data = new UserStoreDto()
            {
                DataCount = count
            };
            return RedirectToAction("Index", data);
        }

        [HttpPost]
        public JsonResult GetAllSizes()
        {
            var sizes = _sizeRepository.GetAllSizes().ToArray(); 

            return Json(sizes);
        }

        [HttpGet]
        public ActionResult PrintPCASTRPRCTest(string id)
        {
            try
            {
                ReportDocument rptH = new ReportDocument();
                var strReport = string.Empty;
                using (var _db = new ApplicationDbContext())
                {

                    var stprcs = (from a in _db.STRPRCs
                                  where a.O3SKU.ToString().ToLower() == id.ToLower()
                                  select new STRPRCDto
                                  {
                                      O3SKU = a.O3SKU,
                                      O3FNAM = a.O3FNAM,
                                      O3IDSC = a.O3IDSC
                                  }).First();

                    //If Brand Name's characters are greater than 14
                    if (stprcs.O3FNAM.Length > 14)
                    {
                        //If Description's characters are greater than 44
                        if (stprcs.O3IDSC.Length > 44)
                            strReport = "WholeReport_DLBrandAndDLDesc.rpt";
                        else
                            strReport = "WholeReport_DLBrandAndSLDesc.rpt";
                    }
                    //Else, Brand Name's characters are less than 14
                    else
                    {
                        //If Description's characters are greater than 44
                        if (stprcs.O3IDSC.Length > 44)
                            strReport = "WholeReport_SLBrandAndDLDesc.rpt";
                        else
                            strReport = "WholeReport_SLBrandAndSLDesc.rpt";
                    }

                    var strReportPath = System.Web.HttpContext.Current.Server.MapPath("~/Reports/CrystalReports/WholeReport/" + strReport);

                    rptH.Load(strReportPath);
                    rptH.SetDatabaseLogon("sa", "@dm1n@8800");
                    rptH.SetParameterValue("sku", id);
                    rptH.SetParameterValue("user", Session["Username"].ToString());

                    Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    var pdfBytes = new byte[stream.Length];
                    stream.Read(pdfBytes, 0, pdfBytes.Length);

                    //rptH.PrintOptions.PrinterName = @"\\199.85.2.2\Canon LBP2900";
                    //rptH.PrintToPrinter(1,true,0,0);

                    Response.AppendHeader("Content-Disposition", "inline; filename=test.pdf");
                    return File(pdfBytes, "application/pdf");
                }
            }
            catch (Exception ex)
            {
                return Content("<h2>Error: " + ex.Message + "</h2>", "text/html");
            }
        }
    }
}