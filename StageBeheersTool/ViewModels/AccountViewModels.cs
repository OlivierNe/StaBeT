using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using StageBeheersTool.Models.Authentication;
using StageBeheersTool.Models.Domain;

namespace StageBeheersTool.ViewModels
{

    public class AccountMetRolesVM
    {
        public string Id { get; set; }
        public string Login { get; set; }
        public bool Admin { get; set; }
        public bool Bedrijf { get; set; }
        public bool Student { get; set; }
        public bool Begeleider { get; set; }
    }

    public class AccountListVM
    {
        public ICollection<AccountMetRolesVM> Users { get; set; }
        public bool IsAdmin { get { return _isAdmin; } set { _isAdmin = value; } }
        private bool _isAdmin = true;

        public bool IsBedrijf { get { return _isBedrijf; } set { _isBedrijf = value; } }
        private bool _isBedrijf = true;

        public bool IsStudent { get { return _isStudent; } set { _isStudent = value; } }
        private bool _isStudent = true;

        public bool IsBegeleider { get { return _isBegeleider; } set { _isBegeleider = value; } }
        private bool _isBegeleider = true;

        public string LoginZoeken { get; set; }

        public bool ToonActies { get; set; }

        public void SetUsers(IEnumerable<UserMetRoles> users)
        {
            Users = new List<AccountMetRolesVM>();
            foreach (var user in users)
            {
                Users.Add(new AccountMetRolesVM()
                {
                    Id = user.Id,
                    Login = user.Login,
                    Admin = user.Roles.Contains(Role.Admin),
                    Bedrijf = user.Roles.Contains(Role.Bedrijf),
                    Begeleider = user.Roles.Contains(Role.Begeleider),
                    Student = user.Roles.Contains(Role.Student)
                });
            }
        }
    }

    public class AccountCreateVM : IValidatableObject
    {
        [Display(Name = "E-mail")]
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public bool Admin { get; set; }
        public bool Student { get; set; }
        public bool Begeleider { get; set; }
        public bool Bedrijf { get; set; }

        public string Id { get; set; }

        public string Title { get; set; }

        [DataType(DataType.Password)]
        public string Wachtwoord { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            if ((Student && Begeleider) || (Student && Admin))
            {
                results.Add(new ValidationResult("Ongeldige rollen combinatie: student kan geen begeleider of admin zijn."));
            }
            if ((Bedrijf && Admin) || (Bedrijf && Student) || (Bedrijf && Begeleider))
            {
                results.Add(new ValidationResult("Ongeldige rollen combinatie: accounts met de bedrijf rol kunnen geen student, begeleider of admin zijn."));
            }
            if (Bedrijf && string.IsNullOrWhiteSpace(Wachtwoord))
            {
                results.Add(new ValidationResult("Verplicht wachtwoord op te geven voor bedrijven."));
            }

            return results;
        }
    }

    public class ChangePasswordViewModel : IValidatableObject
    {
        //[Required]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Resources), Name = "DisplayOldPassword")]
        public string OldPassword { get; set; }

        [Required]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Resources), Name = "DisplayNewPassword")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Resources), Name = "DisplayNieuwWachtwoordBevestigen")]
        [Compare("NewPassword", ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorNieuwEnBevestigdWachtwoord")]
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
        [Display(ResourceType = typeof(Resources), Name = "DisplayWachtwoord")]
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
