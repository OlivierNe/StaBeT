using System;

namespace StageBeheersTool.Helpers
{
    public class AcademiejaarHelper
    {
        public static string HuidigAcademiejaar()
        {
            if (DateTime.Now.Month >= 9)
            {
                return DateTime.Now.Year + "-" + (DateTime.Now.Year + 1);
            }
            return (DateTime.Now.Year - 1) + "-" + DateTime.Now.Year;
        }
    }
}