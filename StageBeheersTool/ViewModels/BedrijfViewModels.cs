using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using StageBeheersTool.Models.Domain;

namespace StageBeheersTool.ViewModels
{
    public class BedrijfListVM
    {
        public IEnumerable<Bedrijf> Bedrijven { get; set; }
       
    }

    public class BedrijfDetailsVM
    {
        public Bedrijf Bedrijf { get; set; }

        public bool ToonEdit { get; set; }
        public bool ToonChangePassword { get; set; }
        public bool ToonExtra { get; set; }
        public bool ToonTerug { get; set; }
    }

    public class RegisterBedrijfViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-mail")]
        [StringLength(50, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Bedrijfsnaam")]
        [StringLength(100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string Naam { get; set; }
        [Required]
        [StringLength(30, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string Gemeente { get; set; }
        [Required]
        [StringLength(15, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string Postcode { get; set; }
        [StringLength(50, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string Straat { get; set; }
        [Required]
        [StringLength(20, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        [Display(Name = "Telefoon/gsm")]
        public string Telefoon { get; set; }
        [Url]
        [StringLength(100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string Website { get; set; }
        [StringLength(100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string Bereikbaarheid { get; set; }
        [StringLength(200, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string Bedrijfsactiviteiten { get; set; }

        public string Overzicht { get; set; }
    }

    public class EditBedrijfVM
    {
        [HiddenInput]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Bedrijfsnaam")]
        [StringLength(100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string Naam { get; set; }
        [Required]
        [StringLength(30, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string Gemeente { get; set; }
        [Required]
        [StringLength(15, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string Postcode { get; set; }
        [StringLength(50, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string Straat { get; set; }
        [Required]
        [StringLength(20, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        [Display(Name = "Telefoon/gsm")]
        public string Telefoon { get; set; }
        [Url]
        [StringLength(100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string Website { get; set; }
        [StringLength(100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string Bereikbaarheid { get; set; }
        [StringLength(200, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string Bedrijfsactiviteiten { get; set; }

        public bool ToonTerug { get; set; }
        public string Overzicht { get; set; }
    }

    public class BedrijfJsonVM
    {
        public IEnumerable<ContactpersoonJsonVM> Stagementors { get; set; }
        public IEnumerable<ContactpersoonJsonVM> Contractondertekenaars { get; set; }
        public string Gemeente { get; set; }
        public string Straat { get; set; }
        public string Postcode { get; set; }
        public string Email { get; set; }
        public int Id { get; set; }
    }

}