using PriceSignageSystem.Helper;
using PriceSignageSystem.Models;
using PriceSignageSystem.Models.Constants;
using PriceSignageSystem.Models.Dto;
using PriceSignageSystem.Models.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var storelist = GetStoreList();

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
                var user = _userRepository.GetUsers().FirstOrDefault(a => a.UserName == model.UserName && a.Password == encryptedPassword);

                if (user != null)
                {
                    if(user.IsActive == UserStatusConstants.Inactive)
                    {
                        ModelState.AddModelError("", "User is inactive.");

                        model.StoreList = GetStoreList();
                        return View(model);
                    }
                    return RedirectToAction("Index", "STRPRC", model);
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }
            //else
            //{
            //    if (String.IsNullOrEmpty(model.UserName))
            //    {
            //        ModelState.AddModelError("", "Invalid username or password.");
            //    }
            //}

            model.StoreList = GetStoreList();
            return View(model);
        }

        public List<SelectListItem> GetStoreList()
        {
            var list = _sTRPRCRepository.GetStores().Select(a => new SelectListItem
                        {
                            Value = a.O3LOC.ToString(),
                            Text = a.O3LOC.ToString()
                        }).ToList();
            return list;
        }
    }
}