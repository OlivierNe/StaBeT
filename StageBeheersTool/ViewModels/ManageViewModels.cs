using System.Collections.Generic;
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
        [Display(Name = "Huidig wachtwoord")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nieuw wachtwoord")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Nieuw wachtwoord bevestigen")]
        [Compare("NewPassword", ErrorMessage = "Het nieuwe wachtwoord en het bevestigde antwoord komen niet overeen.")]
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