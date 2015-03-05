using StageBeheersTool.Models.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StageBeheersTool.ViewModels
{
    public class StudentEditVM
    {
        public int Id { get; set; }
        [Display(Name = "Naam")]
        public string Familienaam { get; set; }
        public string Voornaam { get; set; }
        [Display(Name = "Keuzepakket")]
        public int? KeuzepakketId { get; set; }
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
        public SelectList KeuzevakSelectList { get; set; }

        public void InitSelectList(IEnumerable<Keuzepakket> keuzepakketten)
        {
            KeuzevakSelectList = new SelectList(keuzepakketten, "Id", "Naam", KeuzepakketId != 0 ? KeuzepakketId.ToString() : "");
        }
    }
}