using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using StageBeheersTool.Helpers;
using StageBeheersTool.Models.Domain;

namespace StageBeheersTool.ViewModels
{
    public class BegeleiderListVM
    {
        public IEnumerable<Begeleider> Begeleiders { get; set; }

        public bool ToonActies { get; set; }

        public string Naam { get; set; }
        public string Voornaam { get; set; }
    }

    public class BegeleiderCreateVM
    {
        [EmailAddress]
        [Required]
        [Display(Name = "HoGent E-mail")]
        [HoGentPersoneelEmail]
        [StringLength(100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string HogentEmail { get; set; }
        [Display(Name = "Naam")]
        [StringLength(30, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string Familienaam { get; set; }
        [StringLength(20, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string Voornaam { get; set; }
        [Display(Name = "E-mail")]
        [EmailAddress]
        [StringLength(50, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string Email { get; set; }
        [StringLength(20, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string Gsm { get; set; }
        [StringLength(30, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string Gemeente { get; set; }
        [StringLength(15, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string Postcode { get; set; }
        [StringLength(50, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string Straat { get; set; }
        [Display(Name = "Ook een login account aanmaken?")]
        public bool LoginAccountAanmaken { get; set; }

        public string Overzicht { get; set; }
    }

    public class BegeleiderEditVM
    {
        public int Id { get; set; }
        [Display(Name = "Naam")]
        [StringLength(30, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string Familienaam { get; set; }
        [StringLength(20, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string Voornaam { get; set; }
        [Display(Name = "Hogent E-mail")]
        [HoGentPersoneelEmail]
        public string HogentEmail { get; set; }
        [Display(Name = "E-mail")]
        [EmailAddress]
        [StringLength(50, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string Email { get; set; }
        [StringLength(20, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string Gsm { get; set; }
        [StringLength(30, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string Gemeente { get; set; }
        [StringLength(15, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string Postcode { get; set; }
        [StringLength(50, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string Straat { get; set; }
        public Foto Foto { get; set; }
        public HttpPostedFileBase FotoFile { get; set; }

        public string Overzicht { get; set; }
    }

    public class BegeleiderDetailsVM
    {
        public int Id { get; set; }
        public string Naam { get; set; }
        [Display(Name = "HoGent E-mail")]
        public string HogentEmail { get; set; }
        [Display(Name = "E-mail")]
        public string Email { get; set; }
        public string Gemeente { get; set; }
        public string Postcode { get; set; }
        public string Straat { get; set; }
        public string Gsm { get; set; }
        public Foto Foto { get; set; }

        public bool ToonEdit { get; set; }
        public bool ToonTerugNaarLijst { get; set; }
        public bool ToonVerwijderen { get; set; }
      
    }

    public class BegeleiderJsonVM
    {
        public int Id { get; set; }
        public string Naam { get; set; }
        public string Email { get; set; }
    }
}