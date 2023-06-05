﻿using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using PriceSignageSystem.Code;
using PriceSignageSystem.Helper;
using PriceSignageSystem.Models.Dto;
using PriceSignageSystem.Models.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
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
                item.SizeName =   item.SizeId == 1 ? "Whole"
                                : item.SizeId == 2 ? "Half"
                                : item.SizeId == 3 ? "Jewelry"
                                : item.SizeId == 4 ? "Skinny"
                                : "Whole";
                item.CategoryName =   item.CategoryId == 1 ? "Appliance"
                                    : item.CategoryId == 2 ? "Non-Appliance"
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
        public ActionResult PrintPCASTRPRCTest(string id)
        {
            try
            {
                ReportDocument rptH = new ReportDocument();
                string strReportPath = System.Web.HttpContext.Current.Server.MapPath("~/Reports/CrystalReports/WholeReport/WholeReport_SLBrandAndSLDesc.rpt");
                rptH.Load(strReportPath);
                rptH.SetDatabaseLogon("sa", "@dm1n@8800");
                rptH.SetParameterValue("sku", id);
                rptH.SetParameterValue("user", Session["Username"].ToString());

                Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                var pdfBytes = new byte[stream.Length];
                stream.Read(pdfBytes, 0, pdfBytes.Length);

                Response.AppendHeader("Content-Disposition", "inline; filename=test.pdf");
                return File(pdfBytes, "application/pdf");
            }
            catch (Exception ex)
            {
                return Content("<h2>Error: " + ex.Message + "</h2>", "text/html");
            }
        }
    }
}