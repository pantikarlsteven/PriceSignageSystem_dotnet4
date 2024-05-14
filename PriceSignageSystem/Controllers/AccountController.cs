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
            DateTime currentTime = DateTime.Now;
            DateTime startTime = DateTime.Today.AddHours(4);
            DateTime endTime = DateTime.Today.AddHours(4).AddMinutes(15);

            if (currentTime >= startTime && currentTime < endTime)
            {
                return View("MaintenanceError");
            }

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

        public ActionResult Register_Old()
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
        public ActionResult Register_Old(UserDto user)
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

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(FormCollection form)
        {
            var empid = form["empid"];
            var username = form["username"];
            var pw = form["password"];
            var role = form["roleList"];
            var exceptionDetails = string.Empty;

            if (string.IsNullOrWhiteSpace(empid))
                exceptionDetails += "- Employee ID \n";
            if (string.IsNullOrWhiteSpace(username))
                exceptionDetails += "- Username \n";
            if (string.IsNullOrWhiteSpace(pw))
                exceptionDetails += "- Password \n";
            if(!string.IsNullOrEmpty(exceptionDetails))
                return Json(new { isSuccess = false, message = "Please enter following required fields:\n" , exceptionDetails });

            var existingUser = _userRepository.GetAll().FirstOrDefault(a => a.UserName == username);

            if(existingUser == null)
            {
                var encryptedPassword = EncryptionHelper.Encrypt(pw);
                var newUser = new User();
                newUser.EmployeeId = empid;
                newUser.UserName = username;
                newUser.Password = encryptedPassword;
                newUser.IsActive = UserStatusConstants.Active;
                newUser.RoleId = Convert.ToInt32(role);

                var data = _userRepository.AddUser(newUser);

                if(data != null)
                    return Json(new { isSuccess = true , message = "Registration Successful!" });
            }
            else
            {
                return Json(new { isSuccess = false, message = "Error!\n", exceptionDetails = "Username already exists!" });
            }

            return Json(null);
        }

        public ActionResult GetRoles()
        {
            var roles = _userRepository.GetRoles().Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = a.Name
            }).ToList();

            if (User.IsInRole("Manager"))
                roles.RemoveAt(0); // Administrator

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
    }
}