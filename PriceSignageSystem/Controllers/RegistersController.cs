using PriceSignageSystem.Models;
using PriceSignageSystem.Models.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PriceSignageSystem.Controllers
{
    public class RegistersController : Controller
    {
        public readonly IRegistersRepository _repository;

        public RegistersController(IRegistersRepository repository)
        {
            _repository = repository;
        }

        // GET: Registers
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CountryIndex()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetCountries()
        {
            var countries = _repository.GetCountries();
            return Json(countries);
        }

        [HttpPost]
        public ActionResult UploadCountry(string countryName, HttpPostedFileBase fileInput)
        {
            try
            {
                if (fileInput != null && fileInput.ContentLength > 0)
                {
                    byte[] fileBytes;

                    using (var binaryReader = new BinaryReader(fileInput.InputStream))
                    {
                        fileBytes = binaryReader.ReadBytes(fileInput.ContentLength);
                    }

                    _repository.Upload(countryName, fileBytes);

                    return Json(new { success = true });
                }

                return Json(new { success = false, message = "No file uploaded." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error uploading file." });
            }
        }

        [HttpPost]
        public ActionResult DeleteCountry(string[] selectedRows)
        {
            _repository.DeleteSelectedCountry(selectedRows);
            return Json(new { success = true });
        }
    }
}