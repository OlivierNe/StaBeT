using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using StageBeheersTool.Models.Domain;

namespace StageBeheersTool.ViewModels
{
    public class ContactpersoonListVM
    {
        public IEnumerable<Contactpersoon> Contactpersonen { get; set; }
        public bool ToonZoeken { get; set; }
        public bool ToonBedrijf { get; set; }

        public string Naam { get; set; }
        public string Bedrijf { get; set; }

    }

    public class ContactpersoonCreateVM
    {
        [Required]
        [Display(Name = "Naam")]
        [StringLength(30, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string Familienaam { get; set; }
        [Required]
        [StringLength(20, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string Voornaam { get; set; }
        [Required]
        [EmailAddress]
        [StringLength(50, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string Email { get; set; }
        [StringLength(20, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string Aanspreektitel { get; set; }
        [StringLength(20, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string Gsm { get; set; }
        [StringLength(50, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        [Display(Name = "Functie binnen het bedrijf")]
        public string Bedrijfsfunctie { get; set; }
        [Required]
        [Display(Name = "Stagementor")]
        public bool IsStagementor { get; set; }
        [Required]
        [Display(Name = "Contractondertekenaar")]
        public bool IsContractondertekenaar { get; set; }
        [Display(Name = "Bedrijf")]
        public int BedrijfId { get; set; }
        public SelectList BedrijvenSelectList { get; set; }
        public string Overzicht { get; set; }

        public void SetBedrijven(IEnumerable<Bedrijf> bedrijven)
        {
            BedrijvenSelectList = new SelectList(bedrijven, "Id", "Naam", BedrijfId != 0 ? BedrijfId.ToString() : "");
        }
    }

    public class ContactpersoonEditVM : ContactpersoonCreateVM
    {
        public int Id { get; set; }
        [StringLength(20, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string Telefoonnummer { get; set; }
    }

    public class ContactpersoonJsonVM
    {
        public string Naam { get; set; }
        public int Id { get; set; }
    }

}