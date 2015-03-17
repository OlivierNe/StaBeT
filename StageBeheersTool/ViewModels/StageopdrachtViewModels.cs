using StageBeheersTool.Models.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StageBeheersTool.ViewModels
{

    public class StageopdrachtIndexVM
    {
        public IEnumerable<Stageopdracht> Stageopdrachten { get; set; }
        public int? Semester { get; set; }
        [Display(Name = "Studenten")]
        public int? AantalStudenten { get; set; }
        public string Locatie { get; set; }
        public string Bedrijf { get; set; }
        public string Student { get; set; }
        [Display(Name = "Specialisatie")]
        public int? SpecialisatieId { get; set; }
        public SelectList AantalStudentenList { get; set; }
        public SelectList SemesterList { get; set; }
        public SelectList SpecialisatieList { get; set; }
        public bool ToonSearchForm { get; set; }
        public bool ToonZoekenOpStudent { get; set; }
        public bool ToonOordelen { get; set; }

        public StageopdrachtIndexVM()
        {
        }

        public void setItems(IEnumerable<Stageopdracht> stageopdrachten, IEnumerable<Specialisatie> specialisaties)
        {
            Stageopdrachten = stageopdrachten;
            SpecialisatieList = new SelectList(specialisaties, "Id", "Naam", SpecialisatieId == null ? "" : SpecialisatieId.ToString());
            AantalStudentenList = new SelectList(new string[] { "1", "2", "3" }, AantalStudenten == null ? "" : AantalStudenten.ToString());
            SemesterList = new SelectList(new string[] { "1", "2" }, Semester == null ? "" : Semester.ToString());
        }
    }

    public class StageopdrachtDetailsVM
    {
        public Stageopdracht Stageopdracht { get; set; }
        public bool ToonToevoegen { get; set; }
        public bool ToonVerwijderenBtn { get; set; }
        public bool ToonEdit { get; set; }
        public string Stagementor
        {
            get
            {
                if (Stageopdracht.Stagementor == null)
                {
                    return "/";
                }
                return Stageopdracht.Stagementor.Naam + " / " +
                    Stageopdracht.Stagementor.Email + " / " +
                    Stageopdracht.Stagementor.Gsmnummer;
            }
        }
        public string Contractondertekenaar
        {
            get
            {
                if (Stageopdracht.Contractondertekenaar == null)
                {
                    return "/";
                }
                return Stageopdracht.Contractondertekenaar.Naam + " / " + 
                    Stageopdracht.Contractondertekenaar.Email + " / " + 
                    Stageopdracht.Contractondertekenaar.Gsmnummer;
            }
        }
        public StageopdrachtDetailsVM()
        {
        }

        public bool ToonAanvraagIndienen { get; set; }
        public bool ToonAanvraagAnnuleren { get; set; }
    }

    public class StageopdrachtCreateVM : IValidatableObject
    {
        #region Properties
        [Required]
        [StringLength(200, ErrorMessage = "{0} mag niet langer zijn dan 200 characters.")]
        public string Titel { get; set; }
        public string Gemeente { get; set; }
        public string Postcode { get; set; }
        public string Straat { get; set; }
        [Display(Name = "nummer")]
        public string Straatnummer { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public string Omschrijving { get; set; }
        [Required]
        [RegularExpression("[0-9]{4}-[0-9]{4}", ErrorMessage = "Ongeldig academiejaar")]
        public string Academiejaar { get; set; }
        [Display(Name = "Specialisatie")]
        public int? SpecialisatieId { get; set; }
        public bool Semester1 { get; set; }
        public bool Semester2 { get; set; }
        [Range(1, 3)]
        [Display(Name = "Aantal Studenten")]
        public int AantalStudenten { get; set; }
        [Display(Name = "Contractondertekenaar")]
        public int? ContractondertekenaarId { get; set; }
        [Display(Name = "Stagementor")]
        public int? StagementorId { get; set; }
        public SelectList SpecialisatieSelectList { get; set; }
        public SelectList ContractondertekenaarsSelectList { get; set; }
        public SelectList StagementorsSelectList { get; set; }
        public SelectList AantalStudentenSelectList { get; set; }

        public ContactpersoonCreateVM Stagementor { get; set; }
        public ContactpersoonCreateVM Contractondertekenaar { get; set; }
        #endregion

        #region Constructors
        public StageopdrachtCreateVM(IEnumerable<Specialisatie> specialisaties,
          IEnumerable<Contactpersoon> contractondertekenaars, IEnumerable<Contactpersoon> stagementors)
            : this()
        {
            SetSelectLists(specialisaties, contractondertekenaars, stagementors);
        }
        public StageopdrachtCreateVM()
        {
            Academiejaar = DateTime.Now.Year + "-" + (DateTime.Now.Year + 1);
            Semester2 = true;
        }
        #endregion

        #region Public methods
        public void SetSelectLists(IEnumerable<Specialisatie> specialisaties,
            IEnumerable<Contactpersoon> contractondertekenaars, IEnumerable<Contactpersoon> stagementors)
        {
            SpecialisatieSelectList = new SelectList(specialisaties, "Id", "Naam", SpecialisatieId != 0 ? SpecialisatieId.ToString() : "");
            ContractondertekenaarsSelectList = new SelectList(contractondertekenaars, "Id", "Naam",
                ContractondertekenaarId != 0 ? ContractondertekenaarId.ToString() : "");
            StagementorsSelectList = new SelectList(stagementors, "Id", "Naam", StagementorId != 0 ? StagementorId.ToString() : "");
            var aantalStudentenOpties = new SelectListItem[] { new SelectListItem() { Value = "1", Text = "1"},
                     new SelectListItem() { Value = "2", Text = "2"}, new SelectListItem() { Value = "3", Text = "3" } };
            AantalStudentenSelectList = new SelectList(aantalStudentenOpties, "Value", "Text");
            this.AantalStudenten = 2;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();
            var beginJaar = int.Parse(Academiejaar.Substring(0, 4));
            var eindJaar = int.Parse(Academiejaar.Substring(5, 4));
            if (beginJaar != (eindJaar - 1))
            {
                errors.Add(new ValidationResult("Ongeldig Academiejaar"));
            }
            if (eindJaar < DateTime.Now.Year)
            {
                errors.Add(new ValidationResult("Academiejaar mag niet tot het verleden behoren."));
            }

            if (Semester1 == false && Semester2 == false)
            {
                errors.Add(new ValidationResult("Geen semester geselecteerd."));
            }

            return errors;
        }

        public void setAdres(string gemeente, string postcode, string straat, string nummer)
        {
            this.Gemeente = gemeente;
            this.Postcode = postcode;
            this.Straat = straat;
            this.Straatnummer = nummer;
        }
        #endregion

    }

    public class StageopdrachtEditVM : StageopdrachtCreateVM, IValidatableObject
    {
        public StageopdrachtEditVM()
        {
        }

        public StageopdrachtEditVM(IEnumerable<Specialisatie> specialisaties, IEnumerable<Contactpersoon> contractondertekenaars, IEnumerable<Contactpersoon> stagementors)
            : base(specialisaties, contractondertekenaars, stagementors)
        {
        }
        [HiddenInput]
        public int Id { get; set; }
        [Display(Name = "Aantal toegewezen studenten")]
        [Range(0, 3)]
        public int AantalToegewezenStudenten { get; set; }

        public new IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = base.Validate(validationContext).ToList();
            if (AantalToegewezenStudenten > AantalStudenten)
            {
                errors.Add(new ValidationResult("Aantal toegewezen studenten moet lager zijn dan het aantal studenten."));
            }
            return errors;
        }
    }

    public class StageopdrachtAfkeurenVM
    {
        public int Id { get; set; }
        public string Titel { get; set; }
        public string Aan { get; set; }
        [Required]
        public string Onderwerp { get; set; }
        [DataType(DataType.MultilineText)]
        [Required]
        public string Reden { get; set; }

    }
}