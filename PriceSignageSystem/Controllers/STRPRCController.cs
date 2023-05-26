using PriceSignageSystem.Helper;
using PriceSignageSystem.Models.Dto;
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

        public ActionResult Index(UserStoreDto model)
        {
            var date = _sTRPRCRepository.GetLatestUpdate();

            if (date.Date == DateTime.Now.Date)
            {
                ViewBag.IsDateLatest = true;
            }

            ViewBag.DateVersion = date.Date.ToShortDateString();
            return View(model);
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

        public ActionResult UpdateSTRPRCData(int storeId)
        {
            var count = _sTRPRCRepository.UpdateSTRPRCTable(storeId);
            var data = new UserStoreDto()
            {
                DataCount = count
            };
            return RedirectToAction("Index", data);
        }
    }
}