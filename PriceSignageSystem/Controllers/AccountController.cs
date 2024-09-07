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
                        TempData["ErrorMessage"] = "User is Inactive.";
                        ModelState.AddModelError("", "User is inactive.");
                        return View(model);
                    }
                    else if (String.IsNullOrEmpty(user.EmployeeId)) 
                    {
                        TempData["ErrorMessage"] = "Please update your Employee ID.";
                        return RedirectToAction("UpdateInfo", new { userName = user.UserName });

                    }
                    else
                    {
                        FormsAuthentication.SetAuthCookie(user.UserName, false);
                        return (returnUrl != null ? Redirect(returnUrl) : Redirect("/STRPRC/PCA"));
                    }
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

        [CustomAuthorize]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetUserInformation()
        {
            var users = new List<UserDto>();
            if (User.IsInRole("Administrator"))
                users = _userRepository.GetUsersInfo();
            
            else if (User.IsInRole("Manager"))
                users = _userRepository.GetUsersInfo().Where(a => a.RoleName != "Administrator").ToList();


            return Json(users);
        }

        [HttpPost]
        public ActionResult AddUser(UserDto newUser)
        {
            var result = _userRepository.AddUser(newUser);

            if (result > 0)
                return Json(new { success = true });
            else
                return Json(new { success = false });

        }

        public ActionResult GetRoleList()
        {
            var roles = _userRepository.GetRoles();

            if (User.IsInRole("Manager"))
                roles.RemoveAt(0); // remove Administrator

            return Json(roles, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchAccount(string username)
        {
            var result = _userRepository.SearchAccount(username);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateAccount(FormCollection form)
        {
            var empid = form["empId"];
            var username = form["userName"];
            var newpw = form["newPassword"];
            var role = Convert.ToInt32(form["role"]);
            var status = Convert.ToInt32(form["status"]);


            var result =_userRepository.UpdateAccount(empid, newpw, username, role, status);

            if(result == 1)
                return Json(new { isSuccess = true });
            else
                return Json(new { isSuccess = false });

        }

        public ActionResult UpdateInfo(string userName)
        {
            var model = new UserStoreDto();
            model.UserName = userName;
            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateInfoPost(UserStoreDto user)
        {
            if (!String.IsNullOrEmpty(user.EmployeeId))
            {
                var result = _userRepository.UpdateInfo(user);

                if (result == 1)
                {
                    FormsAuthentication.SetAuthCookie(user.UserName, false);
                    return Redirect("/STRPRC/PCA");
                }
            }
                
            return View();
        }

        [HttpPost]
        public ActionResult DeleteUserById(int id)
        {
            var result = _userRepository.DeleteUser(id);

            if (result == 1)
                return Json(new { success = true });
            else
                return Json(new { success = false });

        }

        [HttpPost]
        public ActionResult UpdateUserInfo(UserDto dto)
        {
            var result = _userRepository.UpdateUserInfo(dto);

            if (result == 1)
                return Json(new { success = true });
            else
                return Json(new { success = false });

        }

        public ActionResult GetStore()
        {
            var storeId = int.Parse(ConfigurationManager.AppSettings["StoreID"]);
            var store = _userRepository.GetStoreName(storeId);
            return Json(store.Name, JsonRequestBehavior.AllowGet);
        }
    }
}