using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StageBeheersTool
{
    public class Helpers
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