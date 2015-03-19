using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StageBeheersTool.ViewModels
{
    public class RegisterBedrijfViewModel : EditBedrijfVM
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

    }

    public class EditBedrijfVM
    {
        [HiddenInput]
        public int Id { get; set; }
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
        [Display(Name = "Nummer")]
        public int Straatnummer { get; set; }
        [Required]
        [Display(Name = "Telefoon/gsm")]
        public string Telefoonnummer { get; set; }
        public string Bereikbaarheid { get; set; }
        public string Bedrijfsactiviteiten { get; set; }
    }
}