﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Web;
using Microsoft.AspNet.Identity.Owin;

namespace StageBeheersTool.ViewModels
{
    public class IndexViewModel
    {
        public bool HasPassword { get; set; }
        public bool BrowserRemembered { get; set; }
    }

    public class ChangePasswordViewModel : IValidatableObject
    {
        //[Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public bool FirstLogin { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            var manager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            if (!FirstLogin && string.IsNullOrEmpty(OldPassword))
            {
                results.Add(new ValidationResult("Verplicht oud wachtwoord in te vullen."));
            }
            return results;
        }
    }


}