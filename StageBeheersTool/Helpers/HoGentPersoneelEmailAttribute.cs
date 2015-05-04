using System.ComponentModel.DataAnnotations;

namespace StageBeheersTool.Helpers
{
    public class HoGentPersoneelEmailAttribute : ValidationAttribute
    {
        public HoGentPersoneelEmailAttribute()
        {
            ErrorMessage = "Ongeldig HoGent E-mailadres";
        }
        public override bool IsValid(object value)
        {
            if (value == null) return true;
            return value.ToString().ToLower().EndsWith("@hogent.be");
        }

    }
}