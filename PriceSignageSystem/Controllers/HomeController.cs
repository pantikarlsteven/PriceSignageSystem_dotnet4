using PriceSignageSystem.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PriceSignageSystem.Controllers
{
    [SessionExpiration]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SessionExpired()
        {
            TempData["TimeoutErrorMessage"] = "Your session has expired. Please login again.";
            return RedirectToAction("Login", "Account");
        }
    }
}