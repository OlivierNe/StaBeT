using System.Web;
using StageBeheersTool.Models.Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace StageBeheersTool.ViewModels
{
    public class StudentListVM
    {
        public IEnumerable<Student> Studenten { get; set; }

        public bool ToonStage { get; set; }
        public bool ToonActies { get; set; }

        public string Naam { get; set; }
        public string Voornaam { get; set; }
    }

    public class StudentCreateVM
    {
        [EmailAddress]
        [Required]
        [Display(Name = "HoGent E-mail")]
        [StringLength(100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string HogentEmail { get; set; }
        [Display(Name = "Naam")]
        [StringLength(30, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string Familienaam { get; set; }
        [StringLength(20, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string Voornaam { get; set; }
        [Display(Name = "Keuzepakket")]
        public int? KeuzepakketId { get; set; }
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
        public SelectList KeuzevakSelectList { get; set; }
        [Display(Name = "Ook een login account aanmaken?")]
        public bool LoginAccountAanmaken { get; set; }

        public string Overzicht { get; set; }

        public void SetKeuzevakSelectList(IEnumerable<Keuzepakket> keuzepakketten)
        {
            KeuzevakSelectList = new SelectList(keuzepakketten, "Id", "Naam", KeuzepakketId != 0 ? KeuzepakketId.ToString() : "");
        }
    }

    public class StudentEditVM
    {
        public int Id { get; set; }
        [Display(Name = "Naam")]
        [StringLength(30, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string Familienaam { get; set; }
        [StringLength(20, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string Voornaam { get; set; }
        [Display(Name = "Keuzepakket")]
        public int? KeuzepakketId { get; set; }
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
        [StringLength(100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public Foto Foto { get; set; }
        public HttpPostedFileBase FotoFile  { get; set; }

        public SelectList KeuzevakSelectList { get; set; }

        public bool ToonTerug { get; set; }
        public string Overzicht { get; set; }

        public void SetKeuzevakSelectList(IEnumerable<Keuzepakket> keuzepakketten)
        {
            KeuzevakSelectList = new SelectList(keuzepakketten, "Id", "Naam", KeuzepakketId != 0 ? KeuzepakketId.ToString() : "");
        }
    }

    public class StudentDetailsVM
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
        public Keuzepakket Keuzepakket { get; set; }
        public string Gsm { get; set; }
        public Foto Foto { get; set; }

        public bool ToonDelete { get; set; }
        public bool ToonEdit { get; set; }
        public bool ToonTerugNaarLijst { get; set; }
    }

    public class StudentJsonVM
    {
        public int Id { get; set; }
        public string Naam { get; set; }
        public string Email { get; set; }
    }

}