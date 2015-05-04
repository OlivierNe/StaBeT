using System.ComponentModel.DataAnnotations;

namespace StageBeheersTool.Helpers
{
    public class HoGentStudentEmailAttribute : ValidationAttribute
    {
        public HoGentStudentEmailAttribute()
        {
            ErrorMessage = "Ongeldig HoGent student E-mailadres";
        }

        public override bool IsValid(object value)
        {
            if (value == null) return true;
            return value.ToString().ToLower().EndsWith("@student.hogent.be");
        }
    }
}