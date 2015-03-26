﻿using System.ComponentModel.DataAnnotations;

namespace StageBeheersTool.ViewModels
{
    public class BegeleiderEditVM
    {
        public int Id { get; set; }
        [Display(Name = "Naam")]
        public string Familienaam { get; set; }
        public string Voornaam { get; set; }
        [Display(Name = "Hogent E-mail")]
        public string HogentEmail { get; set; }
        [Display(Name = "E-mail")]
        [EmailAddress]
        public string Email { get; set; }
        [Display(Name = "gsm")]
        public string Gsmnummer { get; set; }
        public string Gemeente { get; set; }
        public string Postcode { get; set; }
        public string Straat { get; set; }
        [Display(Name = "Nummer")]
        public string Straatnummer { get; set; }
        public string FotoUrl { get; set; }
    }


    public class BegeleiderDetailsVM
    {
        public string Naam { get; set; }
        [Display(Name = "HoGent E-mail")]
        public string HogentEmail { get; set; }
        [Display(Name = "E-mail")]
        public string Email { get; set; }
        public string Gemeente { get; set; }
        public string Postcode { get; set; }
        public string Straat { get; set; }
        public string Straatnummer { get; set; }
        public string Gsmnummer { get; set; }
        public string FotoUrl { get; set; }
        public bool ToonEdit { get; set; }
    }
}