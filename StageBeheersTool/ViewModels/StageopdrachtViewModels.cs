using StageBeheersTool.Helpers;
using StageBeheersTool.Models.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace StageBeheersTool.ViewModels
{
    public class StageopdrachtListVM
    {
        public IEnumerable<Stageopdracht> Stageopdrachten { get; set; }
        public bool ToonZoeken { get; set; }
        public bool ToonZoekenOpStudent { get; set; }
        public bool ToonOordelen { get; set; }
        public bool ToonStudenten { get; set; }
        public bool ToonStatus { get; set; }
        public bool ToonBedrijf { get; set; }
        public bool ToonAantalStudenten { get; set; }
        public bool ToonCreateNew { get; set; }
        public bool ToonSemester { get; set; }
        public bool ToonBegeleider { get; set; }
        public bool ToonAcademiejaar { get; set; }
        public bool ToonDossierIndienen { get; set; }

        public int? StageIdDossierIngediend { get; set; }
        public StagedossierStatus? CurrentStudentStagedossierStatus { get; set; }

        public string Title { get; set; }
        public string OverzichtAction { get; set; }

        private SelectList _academiejaarList;

        public SelectList AantalStudentenList
        {
            get
            {
                return new SelectList(new[] { "1", "2", "3" }, AantalStudenten == null ? "" : AantalStudenten.ToString());
            }
        }

        public SelectList SemesterList
        {
            get
            {
                return new SelectList(new[] { "1", "2" }, Semester == null ? "" : Semester.ToString());
            }
        }

        public SelectList AcademiejaarSelectList
        {
            get
            {
                return _academiejaarList ??
                    (_academiejaarList = new SelectList(Academiejaren, Academiejaar ?? ""));
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
        public string[] Academiejaren { get; set; }
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

        public bool ToonVoorkeurToevoegen { get; set; }
        public bool ToonVoorkeurVerwijderen { get; set; }
        public bool ToonEdit { get; set; }
        public bool ToonVerwijderen { get; set; }
        public bool ToonBedrijfeditDeadline { get; set; }
        public bool ToonAanvraagIndienen { get; set; }
        public bool ToonAanvraagAnnuleren { get; set; }
        public bool ToonOordelen { get; set; }
        public bool ToonStudenten { get; set; }
        public bool ToonStatus { get; set; }

        private string _overzichtAction;
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
                        Stageopdracht.Stagementor.Gsm }
                        .Where(s => !string.IsNullOrEmpty(s)));
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
                        Stageopdracht.Contractondertekenaar.Gsm }
                        .Where(s => !string.IsNullOrEmpty(s)));
            }
        }

        public bool BedrijfHeeftGeldigEmail()
        {
            return new EmailAddressAttribute().IsValid(Stageopdracht.Bedrijf.Email);
        }
    }

    public class StageopdrachtCreateVM : IValidatableObject
    {
        #region Properties
        [Required]
        [StringLength(250, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string Titel { get; set; }
        [StringLength(30, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string Gemeente { get; set; }
        [StringLength(15, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string Postcode { get; set; }
        [StringLength(50, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
        public string Straat { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public string Omschrijving { get; set; }
        [Required]
        [RegularExpression("[0-9]{4}-[0-9]{4}", ErrorMessageResourceType = typeof(Resources),
           ErrorMessageResourceName = "ErrorOngeldigAcademiejaarFormaat")]
        public string Academiejaar { get; set; }
        [StringLength(50, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "ErrorVeldlengte")]
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
        [Display(Name = "Bedrijf")]
        public int? BedrijfId { get; set; }

        public SelectList SpecialisatieSelectList { get; set; }
        public SelectList ContractondertekenaarsSelectList { get; set; }
        public SelectList StagementorsSelectList { get; set; }
        public SelectList AantalStudentenSelectList { get; set; }
        public SelectList BedrijvenSelectList { get; set; }

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

        public StageopdrachtCreateVM(IEnumerable<Bedrijf> bedrijven, IEnumerable<Specialisatie> specialisaties)
            : this()
        {
            SetBedrijfSelectList(bedrijven, specialisaties);
        }

        public void SetBedrijfSelectList(IEnumerable<Bedrijf> bedrijven, IEnumerable<Specialisatie> specialisaties)
        {
            BedrijvenSelectList = new SelectList(bedrijven, "Id", "Naam", BedrijfId != null ? BedrijfId.ToString() : "");
            SetSelectLists(specialisaties, new List<Contactpersoon>(), new List<Contactpersoon>());
        }

        public StageopdrachtCreateVM()
        {
            Academiejaar = AcademiejaarHelper.HuidigAcademiejaar();
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

        public void SetAdres(string gemeente, string postcode, string straat)
        {
            this.Gemeente = gemeente;
            this.Postcode = postcode;
            this.Straat = straat;
        }

        #endregion
    }

    public class StageopdrachtEditVM : StageopdrachtCreateVM
    {
        [HiddenInput]
        [Required]
        public int Id { get; set; }

        public StageopdrachtEditVM()
        {
        }

        public StageopdrachtEditVM(IEnumerable<Specialisatie> specialisaties, IEnumerable<Contactpersoon> contractondertekenaars, IEnumerable<Contactpersoon> stagementors)
            : base(specialisaties, contractondertekenaars, stagementors)
        {
        }

    }

    public class StageopdrachtAfkeurenVM
    {
        public int Id { get; set; }
        public string Titel { get; set; }
        [Required]
        public string Aan { get; set; }
        [Required]
        public string Onderwerp { get; set; }
        [DataType(DataType.MultilineText)]
        [Required]
        public string Reden { get; set; }
    }

    public class StageopdrachtLijstExcelVM
    {
        [Display(Name = "Stagebegeleider")]
        public int? SelectedStagebegeleiderId { get; set; }
        [Display(Name = "Academiejaar")]
        public string SelectedAcademiejaar { get; set; }
        [Display(Name = "Status stage")]
        public int? SelectedStatus { get; set; }

        public SelectList StagebegeleiderSelectList { get; set; }
        public SelectList AcademiejaarSelectList { get; set; }
        public SelectList StatusSelectList { get; set; }
        //...

        [Display(Name = "Tabblad naam")]
        [Required]
        public string TabbladNaam { get; set; }
        [Required]
        public string Bestandsnaam { get; set; }

        public bool Bedrijfsnaam { get; set; }
        public bool Bedrijfsadres { get; set; }
        //andere bedrijfsgegevens

        public bool Stageplaats { get; set; }
        public bool Titel { get; set; }
        public bool Omschrijving { get; set; }
        public bool Studenten { get; set; }
        public bool Begeleider { get; set; }
        public bool Status { get; set; }
        //...

        public StageopdrachtLijstExcelVM()
        {
            TabbladNaam = "Stages";
            Bestandsnaam = "Stages";
        }

        public void InitSelectLists(IEnumerable<Begeleider> stagebegeleiders, string[] academiejaren)
        {
            StagebegeleiderSelectList = new SelectList(stagebegeleiders, "Id", "Naam", SelectedStagebegeleiderId != null ? SelectedStagebegeleiderId.ToString() : "");
            AcademiejaarSelectList = new SelectList(academiejaren);
            var statusOpties = new SelectListItem[] { new SelectListItem { Value = ((int)StageopdrachtStatus.NietBeoordeeld).ToString(), Text = "Niet beoordeeld"},
                     new SelectListItem { Value = ((int)StageopdrachtStatus.Goedgekeurd).ToString(), Text = "Goedgekeurd"}, 
                     new SelectListItem {Value = ((int)StageopdrachtStatus.Afgekeurd).ToString(), Text = "Afgekeurd"}};
            StatusSelectList = new SelectList(statusOpties, "Value", "Text", SelectedStatus != null ? ((StageopdrachtStatus)SelectedStatus).ToString() : "");
        }

        public List<String> GetHeaders()
        {
            var headers = new List<String>();
            if (Titel)
            {
                headers.Add("Titel");
            }
            if (Omschrijving)
            {
                headers.Add("Omschrijving");
            }
            if (Stageplaats)
            {
                headers.Add("Stageplaats");
            }
            if (Bedrijfsnaam)
            {
                headers.Add("Bedrijfsnaam");
            }
            if (Bedrijfsadres)
            {
                headers.Add("Bedrijfsadres");
            }
            if (Begeleider)
            {
                headers.Add("Stagebegeleider");
            }
            if (Studenten)
            {
                headers.Add("Studenten");
            }
            if (Status)
            {
                headers.Add("Status");
            }
            return headers;
        }
    }

    public class StageAanStudentToewijzenVM : IValidatableObject
    {
        public int StageopdrachtId { get; set; }
        public int StudentId { get; set; }
        public Stageopdracht Stageopdracht { get; set; }
        public Student Student { get; set; }

        [UIHint("NullableDateTime")]
        public DateTime? Begindatum { get; set; }
        [UIHint("NullableDateTime")]
        public DateTime? Einddatum { get; set; }
        [Range(1, 2)]
        public int Semester { get; set; }

        public SelectList SemesterSelectList
        {
            get
            {
                return new SelectList(new[] { "1", "2" }, Semester == 0 ? 2 : Semester);
            }
        }

        public bool AangepasteStageperiode { get; set; }

        [UIHint("NullableDateTime")]
        public DateTime? Semester1Begin { get; set; }
        [UIHint("NullableDateTime")]
        public DateTime? Semester1Einde { get; set; }
        [UIHint("NullableDateTime")]
        public DateTime? Semester2Begin { get; set; }
        [UIHint("NullableDateTime")]
        public DateTime? Semester2Einde { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();
            if (AangepasteStageperiode && (Einddatum == null || Begindatum == null))
            {
                errors.Add(new ValidationResult(Resources.ErrorVerplichtBeginEnEindDatum));
            }
            return errors;
        }
    }
}
