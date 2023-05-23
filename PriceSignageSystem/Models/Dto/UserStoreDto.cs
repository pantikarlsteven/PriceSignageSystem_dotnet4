using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PriceSignageSystem.Models.Dto
{
    public class UserStoreDto
    {
        public int UserId { get; set; }
        [Required(ErrorMessage = "Username is required")]
        [Display(Name = "Username")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [Display(Name = "Password")]
        public string Password { get; set; }
        public int IsActive { get; set; }
        [Required(ErrorMessage = "Store ID is required")]
        [Display(Name = "Store ID")]
        public int SelectedStore { get; set; }
        public List<SelectListItem> StoreList { get; set; }
        public User User { get; set; }
        public int DataCount { get; set; }
    }
}