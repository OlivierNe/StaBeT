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
        [StringLength(200, ErrorMessage = "{0} mag niet langer zijn dan 200 characters.")]
        public string Omschrijving { get; set; }
        [Required]
        [RegularExpression("[0-9]{4}-[0-9]{4}", ErrorMessage = "Ongeldig academiejaar")]
        public string Academiejaar { get; set; }
        [Required]
        [Display(Name = "Specialisatie")]
        public int SpecialisatieId { get; set; }
        public SelectList SpecialisatieSelectList { get; set; }
        [Required]
        public int Semester { get; set; }
        [Required]
        [Range(1, 3)]
        public int AantalStudenten { get; set; }

        public StageopdrachtCreateVM(IEnumerable<Specialisatie> specialisaties)
        {
            SetSelectList(specialisaties);
        }
        public StageopdrachtCreateVM()
        {

        }
        //TODO
        //public StagecontractOndertekenaar ContractOndertekenaar { get; set; }
        //public Stagementor Stagementor { get; set; }
        public void SetSelectList(IEnumerable<Specialisatie> specialisaties)
        {
            SpecialisatieSelectList = new SelectList(specialisaties, "Id", "Naam", SpecialisatieId != 0 ? SpecialisatieId.ToString() : "");
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

    public class StageopdrachtEditVM : StageopdrachtCreateVM
    {
        public StageopdrachtEditVM()
        {
        }

        public StageopdrachtEditVM(IQueryable<Specialisatie> specialisaties) : base(specialisaties)
        {
        }
        public int Id { get; set; }
    } 

}