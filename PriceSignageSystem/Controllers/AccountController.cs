using PriceSignageSystem.Code.CustomValidations;
using PriceSignageSystem.Helper;
using PriceSignageSystem.Models;
using PriceSignageSystem.Models.Constants;
using PriceSignageSystem.Models.Dto;
using PriceSignageSystem.Models.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace PriceSignageSystem.Controllers
{
    [CustomAuthorize]
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly ISTRPRCRepository _sTRPRCRepository;

        public AccountController(IUserRepository userRepository, ISTRPRCRepository sTRPRCRepository)
        {
            _userRepository = userRepository;
            _sTRPRCRepository = sTRPRCRepository;
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            var model = new UserStoreDto
            {
                User = new User()
            };
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(UserStoreDto model, string returnUrl)
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
                    FormsAuthentication.SetAuthCookie(user.UserName, false);
                    //Session["UserId"] = user.UserId;
                    //Session["Username"] = user.UserName;
                    //Session["RoleId"] = user.RoleId;
                    //return RedirectToAction("SearchByDate", "STRPRC");

                    return (returnUrl != null ? Redirect(returnUrl) : Redirect("/STRPRC/PCA"));
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
            //Session.Clear();
            //Session.Abandon();
            FormsAuthentication.SignOut();
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
            var user = new UserDto();
            user.RoleList = _userRepository.GetRoles().Select(a => new SelectListItem 
                        {
                            Value = a.Id.ToString(),
                            Text = a.Name.ToString()
                        }).ToList();

            return View(user);
        }

        [HttpPost]
        public ActionResult Register(UserDto user)
        {
            user.RoleList = _userRepository.GetRoles().Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = a.Name.ToString()
            }).ToList();

            if (ModelState.IsValid)
            {
                var existingUser = _userRepository.GetAll().FirstOrDefault(a => a.UserName == user.UserName);
               
                if (existingUser != null)
                {
                    ModelState.AddModelError("UserName", "Username must be unique.");
                    return View(user); // Return to the view with the validation error
                }

                var encryptedPassword = EncryptionHelper.Encrypt(user.Password);
                var newUser = new User();
                newUser.UserName = user.UserName;
                newUser.Password = encryptedPassword;
                newUser.IsActive = 1;
                newUser.RoleId = Convert.ToInt32(user.SelectedRoleId);

                //user.Password = encryptedPassword;
                //user.IsActive = 1;

                var data = _userRepository.AddUser(newUser);

                TempData["RegistrationSuccessMessage"] = "Registration successful!";
                return RedirectToAction("PCA", "STRPRC");
            }
            return View(user);
        }

        public ActionResult UpdatePassword(string username)
        {
            new UserDto();
            ViewBag.Username = username;
            return View();
        }

        [HttpPost]
        public ActionResult UpdatePassword(string username, string newPassword)
        {
            var result = _userRepository.UpdatePassword(username, newPassword);
           
            if(result == 1)
            {
                return Json(new { success = true });

            }

            return Json(new{success = false });
        }
    }
}