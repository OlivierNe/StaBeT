using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StageBeheersTool.ViewModels
{
    public class StudentEditVM
    {
        //naam voornaam - adres - tweede e-mail (niet hogent) -  gsm -
        //keuzepakket (e-commerce-mobile-netwerken-mainframe). Een foto uploaden of  wijzigen moet mogelijk zijn.
        public int Id { get; set; }
        [Display(Name = "Naam")]
        public string Familienaam { get; set; }
        public string Voornaam { get; set; }
        [Display(Name = "Hogent E-mail")]
        public string HogentEmail { get; set; }
        public string Keuzevak { get; set; }
        [Display(Name = "E-mail")]
        [EmailAddress]
        public string Email { get; set; }
        [Display(Name="gsm")]
        public string Gsmnummer { get; set; }
        public string Gemeente { get; set; }
        public string Postcode { get; set; }
        public string Straat { get; set; }
        [Display(Name="Nummer")]
        public string Straatnummer { get; set; }
        public string FotoUrl { get; set; }

    }
}