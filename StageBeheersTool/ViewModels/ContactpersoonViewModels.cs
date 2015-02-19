﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StageBeheersTool.ViewModels
{
    /**
     *  naam - voornaam - e-mail - gsm - functie binnen het bedrijf - aanspreektitel - functie stageopdracht (mentor-contractondertekenaar.
     * */
    public class ContactpersoonCreateVM
    {
        [Required]
        [Display(Name = "Naam")]
        public string Familienaam { get; set; }
        [Required]
        public string Voornaam { get; set; }
        [Required]
        public string Aanspreektitel { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Gsmnummer { get; set; }
        [Display(Name = "Functie binnen het bedrijf")]
        [Required]
        public string Bedrijfsfunctie { get; set; }
        [Required]
        [Display(Name = "Stagementor")]
        public bool IsStagementor { get; set; }
        [Required]
        [Display(Name = "Contractondertekenaar")]
        public bool IsContractOndertekenaar { get; set; }
    }

    public class ContactpersoonEditVM : ContactpersoonCreateVM
    {
        public int Id { get; set; }
        public string Telefoonnummer { get; set; }
    }

    public class AdresVM
    {
        [Required]
        public string Gemeente { get; set; }
        [Required]
        public int Postcode { get; set; }
        [Required]
        public string Straat { get; set; }
        [Required]
        public int Straatnummer { get; set; }
    }
}