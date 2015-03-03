using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StageBeheersTool.ViewModels
{/*
    public class RegisterBedrijfViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-mail")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Bedrijfsnaam")]
        public string Naam { get; set; }
        [Required]
        public string Gemeente { get; set; }
        [Required]
        public int Postcode { get; set; }
        [Required]
        public string Straat { get; set; }
        [Required]
        [Display(Name="Nummer")]
        public int Straatnummer { get; set; }
        [Required]
        [Display(Name = "Telefoon/gsm")]
        public string Telefoonnummer { get; set; }
        public string Bereikbaarheid { get; set; } //(wagen – openbaar vervoer – georganiseerd vervoer door bedrijf) 
        public string BedrijfsActiviteiten { get; set; } //(bank – software ontwikkelaar – openbare diensten ….)
    }*/


    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
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
        [Display(Name = "Wachtwoord")]
        public string Password { get; set; }

        [Display(Name = "Onthouden")]
        public bool RememberMe { get; set; }
    }



    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
