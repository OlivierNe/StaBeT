using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StageBeheersTool.ViewModels
{
    public class AcademiejaarInstellingenVM : IValidatableObject
    {
        [RegularExpression("[0-9]{4}-[0-9]{4}", ErrorMessageResourceType = typeof(Resources),
            ErrorMessageResourceName = "ErrorOngeldigAcademiejaarFormaat")]
        [Required]
        public string Academiejaar { get; set; }
        [UIHint("NullableDateTime")]
        [Display(Name = "DisplayBeginSem1", ResourceType = typeof(Resources))]
        public DateTime? Semester1Begin { get; set; }

        [Display(Name = "DisplayEindeSem1", ResourceType = typeof(Resources))]
        [UIHint("NullableDateTime")]
        public DateTime? Semester1Einde { get; set; }

        [Display(Name = "DisplayBeginSem2", ResourceType = typeof(Resources))]
        [UIHint("NullableDateTime")]
        public DateTime? Semester2Begin { get; set; }

        [Display(Name = "DisplayEindeSem2", ResourceType = typeof(Resources))]
        [UIHint("NullableDateTime")]
        public DateTime? Semester2Einde { get; set; }

        [Display(ResourceType = typeof(Resources), Name = "DisplayDeadlineBedrijfMagStageWijzigen")]
        [UIHint("NullableDateTime")]
        public DateTime? DeadlineBedrijfStageEdit { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();
            if (Semester1Begin != null && Semester1Einde != null && (DateTime.Compare((DateTime)Semester1Begin, (DateTime)Semester1Einde) > 0))
            {
                errors.Add(new ValidationResult(Resources.ErrorStageperiodeSem1));
            }
            if (Semester2Begin != null && Semester2Einde != null && (DateTime.Compare((DateTime)Semester2Begin, (DateTime)Semester2Einde) > 0))
            {
                errors.Add(new ValidationResult(Resources.ErrorStageperiodeSem2));
            }
            var beginJaar = int.Parse(Academiejaar.Substring(0, 4));
            var eindJaar = int.Parse(Academiejaar.Substring(5, 4));
            if (beginJaar != (eindJaar - 1))
            {
                errors.Add(new ValidationResult(Resources.ErrorOngeldigAcademiejaarFormaat));
            }
            if (beginJaar < (DateTime.Now.Year - 1))
            {
                errors.Add(new ValidationResult(Resources.ErrorAcademiejaarInVerleden));
            }
            return errors;
        }
    }

    public class InstellingenVM
    {
        [EmailAddress]
        [Display(Name = "Mailbox stages")]
        public string MailboxStages { get; set; }
        //...
    }
}