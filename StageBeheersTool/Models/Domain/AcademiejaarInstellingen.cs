using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StageBeheersTool.Models.Domain
{
    /* Beheer academiejaren: administrator moet per academiejaar een aantal parameters 
       kunnen aanmaken, wijzigen , verwijderen. 
       Deze parameters zijn:
       - huidig academiejaar
       - per semester kunnen aanduiden wanneer de stageperiode valt (begindatum - einddatum)
       - de einddatum aanduiden waarop bedrijven de aangeboden stages nog kunnen wijzigen
     */
    public class AcademiejaarInstellingen
    {
        public string Academiejaar { get; set; }
        public DateTime? Semester1Begin { get; set; }
        public DateTime? Semester1Einde { get; set; }
        public DateTime? Semester2Begin { get; set; }
        public DateTime? Semester2Einde { get; set; }
        public DateTime? DeadlineBedrijfStageEdit { get; set; }
        //...


        public string StageperiodeSemester1()
        {
            if (Semester1Begin != null && Semester1Einde != null)
            {
                return string.Format("({0} - {1})",
                    ((DateTime)Semester1Begin).ToString("dd/MM/yyyy"),
                    ((DateTime)Semester1Einde).ToString("dd/MM/yyyy"));
            }
            return "";
        }

        public string StageperiodeSemester2()
        {
            if (Semester2Begin != null && Semester2Einde != null)
            {
                return string.Format("({0} - {1})",
                    ((DateTime)Semester2Begin).ToString("dd/MM/yyyy"),
                    ((DateTime)Semester2Einde).ToString("dd/MM/yyyy"));
            }
            return "";
        }

        public string DeadlineBedrijfStageEditToString()
        {
            if (DeadlineBedrijfStageEdit == null)
            {
                return "";
            }
            return "Gegevens mogen aangepast worden tot: " + ((DateTime)DeadlineBedrijfStageEdit).ToString("dd/MM/yyyy");
        }
    }
}