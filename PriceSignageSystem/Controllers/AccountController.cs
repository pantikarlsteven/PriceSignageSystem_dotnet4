using PriceSignageSystem.Helper;
using PriceSignageSystem.Models;
using PriceSignageSystem.Models.Constants;
using PriceSignageSystem.Models.Dto;
using PriceSignageSystem.Models.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace PriceSignageSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly ISTRPRCRepository _sTRPRCRepository;

        public AccountController(IUserRepository userRepository, ISTRPRCRepository sTRPRCRepository)
        {
            _userRepository = userRepository;
            _sTRPRCRepository = sTRPRCRepository;
        }

        public ActionResult Login()
        {
            var model = new UserStoreDto
            {
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
                        return View(model);
                    }
                    Session["UserId"] = user.UserId;
                    Session["Username"] = user.UserName;
                    return RedirectToAction("SearchByDate", "STRPRC");
                }
                else
                {
                    TempData["ErrorMessage"] = "Invalid Username or Password!";
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }
            
            return View(model);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();

            return RedirectToAction("Login", "Account");
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

        public ActionResult Register()
        {
            var user = new User();
            return View(user);
        }

        [HttpPost]
        public ActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                var encryptedPassword = EncryptionHelper.Encrypt(user.Password);
                user.Password = encryptedPassword;
                user.IsActive = 1;
                user.RoleId = 2;
                var data = _userRepository.AddUser(user);

                TempData["SuccessMessage"] = "Registration successful!";
                return RedirectToAction("Login");
            }
            return View();
        }
    }
}