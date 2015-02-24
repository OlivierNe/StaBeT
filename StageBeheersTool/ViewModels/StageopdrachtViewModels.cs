using StageBeheersTool.Models.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StageBeheersTool.ViewModels
{
    public class StageopdrachtCreateVM : IValidatableObject
    {
        [Required]
        [StringLength(200, ErrorMessage = "{0} mag niet langer zijn dan 200 characters.")]
        public string Titel { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public string Omschrijving { get; set; }
        [Required]
        [RegularExpression("[0-9]{4}-[0-9]{4}", ErrorMessage = "Ongeldig academiejaar")]
        public string Academiejaar { get; set; }
        [Display(Name = "Specialisatie")]
        public int? SpecialisatieId { get; set; }
        [Required]
        public int Semester { get; set; }
        [Range(1, 3)]
        [Display(Name = "Aantal Studenten")]
        public int AantalStudenten { get; set; }
        [Display(Name = "Contractondertekenaar")]
        public int? ContractOndertekenaarId { get; set; }
        [Display(Name = "Stagementor")]
        public int? StagementorId { get; set; }
        public SelectList SpecialisatieSelectList { get; set; }
        public SelectList ContractOndertekenaarsSelectList { get; set; }
        public SelectList StagementorsSelectList { get; set; }

        public StageopdrachtCreateVM(IEnumerable<Specialisatie> specialisaties,
            IEnumerable<Contactpersoon> contractOndertekenaars, IEnumerable<Contactpersoon> stagementors)
            : this()
        {
            SetSelectLists(specialisaties, contractOndertekenaars, stagementors);
        }
        public StageopdrachtCreateVM()
        {
            Academiejaar = DateTime.Now.Year + "-" + (DateTime.Now.Year + 1);
            Semester = 2;
        }
        public void SetSelectLists(IEnumerable<Specialisatie> specialisaties,
            IEnumerable<Contactpersoon> contractOndertekenaars, IEnumerable<Contactpersoon> stagementors)
        {
            SpecialisatieSelectList = new SelectList(specialisaties, "Id", "Naam", SpecialisatieId != 0 ? SpecialisatieId.ToString() : "");
            ContractOndertekenaarsSelectList = new SelectList(contractOndertekenaars, "Id", "Naam", ContractOndertekenaarId != 0 ? ContractOndertekenaarId.ToString() : "");
            StagementorsSelectList = new SelectList(stagementors, "Id", "Naam", StagementorId != 0 ? StagementorId.ToString() : "");
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

            return errors;
        }

    }

    public class StageopdrachtEditVM : StageopdrachtCreateVM, IValidatableObject
    {
        public StageopdrachtEditVM()
        {
        }

        public StageopdrachtEditVM(IEnumerable<Specialisatie> specialisaties, IEnumerable<Contactpersoon> contractOndertekenaars, IEnumerable<Contactpersoon> stagementors)
            : base(specialisaties, contractOndertekenaars, stagementors)
        {
        }
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

}