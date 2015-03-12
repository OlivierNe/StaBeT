using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using StageBeheersTool.Models.Domain;

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
        public Begeleider Begeleider { get; set; }
        public bool ToonEdit { get; set; }
    }
}