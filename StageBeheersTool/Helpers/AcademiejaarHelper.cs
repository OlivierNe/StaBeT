using System;
using System.Configuration;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using StageBeheersTool.Models.Domain;

namespace StageBeheersTool.Helpers
{
    public class AcademiejaarHelper
    {
        public static string HuidigAcademiejaar()
        {
            object academiejaar = HttpContext.Current.Cache["academiejaar"];

            if (academiejaar != null)
            {
                return academiejaar.ToString();
            }
            var instellingenRepository =
                DependencyResolver.Current.GetService(typeof(IInstellingenRepository)) as IInstellingenRepository;
            var beginNieuwAcademiejaar = instellingenRepository.Find(Instelling.BeginNieuwAcademiejaar) ??
                new Instelling("", new DateTime(1, 9, 1).ToString());
            var datum = beginNieuwAcademiejaar.DateTimeValue;
            datum = datum.AddYears(DateTime.Now.Year - datum.Year);
            if (DateTime.Now >= datum)
            {
                academiejaar = DateTime.Now.Year + "-" + (DateTime.Now.Year + 1);
            }
            else
            {
                academiejaar = (DateTime.Now.Year - 1) + "-" + DateTime.Now.Year;
            }
            HttpContext.Current.Cache.Add("academiejaar", academiejaar, null, DateTime.Now.AddDays(1),
                Cache.NoSlidingExpiration, CacheItemPriority.Default, null);//cache voor een dag (niet elk request naar db)
            return academiejaar.ToString();
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