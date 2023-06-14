using PriceSignageSystem.Models.DatabaseContext;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PriceSignageSystem.Code.CustomValidations
{
    public class UniqueUsernameAttribute : ValidationAttribute
    {
        private readonly ApplicationDbContext _db;
        public UniqueUsernameAttribute()
        {
            _db = new ApplicationDbContext();
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.Success; // Return success if username is null or blank
            }

            // Access the database to check if the username is unique
            var existingUser = _db.Users.FirstOrDefault(u => u.UserName == (string)value);

            if (existingUser != null)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}