using StageBeheersTool.Models.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace StageBeheersTool.ViewModels
{

    public class StageopdrachtIndexVM
    {
        public IEnumerable<Stageopdracht> Stageopdrachten { get; set; }
        public bool ToonSearchForm { get; set; }
        public bool ToonZoekenOpStudent { get; set; }
        public bool ToonOordelen { get; set; }
        public bool ToonStudenten { get; set; }

        public string Title { get; set; }
        public string OverzichtAction { get; set; }
        public RouteValueDictionary ActionParams { get; set; }

        public void InitializeItems(IEnumerable<Stageopdracht> stageopdrachten)
        {
            ActionParams = new RouteValueDictionary();
            Stageopdrachten = stageopdrachten;
        }

        private SelectList _aantalStudentenList;
        private SelectList _semesterList;

        public SelectList AantalStudentenList
        {
            get
            {
                return _aantalStudentenList ??
                       (_aantalStudentenList = new SelectList(new[] { "1", "2", "3" },
                           AantalStudenten == null ? "" : AantalStudenten.ToString()));
            }
        }

        public SelectList SemesterList
        {
            get
            {
                return _semesterList ??
                    (_semesterList = new SelectList(new[] { "1", "2" }, Semester == null ? "" : Semester.ToString()));
            }
        }

        public int? Semester { get; set; }
        [Display(Name = "Studenten")]
        public int? AantalStudenten { get; set; }
        public string Locatie { get; set; }
        public string Bedrijf { get; set; }
        public string Student { get; set; }
        public string Specialisatie { get; set; }
        public string Academiejaar { get; set; }
    }

    public class StageopdrachtDetailsVM
    {
        public Stageopdracht Stageopdracht { get; set; }
        public string Semester
        {
            get
            {
                string semester = "";
                if (Stageopdracht.Semester1)
                {
                    semester += "1 " + StageperiodeSem1;
                }
                if (Stageopdracht.Semester1 && Stageopdracht.Semester2)
                {
                    semester += "\n";
                }
                if (Stageopdracht.Semester2)
                {
                    semester += "2 " + StageperiodeSem2;
                }

                return semester;
            }
        }
        public string StageperiodeSem1 { get; set; }
        public string StageperiodeSem2 { get; set; }
        public string EditDeadline { get; set; }

        public bool ToonToevoegen { get; set; }
        public bool ToonVerwijderenBtn { get; set; }
        public bool ToonEdit { get; set; }
        public bool ToonAanvraagIndienen { get; set; }
        public bool ToonAanvraagAnnuleren { get; set; }

        private string _overzichtAction = null;
        public string OverzichtAction { get { return _overzichtAction ?? "Index"; } set { _overzichtAction = value; } }

        public string Stagementor
        {
            get
            {
                if (Stageopdracht.Stagementor == null)
                {
                    return "";
                }
                return string.Join(" / ", 
                    new[] { Stageopdracht.Stagementor.Naam,
                        Stageopdracht.Stagementor.Email,
                        Stageopdracht.Stagementor.Gsmnummer }
                        .Where(s => !string.IsNullOrEmpty(s) ));
            }
        }
        public string Contractondertekenaar
        {
            get
            {
                if (Stageopdracht.Contractondertekenaar == null)
                {
                    return "";
                }
                return string.Join(" / ",
                    new[] { Stageopdracht.Contractondertekenaar.Naam,
                        Stageopdracht.Contractondertekenaar.Email,
                        Stageopdracht.Contractondertekenaar.Gsmnummer }
                        .Where(s => !string.IsNullOrEmpty(s)));
            }
        }

        public bool BedrijfHeeftGeldigEmail()
        {
            return new EmailAddressAttribute().IsValid(Stageopdracht.Bedrijf.Email);
        }

        public string Bedrijf
        {
            get
            {
                var retVal = Stageopdracht.Bedrijf.Naam;
                retVal += "\n" + Stageopdracht.Bedrijf.Adres;
                return retVal;
            }
        }
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
        public string Specialisatie { get; set; }
        [Display(Name = "Specialisatie")]
        public int? SpecialisatieId { get; set; }
        public bool Semester1 { get; set; }
        public bool Semester2 { get; set; }
        public string StageperiodeSem1 { get; set; }
        public string StageperiodeSem2 { get; set; }
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

        public void SetAdres(string gemeente, string postcode, string straat, string nummer)
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