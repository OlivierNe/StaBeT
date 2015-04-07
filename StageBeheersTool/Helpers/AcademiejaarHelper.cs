using System;
using System.Configuration;

namespace StageBeheersTool.Helpers
{
    public class AcademiejaarHelper
    {
        public static string HuidigAcademiejaar()
        {
            int maand = int.Parse(ConfigurationManager.AppSettings["MaandBeginNieuwSemester"]);

            if (DateTime.Now.Month >= maand)
            {
                return DateTime.Now.Year + "-" + (DateTime.Now.Year + 1);
            }
            return (DateTime.Now.Year - 1) + "-" + DateTime.Now.Year;
        }

        public static bool VroegerDanHuidig(string academiejaar)
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