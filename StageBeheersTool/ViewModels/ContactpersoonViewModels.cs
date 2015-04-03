using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using StageBeheersTool.Models.Domain;

namespace StageBeheersTool.ViewModels
{
    public class ContactpersoonIndexVM
    {
        public IEnumerable<Contactpersoon> Contactpersonen { get; set; }
        public bool ToonBedrijf { get; set; }
        public string Naam { get; set; }
        public string Bedrijf { get; set; }
    }

    public class ContactpersoonCreateVM
    {
        [Required]
        [Display(Name = "Naam")]
        public string Familienaam { get; set; }
        [Required]
        public string Voornaam { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Aanspreektitel { get; set; }
        public string Gsm { get; set; }
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

        public void SetBedrijven(IEnumerable<Bedrijf> bedrijven)
        {
            BedrijvenSelectList = new SelectList(bedrijven, "Id", "Naam", BedrijfId != 0 ? BedrijfId.ToString() : "");
        }
    }

    public class ContactpersoonEditVM : ContactpersoonCreateVM
    {
        public int Id { get; set; }
        public string Telefoonnummer { get; set; }
    }
}