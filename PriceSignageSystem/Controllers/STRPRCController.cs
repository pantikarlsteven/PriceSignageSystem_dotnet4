using PriceSignageSystem.Helper;
using PriceSignageSystem.Models.Interface;
using System;
using System.Linq;
using System.Web.Mvc;


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

                if(dto != null)
                {
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

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDropdownsArray()
        {
            return data;
        }
    }
}