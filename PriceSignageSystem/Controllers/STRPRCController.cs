using PriceSignageSystem.Helper;
using PriceSignageSystem.Models.Dto;
using PriceSignageSystem.Models.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic.Core;


namespace PriceSignageSystem.Controllers
{
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

                return PartialView("~/Views/STRPRC/_SearchResultPartialView.cshtml", dto);

            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }

        }

        public ActionResult ListByDate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ListByDate(DateTime fromStartDate/*, DateTime toEndDate*/)
        {
            var startDateToDecimal = ConversionHelper.ToDecimal(fromStartDate);
            //var endDateToDecimal = ConversionHelper.ToDecimal(toEndDate);

            var data = _sTRPRCRepository.FilterByDate(startDateToDecimal/*, endDateToDecimal*/);
            return PartialView("~/Views/STRPRC/_ListByDatePartialView.cshtml", data);
        }

        [HttpPost]
        public ActionResult LoadData()
        {
            var rawData = _sTRPRCRepository.GetAll().AsQueryable();
            var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
            var start = Convert.ToInt32(HttpContext.Request.Form["start"].FirstOrDefault());
            var length = Convert.ToInt32(HttpContext.Request.Form["length"].FirstOrDefault());
            var searchValue = Convert.ToString(HttpContext.Request.Form["search[value]"].FirstOrDefault());

            var orderBy = Convert.ToString(HttpContext.Request.Form["columns[" + HttpContext.Request.Form["order[0][column]"].FirstOrDefault() + "][data]"].FirstOrDefault());
            var orderDir = Convert.ToString(HttpContext.Request.Form["order[0][dir]"].FirstOrDefault());

            //Sorting  
            if (!(string.IsNullOrEmpty(orderBy) && string.IsNullOrEmpty(orderDir)))
            {
                rawData = rawData.OrderBy(orderBy + " " + orderDir);
            }

            if (string.IsNullOrEmpty(searchValue))
            {
                searchValue = "230101";
            }
            //Search
            if (!string.IsNullOrEmpty(searchValue))
            {
                rawData = rawData.Where(c => c.O3SDT >= Convert.ToDecimal(searchValue));
            }
            

            //if shows all data
            if (length == -1)
                length = int.MaxValue;

            //Paging
            var data = rawData.Skip(start).Take(length).ToList();

            //Total number of filtered data
            var recordsTotal = rawData.Count();

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }
    }
}