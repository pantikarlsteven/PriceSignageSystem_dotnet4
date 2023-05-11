using PriceSignageSystem.Helper;
using PriceSignageSystem.Models;
using PriceSignageSystem.Models.Constants;
using PriceSignageSystem.Models.Dto;
using PriceSignageSystem.Models.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PriceSignageSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly ISTRPRCRepository _sTRPRCRepository;

        public AccountController()
        {
        }

        public AccountController(IUserRepository userRepository, ISTRPRCRepository sTRPRCRepository)
        {
            _userRepository = userRepository;
            _sTRPRCRepository = sTRPRCRepository;
        }
        
        public ActionResult Login()
        {
            var storelist = _sTRPRCRepository.GetStores().Select(a => new SelectListItem
            {
                Value = a.O3LOC.ToString(),
                Text = a.O3LOC.ToString()
            }).ToList();

            var model = new UserStoreDto
            {
                StoreList = storelist,
                User = new User()
            };
            return View(model);
        }
        [HttpPost]
        public ActionResult Login(UserStoreDto model)
        {
            if (ModelState.IsValid)
            {
                var encryptedPassword = EncryptionHelper.Encrypt(model.Password);
                var result = _userRepository.GetUsers().Where(a => a.UserName == model.UserName && a.Password == encryptedPassword && a.IsActive == UserStatusConstants.Active).FirstOrDefault();
                if (result != null)
                {
                    return RedirectToAction("Index", "STRPRC", model);
                }
                else return View();
            }
            else
            {
                return View(model);
            }
        }
    }
}