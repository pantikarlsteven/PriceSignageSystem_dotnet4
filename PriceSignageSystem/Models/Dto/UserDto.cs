using PriceSignageSystem.Code.CustomValidations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PriceSignageSystem.Models.Dto
{
    public class UserDto
    {
        
        public int UserId { get; set; }
        [Display(Name = "Role")]
        [Required(ErrorMessage = "Role is required.")]
        public int RoleId { get; set; }
        [Display(Name = "Username")]
        [Required(ErrorMessage = "Username is required.")]
        [UniqueUsername(ErrorMessage = "Username must be unique.")]
        public string UserName { get; set; }
        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
        public int IsActive { get; set; }
        public List<SelectListItem> RoleList { get; set; }
        public string SelectedRoleId { get; set; }
    }
}