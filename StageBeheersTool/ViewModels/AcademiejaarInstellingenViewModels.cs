using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StageBeheersTool.ViewModels
{
    public class AcademiejaarInstellingenVM : IValidatableObject
    {
        public string Academiejaar { get; set; }
        [UIHint("NullableDateTime")]
        [Display(Name = "Begin Semester 1")]
        public DateTime? Semester1Begin { get; set; }

        [Display(Name = "Einde Semester 1")]
        [UIHint("NullableDateTime")]
        public DateTime? Semester1Einde { get; set; }

        [Display(Name = "Begin Semester 2")]
        [UIHint("NullableDateTime")]
        public DateTime? Semester2Begin { get; set; }

        [Display(Name = "Einde Semester 1")]
        [UIHint("NullableDateTime")]
        public DateTime? Semester2Einde { get; set; }

        [Display(Name = "Deadline voor bedrijven om stageopdrachten te wijzigen")]
        [UIHint("NullableDateTime")]
        public DateTime? DeadlineBedrijfStageEdit { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();
            if (Semester1Begin != null && Semester1Einde != null && (DateTime.Compare((DateTime)Semester1Begin, (DateTime)Semester1Einde) > 0))
            {
                errors.Add(new ValidationResult("Foute stageperiode voor semester 1: begindatum moet na de einddatum liggen"));
            }
            if (Semester2Begin != null && Semester2Einde != null && (DateTime.Compare((DateTime)Semester2Begin, (DateTime)Semester2Einde) > 0))
            {
                errors.Add(new ValidationResult("Foute stageperiode voor semester 2: begindatum moet na de einddatum liggen"));
            }
            var beginJaar = int.Parse(Academiejaar.Substring(0, 4));
            var eindJaar = int.Parse(Academiejaar.Substring(5, 4));
            if (beginJaar != (eindJaar - 1))
            {
                errors.Add(new ValidationResult("Ongeldig Academiejaar(beginjaar-eindjaar)"));
            }
            if (beginJaar < (DateTime.Now.Year-1))
            {
                errors.Add(new ValidationResult("Academiejaar mag niet tot het verleden behoren."));
            }
            return errors;
        }
    }
}