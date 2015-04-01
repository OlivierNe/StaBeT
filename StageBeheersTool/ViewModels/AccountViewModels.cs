using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StageBeheersTool.ViewModels
{

    public class ChangePasswordViewModel : IValidatableObject
    {
        //[Required]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof (Resources), Name = "DisplayOldPassword")]
        public string OldPassword { get; set; }

        [Required]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof (Resources), Name = "DisplayNewPassword")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(ResourceType = typeof (Resources), Name = "DisplayNieuwWachtwoordBevestigen")]
        [Compare("NewPassword", ErrorMessageResourceType = typeof (Resources), ErrorMessageResourceName = "ErrorNieuwEnBevestigdWachtwoord")]
        public string ConfirmPassword { get; set; }

        public bool FirstLogin { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            if (!FirstLogin && string.IsNullOrEmpty(OldPassword))
            {
                results.Add(new ValidationResult(Resources.ErrorOudWachtwoordVerplicht));
            }
            return results;
        }
    }
    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "E-mail")]
        [EmailAddress]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "E-mail")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof (Resources), Name = "DisplayWachtwoord")]
        public string Password { get; set; }

        [Display(Name = "Onthouden")]
        public bool RememberMe { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Required]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Wachtwoord")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Resources), Name = "DisplayNieuwWachtwoordBevestigen")]
        [Compare("Password", ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorNieuwEnBevestigdWachtwoord")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-mail")]
        public string Email { get; set; }
    }
}
