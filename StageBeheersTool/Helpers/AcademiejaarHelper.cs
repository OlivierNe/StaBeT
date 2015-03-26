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

        public static bool KleinderDanHuidig(string academiejaar)
        {
            var huidig = HuidigAcademiejaar();
            if ((int.Parse(academiejaar.Substring(0, 4)) < int.Parse(huidig.Substring(0, 4))))
            {
                return true;
            }
            return false;
        }
    }
}