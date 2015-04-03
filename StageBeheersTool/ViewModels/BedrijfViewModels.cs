using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using StageBeheersTool.Models.Domain;

namespace StageBeheersTool.ViewModels
{
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
        public string Postcode { get; set; }
        public string Straat { get; set; }
        [Required]
        [Display(Name = "Telefoon/gsm")]
        public string Telefoon { get; set; }
        [Url]
        public string Website { get; set; }
        public string Bereikbaarheid { get; set; }
        public string Bedrijfsactiviteiten { get; set; }
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
        public string Postcode { get; set; }
        public string Straat { get; set; }
        [Required]
        [Display(Name = "Telefoon/gsm")]
        public string Telefoon { get; set; }
        [Url]
        public string Website { get; set; }
        public string Bereikbaarheid { get; set; }
        public string Bedrijfsactiviteiten { get; set; }
    }

    public class BedrijfInfoVM
    {
        public IEnumerable<Contactpersoon> Stagementors { get; set; }
        public IEnumerable<Contactpersoon> Contractondertekenaars { get; set; }
        public string Gemeente { get; set; }
        public string Straat { get; set; }
        public string Postcode { get; set; }
    }

}