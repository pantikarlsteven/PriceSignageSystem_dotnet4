﻿using CrystalDecisions.CrystalReports.Engine;
using PriceSignageSystem.Code;
using PriceSignageSystem.Helper;
using PriceSignageSystem.Models.Constants;
using PriceSignageSystem.Models.Dto;
using PriceSignageSystem.Models.Interface;
using System;
using System.Configuration;
using System.Data;
using System.Globalization;
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
            var date = _sTRPRCRepository.GetLatestUpdate();

            if (date.Date == DateTime.Now.Date)
            {
                ViewBag.IsDateLatest = true;
            }

            ViewBag.DateVersion = date.ToString("MMM dd yyyy HH:mm:ss tt"); ;
            return View();
        }

        [HttpPost]
        public ActionResult GetDataByDate(DateTime startDate, DateTime endDate)
        {
            var startDateFormatted = ConversionHelper.ToDecimal(startDate);
            var endDateFormatted = ConversionHelper.ToDecimal(endDate);

            var data = _sTRPRCRepository.GetDataByDate(startDateFormatted, endDateFormatted).ToList();
          
            foreach (var item in data) // TEMPORARY -- SOON TO BE DEFINED IN DB
            {
                item.TypeName = startDateFormatted == item.O3SDT && item.O3EDT != 999999 ? "Save"
                                : startDateFormatted == item.O3SDT && item.O3EDT == 999999 ? "Regular"
                                : "Save";
                item.SizeName = item.SizeId == 1 ? "Whole"
                                : item.SizeId == 2 ? "Half"
                                : item.SizeId == 3 ? "Jewelry"
                                : item.SizeId == 4 ? "Skinny"
                                : "Whole";
                item.CategoryName = item.CategoryId == 1 ? "Appliance"
                                    : item.CategoryId == 2 ? "Non-Appliance"
                                    : "Non-Appliance";
            }

            //UPDATE SIZE, TYPE AND CATEGORY
            _sTRPRCRepository.UpdateSelection(startDateFormatted, endDateFormatted);
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
            var storeId = int.Parse(ConfigurationManager.AppSettings["StoreID"]);
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
        public void PrintPCASTRPRCTest(decimal id)
        {
            Logs.WriteToFile("test 0");

            try
            {
                ReportDocument rptH = new ReportDocument();
                var strReport = string.Empty;

                Logs.WriteToFile("test 00");

                var strReportPath = Server.MapPath(ReportConstants.Dynamic_HalfReportPath);
                Logs.WriteToFile(strReportPath);
                rptH.Load(strReportPath);
                Logs.WriteToFile("test 01");

                rptH.SetDataSource(ConversionHelper.ConvertObjectToDataTable(_sTRPRCRepository.GetDataBySKU(id)));
                Logs.WriteToFile("test 1");
                //rptH.PrintOptions.PrinterName = "Canon LBP2900 (redirected 3)";

                rptH.PrintToPrinter(1, true, 0, 0);
                Logs.WriteToFile("test 2");

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
    }
}