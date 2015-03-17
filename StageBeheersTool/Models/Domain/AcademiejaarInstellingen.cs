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
    }
}